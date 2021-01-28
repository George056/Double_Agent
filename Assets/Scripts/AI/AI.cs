using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class AI : MonoBehaviour
{
    //resources
    List<Resource> __resources;

    //nodes

    //board

    Difficulty __difficulty;
    int __ai_score;
    int __human_score;

    AI(Difficulty d, int ai_score, int human_score)
    {
        __resources = new List<Resource>();
        __difficulty = d;
        __ai_score = ai_score;
        __human_score = human_score;
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
