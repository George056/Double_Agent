using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;

public enum Difficulty
{
    Easy,
    Hard
}

public enum Resource
{
    red = 0,
    blue = 1,
    yellow = 2,
    green = 3
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
    List<int> __player_resources;
    [Tooltip("A list of all player resources last turn, used in determining a draw")]
    List<int> __player_last_resources;

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

    [HideInInspector][Tooltip("The different connections between the roads")]
    public Dictionary<int, List<int>> connectionsRoad = new Dictionary<int, List<int>>();

    [HideInInspector][Tooltip("The different connections to a node")]
    public Dictionary<char, List<int>> connectionsNode = new Dictionary<char, List<int>>();

    [Tooltip("An integer that says if the AI is orange (0) or purple (1)")]
    public short __player;

    [Tooltip("The piece does the AI play, 0 = US, 1 = USSR")]
    public Owner __piece_type;

    [Tooltip("Whether this is in training mode or not")]
    public bool trainingMode;

    private List<int> __myRoads;

    [Tooltip("Used to tell if it is the first move or not")]
    private bool firstMove;

    private void Start()
    {
        SetUpConnections();
        __myRoads = new List<int>();
        GetDifficulty();
        GetPlayer();
        UpdateScores(0, 0);
        firstMove = __player == 0;
        //get active board
    }

    /// <summary>
    /// This sets up the two connection maps (dictionaries)
    /// </summary>
    void SetUpConnections()
    {
        connectionsRoad.Add(0, new List<int>() { 1, 2 });
        connectionsRoad.Add(1, new List<int>() { 0, 3, 4, 7 });
        connectionsRoad.Add(2, new List<int>() { 0, 4, 5, 8 });
        connectionsRoad.Add(3, new List<int>() { 1, 4, 6, 7 });
        connectionsRoad.Add(4, new List<int>() { 1, 2, 3, 5, 7, 8 });
        connectionsRoad.Add(5, new List<int>() { 2, 4, 8, 9 });
        connectionsRoad.Add(6, new List<int>() { 3, 10, 11, 16 });
        connectionsRoad.Add(7, new List<int>() { 1, 3, 4, 11, 12, 17 });
        connectionsRoad.Add(8, new List<int>() { 2, 4, 5, 12, 13, 18 });
        connectionsRoad.Add(9, new List<int>() { 5, 13, 14, 19 });
        connectionsRoad.Add(10, new List<int>() { 6, 15, 16 });
        connectionsRoad.Add(11, new List<int>() { 6, 7, 10, 12, 16, 17 });
        connectionsRoad.Add(12, new List<int>() { 7, 8, 11, 13, 17, 18 });
        connectionsRoad.Add(13, new List<int>() { 8, 9, 12, 14, 18, 19 });
        connectionsRoad.Add(14, new List<int>() { 9, 13, 19, 20 });
        connectionsRoad.Add(15, new List<int>() { 10, 21 });
        connectionsRoad.Add(16, new List<int>() { 6, 10, 11, 21, 22, 26 });
        connectionsRoad.Add(17, new List<int>() { 7, 11, 12, 22, 23, 27 });
        connectionsRoad.Add(18, new List<int>() { 8, 12, 13, 23, 24, 28 });
        connectionsRoad.Add(19, new List<int>() { 9, 13, 14, 24, 25, 29 });
        connectionsRoad.Add(20, new List<int>() { 14, 25 });
        connectionsRoad.Add(21, new List<int>() { 15, 16, 22, 26 });
        connectionsRoad.Add(22, new List<int>() { 16, 17, 21, 23, 26, 27 });
        connectionsRoad.Add(23, new List<int>() { 17, 18, 22, 24, 27, 28 });
        connectionsRoad.Add(24, new List<int>() { 18, 19, 23, 25, 28, 29 });
        connectionsRoad.Add(25, new List<int>() { 19, 20, 24, 29 });
        connectionsRoad.Add(26, new List<int>() { 21, 22, 30 });
        connectionsRoad.Add(27, new List<int>() { 17, 22, 23, 30, 31, 33 });
        connectionsRoad.Add(28, new List<int>() { 18, 23, 24, 31, 32, 34 });
        connectionsRoad.Add(29, new List<int>() { 19, 24, 25, 32 });
        connectionsRoad.Add(30, new List<int>() { 26, 27, 31, 33 });
        connectionsRoad.Add(31, new List<int>() { 27, 28, 30, 32, 33, 34 });
        connectionsRoad.Add(32, new List<int>() { 28, 29, 31, 34 });
        connectionsRoad.Add(33, new List<int>() { 27, 30, 21, 35 });
        connectionsRoad.Add(34, new List<int>() { 28, 31, 32, 35 });
        connectionsRoad.Add(35, new List<int>() { 33, 34 });

        connectionsNode.Add('a', new List<int>() { 0, 1 });
        connectionsNode.Add('b', new List<int>() { 0, 2 });
        connectionsNode.Add('c', new List<int>() { 3, 6 });
        connectionsNode.Add('d', new List<int>() { 1, 3, 4, 7 });
        connectionsNode.Add('e', new List<int>() { 2, 4, 5, 8 });
        connectionsNode.Add('f', new List<int>() { 5, 9 });
        connectionsNode.Add('g', new List<int>() { 10, 15 });
        connectionsNode.Add('h', new List<int>() { 6, 10, 11, 16 });
        connectionsNode.Add('i', new List<int>() { 7, 11, 12, 17 });
        connectionsNode.Add('j', new List<int>() { 9, 12, 13, 18 });
        connectionsNode.Add('k', new List<int>() { 9, 13, 14, 19 });
        connectionsNode.Add('l', new List<int>() { 14, 20 });
        connectionsNode.Add('m', new List<int>() { 15, 21 });
        connectionsNode.Add('n', new List<int>() { 16, 21, 22, 26 });
        connectionsNode.Add('o', new List<int>() { 17, 22, 23, 27 });
        connectionsNode.Add('p', new List<int>() { 18, 23, 24, 28 });
        connectionsNode.Add('q', new List<int>() { 19, 24, 25, 29 });
        connectionsNode.Add('r', new List<int>() { 20, 25 });
        connectionsNode.Add('s', new List<int>() { 26, 30 });
        connectionsNode.Add('t', new List<int>() { 27, 30, 31, 33 });
        connectionsNode.Add('u', new List<int>() { 28, 31, 32, 34 });
        connectionsNode.Add('v', new List<int>() { 29, 32 });
        connectionsNode.Add('w', new List<int>() { 33, 35 });
        connectionsNode.Add('x', new List<int>() { 34, 35 });
    }

