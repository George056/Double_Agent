using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using Unity.MLAgents.Sensors;

public enum Difficulty
{
    Easy = 0,
    Hard = 1
}

/// <summary>
/// This class is used as the AI for the game. 
/// It is constructed based on the Unity course at: https://learn.unity.com/course/ml-agents-hummingbirds
/// More nural net info can be found at: https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md
/// </summary>
public class AI : Agent
{
    [Tooltip("A list of all AI resources with indexes 0 = red, 1 = blue, 2 = yellow, and 3 = green.")]
    public List<int> __resources = new List<int>(4) { 0, 0, 0, 0 };

    [HideInInspector]
    [Tooltip("What difficulty AI is being used; 0 = easy, 1 = hard")]
    public Difficulty __difficulty;
    [HideInInspector]
    public int __ai_score;
    [HideInInspector]
    public int __human_score;

    [Tooltip("An integer that says if the AI is orange (0) or purple (1)")]
    public short __player;

    [Tooltip("The piece does the AI play, 0 = US, 1 = USSR")]
    public Owner __piece_type;

    // Variables for training {

    [Tooltip("Whether this is in training mode or not")]
    public bool trainingMode;

    [Tooltip("The reward/punishment for getting/loosing the longest net")]
    public float longestNetReward = 0.02f;

    [Tooltip("Capture tile reward")]
    public float captureReward = 0.01f;

    [Tooltip("Red/Blue node reward")]
    public float nodeRBReward = 0.02f;

    [Tooltip("Green/Yellow node reward")]
    public float nodeGYReward = 0.04f;

    [Tooltip("Gray node reward")]
    public float nodeGrayReward = 0.01f;

    [HideInInspector]
    public float totalReward = 0;

    // } End variables for training

    [Tooltip("This holds the numeric ID of the roads that have been captured")]
    private List<int> __myRoads;

    [Tooltip("This holds the numeric ID of the nodes that have been captured")]
    private List<int> __myNodes;

    [Tooltip("Used to tell if it is the first move or not")]
    private bool opener;

    [Tooltip("This tells the AI to do random moves")]
    public bool randAI;

    [Tooltip("This counts what turn it is")]
    private int turn;

    [Tooltip("This is used to say the last turn that a mask was generated")]
    private int lastUpdateTurn;

    [Tooltip("The active BoardManager")]
    private BoardManager bm;

    private bool loss = false;
    private bool win = false;

    private void Awake()
    {
        opener = true;
        __myRoads = new List<int>();
        __myNodes = new List<int>();
        if(!randAI) GetDifficulty();
        GetPlayer();
        GetPiece();
        __ai_score = 0;
        __human_score = 0;
        opener = __player == 0;
        turn = (int)__player;
        bm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BoardManager>();
        __resources = new List<int>(4) { 0, 0, 0, 0 };
        totalReward = 0;
        loss = false;
        win = false;
    }

    public void EndOpener()
    {
        opener = false;
    }

    public void SetOpener()
    {
        opener = true;
    }

    /// <summary>
    /// This is the function that is called to tell the AI to make its move
    /// </summary>
    public void AIMove(int turn)
    {
        this.turn = turn;

        Debug.Log("AI Resources: " + __resources[0] + ", " + __resources[1] + ", " + __resources[2] + ", " + __resources[3]);

        if (this.turn < 5) SetOpener();
        else EndOpener();

        if (__human_score >= 10) Loss();
        else if (__ai_score >= 10) Win();

        if (loss || win) return;

        //make move
        if (randAI)
        {
            RandomAIMove();
        }
        else
        {
            //for adding a reward use AddReward() want it to be about 1 at the end of a game
            RequestDecision();
        }

        bm.EndTurn();
    }

