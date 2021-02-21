using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("An integer that says if the human is orange (0) or purple (1)")]
    public short __player;

    [Tooltip("The piece does the human play, 0 = US, 1 = USSR")]
    public Owner __piece_type;

    [HideInInspector]
    [Tooltip("The score of the human apponent")]
    public int __human_score;

    [HideInInspector]
    [Tooltip("This is true if they have the longest network")]
    public bool __longest_net;

    public string userName;

    [Tooltip("A list of all human resources with indexes 0 = red, 1 = blue, 2 = yellow, and 3 = green.")]
    public List<int> __resources = new List<int>(4) { 0, 0, 0, 0 };

    private int userNameLength = 20;

    public List<int> __owned_nodes;
    public List<int> __owned_branches;
    

    void Start()
    {
        __player = (short)PlayerPrefs.GetInt("Human_Player", 0); // default to orange
        __piece_type = (Owner)PlayerPrefs.GetInt("Human_Piece", 0); // default to US
        __longest_net = false;
        __owned_nodes = new List<int>();
        __owned_branches = new List<int>();
        UpdateResources(new List<int>(4) { 5, 5, 10, 10 });
    }

    public void HumanMove(int human_score, List<int> resources)
    {
        __human_score = human_score;
        //set the board active
    }

    /// <summary>
    /// Sets if the player has the longest network
    /// </summary>
    /// <param name="netLength">If the player has the longest network</param>
    public void SetLongest(bool netLength)
    {
        __longest_net = netLength;
    }

    public void Loss()
    {

    }

    public bool CanAffordNode()
    {
        bool canAfford = true;

        if (__resources[2] < 2) { canAfford = false; }
        if (__resources[3] < 2) { canAfford = false; }

        return canAfford;
    }

    public bool CanAffordBranch()
    {
        bool canAfford = true;

        if (__resources[0] < 1) { canAfford = false; }
        if (__resources[1] < 1) { canAfford = false; }

        return canAfford;
    }

    public void PayForNode()
    {
        __resources[2] -= 2;
        __resources[3] -= 2;
    }

    public void PayForBranch()
    {
        __resources[0]--;
        __resources[1]--;
    }

    public void UpdateResources(List<int> update)
    {
        for (int i = 0; i < __resources.Count; i++)
        {
            __resources[i] += update[i];
        }
    }

    public void UpdateScore(int newScore)
    {
        __human_score = newScore;
    }

    public void AddNode(int index)
    {
        __owned_nodes.Add(index);
    }

    public void AddBranch(int index)
    {
        __owned_branches.Add(index);
    }

}
