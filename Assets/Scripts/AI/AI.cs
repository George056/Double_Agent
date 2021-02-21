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
    Easy,
    Hard
}

public enum Resource
{
    red = 0,         //copper
    blue = 1,        //lumber
    yellow = 2,      //money
    green = 3,       //loyalists
    nil = 4          //none
}

/// <summary>
/// This class is used as the AI for the game. 
/// It is constructed based on the Unity course at: https://learn.unity.com/course/ml-agents-hummingbirds
/// </summary>
public class AI : Agent
{
    [Tooltip("A list of all AI resources with indexes 0 = red, 1 = blue, 2 = yellow, and 3 = green.")]
    List<int> __resources = new List<int>(4) { 0, 0, 0, 0 };
    [Tooltip("A list of all AI resources last turn, used in determining a draw")]
    List<int> __last_resources = new List<int>(4) { 0, 0, 0, 0 };

    [Tooltip("A list of all player resources, used in determining a draw")]
    List<int> __player_resources = new List<int>(4) { 0, 0, 0, 0 };
    [Tooltip("A list of all player resources last turn, used in determining a draw")]
    List<int> __player_last_resources = new List<int>(4) { 0, 0, 0, 0 };

    [Tooltip("The current board")]
    Board __board;
    [Tooltip("The board last turn, used in determining a draw.")]
    Board __old_board;

    [HideInInspector]
    public Difficulty __difficulty;
    [HideInInspector]
    public int __ai_score;
    [HideInInspector]
    public int __human_score;

    [Tooltip("An integer that says if the AI is orange (0) or purple (1)")]
    public short __player;

    [Tooltip("The piece does the AI play, 0 = US, 1 = USSR")]
    public Owner __piece_type;

    [Tooltip("Whether this is in training mode or not")]
    public bool trainingMode;

    [Tooltip("This holds the numeric ID of the roads that have been captured")]
    private List<int> __myRoads;

    [Tooltip("Used to tell if it is the first move or not")]
    private bool opener;

    [Tooltip("This tells the AI to do random moves")]
    private bool randAI;

    [Tooltip("This counts what turn it is")]
    private int turns;

    private void Start()
    {
        randAI = true;
        __myRoads = new List<int>();
        if(!randAI) GetDifficulty();
        GetPlayer();
        __ai_score = 0;
        __human_score = 0;
        opener = __player == 0;
        turns = (int)__player;
        //get active board
    }

    /// <summary>
    /// This is the function that is called to tell the AI to make its move
    /// </summary>
    /// <param name="newNodes">The new moves of nodes by the human</param>
    /// <param name="newConnectors">The list of new moves of connectors by the human</param>
    /// <param name="AIResources">The resources held by the AI</param>
    /// <param name="playerResources">The resources held by the player</param>
    public void AIMove(List<int> newNodes, List<int> newConnectors, List<int> AIResources, List<int> playerResources)
    {
        //update info

        turns++;
        if(turns == 5)
        {
            opener = false;
        }

        __resources = AIResources;
        __player_resources = playerResources;

        //end update info

        //make move
        if (randAI)
        {
            int maxNodes = Math.Min(__resources[2] % 2, __resources[3] % 2);
            int maxCons = Math.Min(__resources[0], __resources[1]);
            List<char> nodesPlaced = new List<char>();
            List<int> consPlaced = new List<int>();
            if (opener)
            {
                int positionCon;

                do
                {
                    positionCon = Random.Range(0, 36);
                } while (LegalMoveConnector(positionCon));
                consPlaced.Add(positionCon);

                char positionNode;

                do
                {
                    positionNode = (char)Random.Range('a', 'x');
                } while (LegalMoveNode(positionNode));
                nodesPlaced.Add(positionNode);
            }
            else
            {
                List<char> legalNodes = new List<char>();
                List<int> legalCon = new List<int>();
                foreach(int i in __myRoads)
                {
                    if (Relationships.connectionsRoad.TryGetValue(i, out var outputC)) legalCon.AddRange(outputC);
                    if (Relationships.connectionsRoadNode.TryGetValue(i, out var outputN)) legalNodes.AddRange(outputN);
                }
                //remove duplicates found at: https://stackoverflow.com/questions/47752/remove-duplicates-from-a-listt-in-c-sharp
                legalNodes = legalNodes.Distinct().ToList();
                legalCon = legalCon.Distinct().ToList();

                int nodesToPlace = Random.Range(0, maxNodes);
                int consToPlace = Random.Range(0, maxCons);

                //posible error from calculating legal nodes before placing legal branches
                for(int i = 0; i < nodesToPlace; i++)
                {
                    char node = (char)Random.Range(0, legalNodes.Count);
                    if (LegalMoveNode(node))
                    {
                        nodesPlaced.Add(node);
                    }
                    else
                    {
                        legalNodes.Remove(node);
                        i--;
                    }
                }

                for(int i = 0; i < consToPlace; i++)
                {
                    int con = Random.Range(0, legalCon.Count);
                    if (LegalMoveConnector(con))
                    {
                        consPlaced.Add(con);
                    }
                    else
                    {
                        legalCon.Remove(con);
                        i--;
                    }
                }
            }

            //place node and connector ****************************************************************************************************
            foreach(int con in consPlaced)
            {

            }

            foreach(char node in nodesPlaced)
            {

            }
        }
        else //ML move***********************************************************************************************************************
        {
            //for adding a reward use AddReward() want it to be about 1 at the end of a game
        }
    }