    /// <summary>
    /// This is the function that is called to tell the AI to make its move
    /// </summary>
    /// <param name="currentBoard">The current board</param>
    /// <param name="AIResources">The resources held by the AI</param>
    /// <param name="playerResources">The resources held by the player</param>
    public void AIMove(Board currentBoard, List<int> AIResources, List<int> playerResources)
    {

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
    /// Index 60: Make a trade (0 = no trade; a trade is a composite of red = 1, blue = 2, green = 3, and yellow = 4
    /// where an input is viewed digit by digit with the 3 trade values followed by the traded for value)
    /// </summary>
    /// <param name="vectorAction">List of actions to take</param>
    public override void OnActionReceived(float[] vectorAction)
    {
        //see if in a draw state, skip if not the first move
        if(!firstMove && __board == __old_board && __resources == __last_resources && __player_resources == __player_last_resources)
        {
            OfferDraw();
            return;
        }

        //make a trade first
        if(vectorAction[60] != 0)
        {
            List<int> trades = new List<int>(4);
            int temp = 0;
            //split the digits apart
            temp = (int)(vectorAction[60] / 1000);
            trades[0] = temp;

        }
        
        //place nodes

        //place connectors

    }

    private void OfferDraw()
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
            connectionsNode.TryGetValue(location, out legal);
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
            connectionsRoad.TryGetValue(location, out legal);
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

    void GetPiece()
    {
        int temp = PlayerPrefs.GetInt("AI_Piece");
        __piece_type = (Owner)temp;
    }

    //[nodes, branches, trades] MakeMove()

    void GetResources(List<int> rs)
    {
        for(int i = 0; i < __resources.Count; i++)
        {
            __resources[i] += rs[i];
        }
    }

    /// <summary>
    /// This updates the stored scores for both players
    /// </summary>
    /// <param name="ai">The score of the AI</param>
    /// <param name="human">The score of the "human"</param>
    void UpdateScores(int ai, int human)
    {
        __ai_score = ai;
        __human_score = human;
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