    /// <summary>
    /// This makes the already clamed nodes be imposible to be chosen by the nural net
    /// </summary>
    /// <param name="actionMasker">Masks posible values of the nural net</param>
    public override void CollectDiscreteActionMasks(DiscreteActionMasker actionMasker)
    {
        if (randAI) return;
        if (turn == lastUpdateTurn) return;
        lastUpdateTurn = turn;

        if (opener)
        {
            Debug.Log("Mask opener moves");
            List<int> clamedNodes = new List<int>();
            for(int i = 0; i < bm.nodes.Length; i++)
            {
                if(bm.nodes[i].GetComponent<NodeInfo>().nodeOwner != Owner.Nil)
                {
                    clamedNodes.Add(i);
                }
            }

            List<int> clamedBranches = new List<int>();
            for(int i = 0; i < bm.allBranches.Length; i++)
            {
                if(bm.allBranches[i].GetComponent<BranchInfo>().branchOwner != Owner.Nil)
                {
                    clamedBranches.Add(i);
                }
            }

            foreach(int i in clamedNodes)
            {
                actionMasker.SetMask(i, new int[1] { 1 });
            }

            foreach(int i in clamedBranches)
            {
                actionMasker.SetMask(i + 24, new int[1] { 1 });
            }
        }

        if(turn >= 5)
        {
            Debug.Log("Debug normal moves");

            //find max branch amount
            int maxCons = Math.Min(__resources[0], __resources[1]);
            
            //restrict branches that cannot be reached
            List<int> branchesThatCanBeReached = new List<int>();

            if (maxCons != 0)
            {
                List<int> additions = new List<int>();

                for(int i = 0; i < __myRoads.Count; i++)
                {
                    if (Relationships.connectionsRoad.TryGetValue(__myRoads[i], out List<int> temp)) additions.AddRange(temp);
                }
                additions = additions.Distinct().ToList();

                //remove if owned
                for(int i = 0; i < additions.Count; i++)
                {
                    if (bm.allBranches[additions[i]].GetComponent<BranchInfo>().branchOwner != Owner.Nil)
                    {
                        additions.Remove(additions[i]);
                        i--;
                    }
                }

                maxCons--;
                branchesThatCanBeReached.AddRange(additions);
                branchesThatCanBeReached = branchesThatCanBeReached.Distinct().ToList();

                while(maxCons > 0)
                {
                    List<int> temp = new List<int>();
                    foreach(int i in additions) Relationships.connectionsRoad.TryGetValue(i, out temp);
                    temp = temp.Distinct().ToList();
                    branchesThatCanBeReached.AddRange(additions);
                    additions = temp;
                    maxCons--;
                }
                branchesThatCanBeReached.AddRange(additions);
                branchesThatCanBeReached = branchesThatCanBeReached.Distinct().ToList();
            }

            List<int> blockedBranches = new List<int>();
            for (int i = 0; i < bm.allBranches.Length; i++) blockedBranches.Add(i);

            //remove branches that can be reached
            if(branchesThatCanBeReached.Count != 0)
            {
                for(int i = 0; i < branchesThatCanBeReached.Count; i++)
                {
                    blockedBranches.Remove(branchesThatCanBeReached[i]);
                    i--;
                }
            }

            foreach(int i in blockedBranches)
            {
                actionMasker.SetMask(i, new int[1] { 1 });
            }

            //restrict nodes that cannot be reached
            int maxNodes = Math.Min(__resources[2] / 2, __resources[3] / 2);
            List<int> nodesThatCanBeReached = new List<int>();
            
            if(maxNodes > 0)
            {
                List<int> additions = new List<int>();
                branchesThatCanBeReached.AddRange(__myRoads);
                foreach(int i in branchesThatCanBeReached)
                {
                    if (Relationships.connectionsRoadNode.TryGetValue(i, out List<int> temp)) additions.AddRange(temp);
                }
                additions = additions.Distinct().ToList();

                //remove if owned
                for(int i = 0; i < additions.Count; i++)
                {
                    if (bm.nodes[additions[i]].GetComponent<NodeInfo>().nodeOwner != Owner.Nil) additions.Remove(additions[i]);
                }
                nodesThatCanBeReached.AddRange(additions);
            }

            List<int> blockedNodes = new List<int>();
            for (int i = 0; i < bm.nodes.Length; i++) blockedNodes.Add(i);

            for(int i = 0; i < nodesThatCanBeReached.Count; i++) blockedNodes.Remove(nodesThatCanBeReached[i]);

            foreach (int i in blockedNodes) actionMasker.SetMask(i, new int[1] { 1 });

        }
    }