    public void GetLongestNet()
    {
        //AddReward();
    }

    public void LoseLongestNet()
    {
        //AddReward(-);
    }

    public void CapturedTile(Color c)
    {
        //addReward();
    }

    public void UpdateScore(int score)
    {
        __ai_score = score;
    }

    /// <summary>
    /// This initializes the AI
    /// </summary>
    public override void Initialize()
    {
        Start();
        //if not in training mode, no max step
        if (!trainingMode) MaxStep = 0;
    }

    public void UpdateResources(List<int> update)
    {
        for(int i = 0; i < __resources.Count; i++)
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
    /// Called when an action is received
    /// 
    /// vectorActions[i] represents:
    /// Index 0: Place node A (0 = don't place, 1 = place)
    /// Index 1: Place node B (0 = don't place, 1 = place)
    /// Index 2: Place node C (0 = don't place, 1 = place)
    /// Index 3: Place node D (0 = don't place, 1 = place)
    /// Index 4: Place node E (0 = don't place, 1 = place)
    /// Index 5: Place node F (0 = don't place, 1 = place)
    /// Index 6: Place node G (0 = don't place, 1 = place)
    /// Index 7: Place node H (0 = don't place, 1 = place)
    /// Index 8: Place node I (0 = don't place, 1 = place)
    /// Index 9: Place node J (0 = don't place, 1 = place)
    /// Index 10: Place node K (0 = don't place, 1 = place)
    /// Index 11: Place node L (0 = don't place, 1 = place)
    /// Index 12: Place node M (0 = don't place, 1 = place)
    /// Index 13: Place node N (0 = don't place, 1 = place)
    /// Index 14: Place node O (0 = don't place, 1 = place)
    /// Index 15: Place node P (0 = don't place, 1 = place)
    /// Index 16: Place node Q (0 = don't place, 1 = place)
    /// Index 17: Place node R (0 = don't place, 1 = place)
    /// Index 18: Place node S (0 = don't place, 1 = place)
    /// Index 19: Place node T (0 = don't place, 1 = place)
    /// Index 20: Place node U (0 = don't place, 1 = place)
    /// Index 21: Place node V (0 = don't place, 1 = place)
    /// Index 22: Place node W (0 = don't place, 1 = place)
    /// Index 23: Place node X (0 = don't place, 1 = place)
    /// Index 24: Place connector 1 (0 = don't place, 1 = place)
    /// Index 25: Place connector 2 (0 = don't place, 1 = place)
    /// Index 26: Place connector 3 (0 = don't place, 1 = place)
    /// Index 27: Place connector 4 (0 = don't place, 1 = place)
    /// Index 28: Place connector 5 (0 = don't place, 1 = place)
    /// Index 29: Place connector 6 (0 = don't place, 1 = place)
    /// Index 30: Place connector 7 (0 = don't place, 1 = place)
    /// Index 31: Place connector 8 (0 = don't place, 1 = place)
    /// Index 32: Place connector 9 (0 = don't place, 1 = place)
    /// Index 33: Place connector 10 (0 = don't place, 1 = place)
    /// Index 34: Place connector 11 (0 = don't place, 1 = place)
    /// Index 35: Place connector 12 (0 = don't place, 1 = place)
    /// Index 36: Place connector 13 (0 = don't place, 1 = place)
    /// Index 37: Place connector 14 (0 = don't place, 1 = place)
    /// Index 38: Place connector 15 (0 = don't place, 1 = place)
    /// Index 39: Place connector 16 (0 = don't place, 1 = place)
    /// Index 40: Place connector 17 (0 = don't place, 1 = place)
    /// Index 41: Place connector 18 (0 = don't place, 1 = place)
    /// Index 42: Place connector 19 (0 = don't place, 1 = place)
    /// Index 43: Place connector 20 (0 = don't place, 1 = place)
    /// Index 44: Place connector 21 (0 = don't place, 1 = place)
    /// Index 45: Place connector 22 (0 = don't place, 1 = place)
    /// Index 46: Place connector 23 (0 = don't place, 1 = place)
    /// Index 47: Place connector 24 (0 = don't place, 1 = place)
    /// Index 48: Place connector 25 (0 = don't place, 1 = place)
    /// Index 49: Place connector 26 (0 = don't place, 1 = place)
    /// Index 50: Place connector 27 (0 = don't place, 1 = place)
    /// Index 51: Place connector 28 (0 = don't place, 1 = place)
    /// Index 52: Place connector 29 (0 = don't place, 1 = place)
    /// Index 53: Place connector 30 (0 = don't place, 1 = place)
    /// Index 54: Place connector 31 (0 = don't place, 1 = place)
    /// Index 55: Place connector 32 (0 = don't place, 1 = place)
    /// Index 56: Place connector 33 (0 = don't place, 1 = place)
    /// Index 57: Place connector 34 (0 = don't place, 1 = place)
    /// Index 58: Place connector 35 (0 = don't place, 1 = place)
    /// Index 59: Place connector 36 (0 = don't place, 1 = place)
    /// Index 60: Make a trade (0 = no trade; 1 = gggy; 2 = gggr; 3 = gggb; 4 = ggyr; 5 = ggyb; 6 = ggry; 7 = ggrb; 8 = ggby; 9 = ggbr; 10 = gyyr; 11 = gyyb;
    /// 12 = gyrb; 13 = gybr; 14 = grry; 15 = grrb; 16 = grby; 17 = gbby; 18 = gbbr; 19 = yyyg; 20 = yyyr; 21 = yyyb; 22 = yyrg; 23 = yyrb; 24 = yybg; 25 = yybr;
    /// 26 = yrrg; 27 = yrrb; 28 = ybbg; 29 = ybbr; 30 = yrbg; 31 = rrrg; 32 = rrry; 33 = rrrb; 34 = rrbg; 35 = rrby; 36 = rbbg; 37 = rbby; 38 = bbbg; 39 = bbby;
    /// 40 = bbbr)
    /// </summary>
    /// <param name="vectorAction">List of actions to take</param>
    public override void OnActionReceived(float[] vectorAction)
    {

        //make a trade first
        if(vectorAction[60] != 0)
        {
            //trade function
        }
        
        //place connectors

        //place nodes

    }

    /// <summary>
    /// Collect vector observations from the environment
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //observe board (60 observations)
        //sensor.AddObservation();

        //observe resources (4 observations)

    }

