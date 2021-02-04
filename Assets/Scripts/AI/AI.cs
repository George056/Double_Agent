using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

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

    //nodes

    //board
    Board __board;

    [HideInInspector]
    public Difficulty __difficulty;
    [HideInInspector]
    int __ai_score;
    [HideInInspector]
    int __human_score;

    [Tooltip("An integer that says if the AI is orange (0) or purple (1)")]
    public short __player;

    [Tooltip("Whether this is in training mode or not")]
    public bool trainingMode;

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

    private bool LegalMoveNode(char location)
    {
        return __board.LegalMoveNode(location);
    }

    private bool LegalMoveConnector(int location)
    {
        return __board.LegalMoveConnector(location);
    }

    private void Start()
    {
        GetDifficulty();
        __ai_score = 0;
        __human_score = 0;
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

    //[nodes, branches, trades] MakeMove()

    void GetResources(List<int> rs)
    {
        for(int i = 0; i < __resources.Count; i++)
        {
            __resources[i] += rs[i];
        }
    }

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
