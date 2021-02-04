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
    red,
    blue,
    yellow,
    green
}

/// <summary>
/// This class is used as the AI for the game. 
/// It is constructed based on the Unity course at: https://learn.unity.com/course/ml-agents-hummingbirds
/// </summary>
public class AI : Agent
{
    [Tooltip("A list of all AI resources")]
    List<Resource> __resources;

    //nodes

    //board

    Difficulty __difficulty;
    int __ai_score;
    int __human_score;

    [Tooltip("Whether this is in training mode or not")]
    public bool trainingMode;

    /// <summary>
    /// This initializes the AI
    /// </summary>
    public override void Initialize()
    {

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

    private void Start()
    {
        __resources = new List<Resource>();
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

    void GetResources(List<Resource> rs)
    {
        foreach(Resource r in rs)
        {
            __resources.Add(r);
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