    /// <summary>
    /// When behavior type is set to "Hueristic Only" on the agent's Behavior Parameters,
    /// this function will be called. Its return values will be fed into
    /// <see cref="OnActionReceived(float[])"/> instead of using the neural net
    /// </summary>
    /// <param name="actionsOut">The output of the function, returned to OnActionReceived</param>
    public override void Heuristic(float[] actionsOut)
    {
        
    }

    private bool LegalMoveNode(char location)
    {
        if(__myRoads.Count == 0)
        {
            return __board.LegalMoveNode(location);
        }
        else
        {
            List<int> legal = new List<int>();
            Relationships.connectionsNode.TryGetValue(location, out legal);
            bool found = false;
            foreach(int i in legal)
            {
                found = __myRoads.Contains(i);
                if (found) break;
            }

            if (found)
            {
                return __board.LegalMoveNode(location);
            }
            else
            {
                return false;
            }
        }
    }

    private bool LegalMoveConnector(int location)
    {
        if(__myRoads.Count == 0)
        {
            return __board.LegalMoveConnector(location);
        }
        else
        {
            List<int> legal = new List<int>();
            Relationships.connectionsRoad.TryGetValue(location, out legal);
            bool found = false;
            foreach (int i in legal)
            {
                found = __myRoads.Contains(i);
                if (found) break;
            }

            if (found)
            {
                return __board.LegalMoveConnector(location);
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// This finds and sets the difficulty based on the PlayerPref Difficulty
    /// </summary>
    void GetDifficulty()
    {
        __difficulty = (Difficulty)PlayerPrefs.GetInt("Difficulty");
    }

    /// <summary>
    /// This finds and sets the player position based on the PlayerPref AI_Player
    /// </summary>
    void GetPlayer()
    {
        __player = (short)PlayerPrefs.GetInt("AI_Player");
    }

    /// <summary>
    /// Find out what "color" piece you based on the PlayerPref AI_Piece
    /// </summary>
    void GetPiece()
    {
        int temp = PlayerPrefs.GetInt("AI_Piece");
        __piece_type = (Owner)temp;
    }

    //calculate longest path*******************************************************************************

    /*
     S_GS + 2*(G+Y) + (B+r) - D  {adjusted for max_nodes}
     */
    double S_a()
    {
        double result = 0;

        return result;
    }

    /*
     S_GO + 2*(2*(G+Y) + (r+B)) + Sigma_c + Sigma_t   {Sigma is block}
     */
    double S_b()
    {
        double result = 0;

        return result;
    }

    /* Calculate trade
     F_N(S_a + S_b) * C_T(R)   {Can you make the move is F_N}
     */
    double S_t()
    {
        double result = 0;

        return result;
    }

}