    public void GetLongestNet()
    {
        AddReward(longestNetReward);
        totalReward += longestNetReward;
    }

    public void LoseLongestNet()
    {
        AddReward(-longestNetReward);
        totalReward -= longestNetReward;
        __ai_score -= 2;
    }

    public void CapturedTile(Color c)
    {
        if(c == Color.blue || c == Color.red)
        {
            AddReward(2 * captureReward);
            totalReward += 2 * captureReward;
        }
        else if(c == Color.green || c == Color.yellow)
        {
            AddReward(4 * captureReward);
            totalReward += 4 * captureReward;
        }
        else //if gray
        {
            AddReward(captureReward);
            totalReward += captureReward;
        }
    }

    public void UpdateScore(int score, bool longestGet, bool longestLost)
    {
        AddReward((score - ((longestGet) ? 2 : 0)) - (__ai_score - ((longestLost) ? 2 : 0)));
        totalReward += (score - ((longestGet) ? 2 : 0)) - (__ai_score - ((longestLost) ? 2 : 0));
        __ai_score = score;
        bm.UpdateOpponentScoreInUI(__ai_score);
    }

    public void Loss()
    {
        if (totalReward > 0) AddReward(-totalReward);
        else if(totalReward < 0) AddReward(totalReward);
        AddReward(-1);
        loss = true;
    }

    public void Win()
    {
        if(totalReward != 1)
        {
            AddReward(1 - totalReward);
        }
    }

    /// <summary>
    /// This initializes the AI
    /// </summary>
    public override void Initialize()
    {
        Awake();
        //if not in training mode, no max step
        if (!trainingMode) MaxStep = 0;
    }

    public void UpdateResources(List<int> update)
    {
        Debug.Log("AI Earned Resources: " + update[0] + ", " + update[1] + ", " + update[2] + ", " + update[3]);
        for (int i = 0; i < __resources.Count; i++)
        {
            __resources[i] += update[i];
        }
    }

    /// <summary>
    /// Reset the agent when an episode begins
    /// </summary>
    public override void OnEpisodeBegin()
    {
        __ai_score = 0;
        __human_score = 0;
    }

    /// <summary>
    /// <para>Called when an action is received</para>
    /// 
    /// <para>vectorActions[i] represents:</para>
    /// <para>Index 0: Place node A (0 = don't place, 1 = place)
    ///       Index 1: Place node B (0 = don't place, 1 = place)
    ///       Index 2: Place node C (0 = don't place, 1 = place)</para>
    /// <para>Index 3: Place node D (0 = don't place, 1 = place)
    ///       Index 4: Place node E (0 = don't place, 1 = place)
    ///       Index 5: Place node F (0 = don't place, 1 = place)</para>
    /// <para>Index 6: Place node G (0 = don't place, 1 = place)
    ///       Index 7: Place node H (0 = don't place, 1 = place)
    ///       Index 8: Place node I (0 = don't place, 1 = place)</para>
    /// <para>Index 9: Place node J (0 = don't place, 1 = place)
    ///       Index 10: Place node K (0 = don't place, 1 = place)
    ///       Index 11: Place node L (0 = don't place, 1 = place)</para>
    /// <para>Index 12: Place node M (0 = don't place, 1 = place)
    ///       Index 13: Place node N (0 = don't place, 1 = place)
    ///       Index 14: Place node O (0 = don't place, 1 = place)</para>
    /// <para>Index 15: Place node P (0 = don't place, 1 = place)
    ///       Index 16: Place node Q (0 = don't place, 1 = place)
    ///       Index 17: Place node R (0 = don't place, 1 = place)</para>
    /// <para>Index 18: Place node S (0 = don't place, 1 = place)
    ///       Index 19: Place node T (0 = don't place, 1 = place)
    ///       Index 20: Place node U (0 = don't place, 1 = place)</para>
    /// <para>Index 21: Place node V (0 = don't place, 1 = place)
    ///       Index 22: Place node W (0 = don't place, 1 = place)
    ///       Index 23: Place node X (0 = don't place, 1 = place)</para>
    /// <para>Index 24: Place connector 1 (0 = don't place, 1 = place)
    ///       Index 25: Place connector 2 (0 = don't place, 1 = place)
    ///       Index 26: Place connector 3 (0 = don't place, 1 = place)</para>
    /// <para>Index 27: Place connector 4 (0 = don't place, 1 = place)
    ///       Index 28: Place connector 5 (0 = don't place, 1 = place)
    ///       Index 29: Place connector 6 (0 = don't place, 1 = place)</para>
    /// <para>Index 30: Place connector 7 (0 = don't place, 1 = place)
    ///       Index 31: Place connector 8 (0 = don't place, 1 = place)
    ///       Index 32: Place connector 9 (0 = don't place, 1 = place)</para>
    /// <para>Index 33: Place connector 10 (0 = don't place, 1 = place)
    ///       Index 34: Place connector 11 (0 = don't place, 1 = place)
    ///       Index 35: Place connector 12 (0 = don't place, 1 = place)</para>
    /// <para>Index 36: Place connector 13 (0 = don't place, 1 = place)
    ///       Index 37: Place connector 14 (0 = don't place, 1 = place)
    ///       Index 38: Place connector 15 (0 = don't place, 1 = place)</para>
    /// <para>Index 39: Place connector 16 (0 = don't place, 1 = place)
    ///       Index 40: Place connector 17 (0 = don't place, 1 = place)
    ///       Index 41: Place connector 18 (0 = don't place, 1 = place)</para>
    /// <para>Index 42: Place connector 19 (0 = don't place, 1 = place)
    ///       Index 43: Place connector 20 (0 = don't place, 1 = place)
    ///       Index 44: Place connector 21 (0 = don't place, 1 = place)</para>
    /// <para>Index 45: Place connector 22 (0 = don't place, 1 = place)
    ///       Index 46: Place connector 23 (0 = don't place, 1 = place)
    ///       Index 47: Place connector 24 (0 = don't place, 1 = place)</para>
    /// <para>Index 48: Place connector 25 (0 = don't place, 1 = place)
    ///       Index 49: Place connector 26 (0 = don't place, 1 = place)
    ///       Index 50: Place connector 27 (0 = don't place, 1 = place)</para>
    /// <para>Index 51: Place connector 28 (0 = don't place, 1 = place)
    ///       Index 52: Place connector 29 (0 = don't place, 1 = place)
    ///       Index 53: Place connector 30 (0 = don't place, 1 = place)</para>
    /// <para>Index 54: Place connector 31 (0 = don't place, 1 = place)
    ///       Index 55: Place connector 32 (0 = don't place, 1 = place)
    ///       Index 56: Place connector 33 (0 = don't place, 1 = place)</para>
    /// <para>Index 57: Place connector 34 (0 = don't place, 1 = place)      
    ///       Index 58: Place connector 35 (0 = don't place, 1 = place)      
    ///       Index 59: Place connector 36 (0 = don't place, 1 = place)</para>
    /// <para>Index 60: Make a trade (0 = no trade; 1 = gggy; 2 = gggr; 3 = gggb; 4 = ggyr; 5 = ggyb; 6 = ggry; 7 = ggrb; 8 = ggby; 9 = ggbr; 10 = gyyr; 11 = gyyb;
    /// 12 = gyrb; 13 = gybr; 14 = grry; 15 = grrb; 16 = grby; 17 = gbby; 18 = gbbr; 19 = yyyg; 20 = yyyr; 21 = yyyb; 22 = yyrg; 23 = yyrb; 24 = yybg; 25 = yybr;
    /// 26 = yrrg; 27 = yrrb; 28 = ybbg; 29 = ybbr; 30 = yrbg; 31 = rrrg; 32 = rrry; 33 = rrrb; 34 = rrbg; 35 = rrby; 36 = rbbg; 37 = rbby; 38 = bbbg; 39 = bbby;
    /// 40 = bbbr)</para>
    /// </summary>
    /// <param name="vectorAction">List of actions to take</param>
    public override void OnActionReceived(float[] vectorAction)
    {
        if (randAI) return;
        //make a trade first
        if(vectorAction[60] != 0)
        {
            List<int> tradeArr = new List<int>(4) { 0, 0, 0, 0 };
            int temp1 = (int)vectorAction[60];
            int temp2 = (temp1 / 1000);
            tradeArr[0] = temp2;
            temp1 -= temp2 * 1000;
            temp2 = temp1 / 100;
            tradeArr[1] = temp2;
            temp1 -= temp2 * 100;
            temp2 = temp1 / 10;
            tradeArr[2] = temp2;
            temp1 -= temp2 * 10;
            tradeArr[3] = temp1;
            MakeTrade(tradeArr);
        }
        
        //place connectors
        for(int i = 0; i < 36; i++)
        {
            if(vectorAction[i + 24] == 1)
            {
                if (LegalMoveConnector(i))
                {
                    Debug.Log("Connector: " + i);
                    PlaceMoveBranch(i);
                    __myRoads.Add(i);
                }
            }
        }

        //place nodes
        for(int i = 0; i < 24; i++)
        {
            if(vectorAction[i] == 1)
            {
                if (LegalMoveNode(i))
                {
                    Debug.Log("Node: " + i);
                    PlaceMoveNode(i);
                    __myRoads.Add(i);
                    if(trainingMode)
                        foreach(GameObject c in bm.nodes[i].GetComponent<NodeInfo>().resources)//account for depleted and captured
                        {
                            if(c.GetComponent<ResourceInfo>().depleted == true)
                            {
                                AddReward(nodeGrayReward / 2);
                                totalReward += nodeGrayReward / 2;
                            }
                            else if(c.GetComponent<ResourceInfo>().resoureTileOwner == ((__piece_type == Owner.US) ? Owner.USSR : Owner.US))
                            {
                                AddReward(nodeGrayReward / 3);
                                totalReward += nodeGrayReward / 3;
                            }
                            else if(c.GetComponent<ResourceInfo>().nodeColor == ResourceInfo.Color.Blue || c.GetComponent<ResourceInfo>().nodeColor == ResourceInfo.Color.Red)
                            {
                                AddReward(nodeRBReward * ((c.GetComponent<ResourceInfo>().resoureTileOwner == __piece_type) ? 2 : 1));
                                totalReward += nodeRBReward * ((c.GetComponent<ResourceInfo>().resoureTileOwner == __piece_type) ? 2 : 1);
                            }
                            else if (c.GetComponent<ResourceInfo>().nodeColor == ResourceInfo.Color.Green || c.GetComponent<ResourceInfo>().nodeColor == ResourceInfo.Color.Yellow)
                            {
                                AddReward(nodeGYReward * ((c.GetComponent<ResourceInfo>().resoureTileOwner == __piece_type) ? 2 : 1));
                                totalReward += nodeGYReward * ((c.GetComponent<ResourceInfo>().resoureTileOwner == __piece_type) ? 2 : 1);
                            }
                            else
                            {
                                AddReward(nodeGrayReward * ((c.GetComponent<ResourceInfo>().resoureTileOwner == __piece_type) ? 2 : 1));
                                totalReward += nodeGrayReward * ((c.GetComponent<ResourceInfo>().resoureTileOwner == __piece_type) ? 2 : 1);
                            }
                        }
                }
            }
        }

        __myNodes = __myNodes.Distinct().ToList();
        __myRoads = __myRoads.Distinct().ToList();
    }

    /// <summary>
    /// Collect vector observations from the environment
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        if (randAI) return;
        //observe board (60 observations)
        //sensor.AddObservation();
        Debug.Log("observe nodes");
        foreach(GameObject node in bm.nodes)
        {
            List<int> observations = new List<int>(7) { 0, 0, 0, 0, 0, 0, 0 };// node owner; # tiles AI captured; gray count; red count; blue count; yellow count; green count;
            NodeInfo nodeTemp = node.GetComponent<NodeInfo>();

            observations[0] = ((int)nodeTemp.nodeOwner == 2) ? 0 : (int)nodeTemp.nodeOwner + 1;

            foreach (GameObject tile in nodeTemp.resources)
            {
                ResourceInfo tileTemp = tile.GetComponent<ResourceInfo>();
                if (tileTemp.resoureTileOwner == __piece_type) observations[1] += 1;
                if (tileTemp.depleted == true || tileTemp.nodeColor == ResourceInfo.Color.Empty || (tileTemp.resoureTileOwner != Owner.Nil) && (tileTemp.resoureTileOwner != __piece_type)) observations[2] += 1;
                else if (tileTemp.nodeColor == ResourceInfo.Color.Red) observations[3] += 1;
                else if (tileTemp.nodeColor == ResourceInfo.Color.Blue) observations[4] += 1;
                else if (tileTemp.nodeColor == ResourceInfo.Color.Yellow) observations[5] += 1;
                else if (tileTemp.nodeColor == ResourceInfo.Color.Green) observations[6] += 1;
            }
            int observation = 0;
            for(int i = (int)10E5, j = 0; j < observations.Count; i /= 10, j++)
            {
                observation += i * observations[j];
            }
            sensor.AddObservation(observation);
        }

        Debug.Log("Observe branches");
        foreach(GameObject branch in bm.allBranches)
        {
            sensor.AddObservation(((int)branch.GetComponent<BranchInfo>().branchOwner == 2) ? 0 : (int)branch.GetComponent<BranchInfo>().branchOwner + 1);
        }

        //observe if opener (1 observation)
        sensor.AddObservation(opener);

        Debug.Log("Observe resources");
        //observe resources (4 observations)
        foreach(int i in __resources)
        {
            sensor.AddObservation(i);
        }
        Debug.Log("End observations");
    }

    /// <summary>
    /// When behavior type is set to "Hueristic Only" on the agent's Behavior Parameters,
    /// this function will be called. Its return values will be fed into
    /// <see cref="OnActionReceived(float[])"/> instead of using the neural net
    /// </summary>
    /// <param name="actionsOut">The output of the function, returned to OnActionReceived</param>
    public override void Heuristic(float[] actionsOut)
    {
        for (int i = 0; i < 61; i++)
        {
            actionsOut[i] = 0;
        }

        RandomAIMove();
    }

    private void MakeTrade(List<int> trade)
    {
        bm.Trade(trade, __piece_type);
    }

    private bool LegalMoveNode(int location)
    {
        return (opener || __resources[2] > 1 && __resources[3] > 1) ? bm.LegalNodeMove(location, __piece_type, __myRoads) : false;
    }

    private bool LegalMoveConnector(int location)
    {
        return (opener || __resources[0] > 0 && __resources[1] > 0) ? bm.LegalBranchMove(location, __piece_type, __myRoads) : false;
    }

    /// <summary>
    /// Captures the node
    /// </summary>
    /// <param name="location">The node to be captured</param>
    private void PlaceMoveNode(int location)
    {
        bm.ChangeNodeOwner(location);
        if (!opener)
        {
            __resources[2] -= 2;
            __resources[3] -= 2;
        }
    }

    /// <summary>
    /// Captures the branch
    /// </summary>
    /// <param name="location">The branch to capture</param>
    private void PlaceMoveBranch(int location)
    {
        bm.ChangeBranchOwner(location);
        if (!opener)
        {
            __resources[0] -= 1;
            __resources[1] -= 1;
        }
    }

    /// <summary>
    /// This finds and sets the difficulty based on the PlayerPref Difficulty
    /// Defaults to easy
    /// </summary>
    void GetDifficulty()
    {
        __difficulty = (Difficulty)PlayerPrefs.GetInt("Difficulty", 0);
    }

    /// <summary>
    /// This finds and sets the player position based on the PlayerPref AI_Player
    /// Defaults to player 2 (purple)
    /// </summary>
    void GetPlayer()
    {
        __player = (short)PlayerPrefs.GetInt("AI_Player", 1);
    }

    /// <summary>
    /// Find out what "color" piece you based on the PlayerPref AI_Piece
    /// Defaults to USSR
    /// </summary>
    void GetPiece()
    {
        __piece_type = (Owner)PlayerPrefs.GetInt("AI_Piece", 1);
    }

    private void RandomAIMove(){
        if (opener)
        {
            int positionCon;

            do
            {
                positionCon = Random.Range(0, 36);
            } while (!LegalMoveConnector(positionCon));//exit when a legal move is found
            PlaceMoveBranch(positionCon);
            Debug.Log("Connector: " + positionCon);
            __myRoads.Add(positionCon);

            int positionNode;

            positionNode = Random.Range(0, 1);
            Relationships.connectionsRoadNode.TryGetValue(positionCon, out var temp);
            if (LegalMoveNode(temp[positionNode]))
            {
                PlaceMoveNode(temp[positionNode]);
                __myNodes.Add(temp[positionNode]);
            }
            else
            {
                PlaceMoveNode(temp[(positionNode == 0) ? 1 : 0]);
                __myNodes.Add(temp[(positionNode == 0) ? 1 : 0]);
            }
            Debug.Log("Node: " + temp[positionNode]);
        }
        else
        {
            int maxNodes = Math.Min(__resources[2] / 2, __resources[3] / 2);
            int maxCons = Math.Min(__resources[0], __resources[1]);

            int counter = 0;

            List<int> legalNodes = new List<int>();
            List<int> legalCon = new List<int>();
            foreach (int i in __myRoads)//get all connections to owned branches
            {
                if (Relationships.connectionsRoad.TryGetValue(i, out var outputC)) legalCon.AddRange(outputC);
            }
            //remove duplicates found at: https://stackoverflow.com/questions/47752/remove-duplicates-from-a-listt-in-c-sharp

            legalCon = legalCon.Distinct().ToList();
            //int consToPlace = Random.Range(0, maxCons);

            //place a legal connection when found and make a list and do at once
            for (int i = 0; i < maxCons && i <= legalCon.Count && counter < 50; i++, counter++) //this cannot happen
            {
                int con = Random.Range(0, (legalCon.Count == 0) ? 0 : legalCon.Count - 1);
                if (legalCon.Count > 0)
                {
                    Relationships.connectionsRoadTiles.TryGetValue(legalCon[con], out List<int> temp);
                    if (LegalMoveConnector(legalCon[con]) &&
                        (bm.resourceList[temp[0]].GetComponent<ResourceInfo>().resoureTileOwner == Owner.Nil &&
                        (temp.Count > 1 && bm.resourceList[temp[1]].GetComponent<ResourceInfo>().resoureTileOwner == Owner.Nil)))
                    {
                        PlaceMoveBranch(legalCon[con]);
                        __myRoads.Add(legalCon[con]);
                        Debug.Log("Connector: " + legalCon[con]);
                    }
                    else
                    {
                        i--;
                    }
                    legalCon.Remove(legalCon[con]); // remove the branch; if added it's already used, if not then it was illegal
                }
            }

            foreach (int i in __myRoads)
            {
                if (Relationships.connectionsRoadNode.TryGetValue(i, out var outputN)) legalNodes.AddRange(outputN);
            }

            legalNodes = legalNodes.Distinct().ToList();
            //int nodesToPlace = Random.Range(0, maxNodes);

            counter = 0;

            for (int i = 0; i < maxNodes && i <= legalNodes.Count && counter < 50; i++, counter++)
            {
                int node = Random.Range(0, (legalNodes.Count == 0) ? 0 : legalNodes.Count - 1);
                if (legalNodes.Count > 0)
                {
                    if (LegalMoveNode(legalNodes[node]))//if a legal move add it
                    {
                        int tileCount = bm.nodes[legalNodes[node]].GetComponent<NodeInfo>().resources.Count;
                        foreach (GameObject go in bm.nodes[legalNodes[node]].GetComponent<NodeInfo>().resources)
                        {
                            if (go.GetComponent<ResourceInfo>().depleted && go.GetComponent<ResourceInfo>().resoureTileOwner != __piece_type)
                            {
                                tileCount--;
                            }
                        }
                        if (tileCount > Mathf.Ceil(bm.nodes[legalNodes[node]].GetComponent<NodeInfo>().resources.Count / 2) || (10 - __ai_score) < maxNodes)
                        {
                            PlaceMoveNode(legalNodes[node]);
                            __myNodes.Add(legalNodes[node]);
                            Debug.Log("Node: " + legalNodes[node]);
                        }

                    }
                    else
                    {
                        i--;
                    }
                    legalNodes.Remove(legalNodes[node]);
                }
            }

            maxNodes = Math.Min(__resources[2] / 2, __resources[3] / 2);
            maxCons = Math.Min(__resources[0], __resources[1]);

            //make trade
            if (Random.Range(0, 6) != 0 && (maxCons == 0 || maxNodes == 0))
            {
                List<int> trade = new List<int>(4) { 0, 0, 0, 0 };

                int traded = 0;
                int blocked = 0;
                while (traded < 3 && blocked < 3)
                {
                    blocked = 0;

                    if (__resources[0] == 0 && __resources[1] != 0)
                    {
                        if (__resources[1] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[1] -= 1;
                            trade[1] -= 1;
                        }
                        else blocked++;
                        if (__resources[2] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[2] -= 1;
                            trade[2] -= 1;
                        }
                        else blocked++;
                        if (__resources[3] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[3] -= 1;
                            trade[3] -= 1;
                        }
                        else blocked++;
                    }
                    else if (__resources[1] == 0 && __resources[0] != 0)
                    {
                        if (__resources[0] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[0] -= 1;
                            trade[0] -= 1;
                        }
                        else blocked++;
                        if (__resources[2] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[2] -= 1;
                            trade[2] -= 1;
                        }
                        else blocked++;
                        if (__resources[3] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[3] -= 1;
                            trade[3] -= 1;
                        }
                        else blocked++;
                    }
                    else if (((__resources[2] % 2) == 1 || (__resources[2] == 0)) && (__resources[3] >= 2))
                    {
                        if (__resources[0] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[0] -= 1;
                            trade[0] -= 1;
                        }
                        else blocked++;
                        if (__resources[1] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[1] -= 1;
                            trade[1] -= 1;
                        }
                        else blocked++;
                        if (__resources[3] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[3] -= 1;
                            trade[3] -= 1;
                        }
                        else blocked++;
                    }
                    else if (((__resources[3] % 2 == 1) || (__resources[3] == 0)) && __resources[2] >= 2)
                    {
                        if (__resources[0] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[0] -= 1;
                            trade[0] -= 1;
                        }
                        else blocked++;
                        if (__resources[1] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[1] -= 1;
                            trade[1] -= 1;
                        }
                        else blocked++;
                        if (__resources[2] > 0 && traded < 3)
                        {
                            traded++;
                            __resources[2] -= 1;
                            trade[2] -= 1;
                        }
                        else blocked++;
                    }
                    else blocked = 3;
                }

                for (int i = 0; i < trade.Count; i++)
                {
                    __resources[i] -= trade[i];
                }

                if (blocked != 3)
                {
                    if (__resources[0] == 0 && __resources[1] != 0)
                    {
                        trade[0] += 1;
                    }
                    else if (__resources[1] == 0 && __resources[0] != 0)
                    {
                        trade[1] += 1;
                    }
                    else if (((__resources[2] % 2) == 1 || (__resources[2] == 0)) && (__resources[3] >= 2))
                    {
                        trade[2] += 1;
                    }
                    else if (((__resources[3] % 2 == 1) || (__resources[3] == 0)) && __resources[2] >= 2)
                    {
                        trade[3] += 1;
                    }
                    MakeTrade(trade);
                    Debug.Log("Trade: " + trade[0] + ", " + trade[1] + ", " + trade[2] + ", " + trade[3]);

                }

            }
        }
    }
}
