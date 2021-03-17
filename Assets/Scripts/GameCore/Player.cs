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

        // Allocate resources to allow player to place first node and branch
        UpdateResources(new List<int>(4) { 1, 1, 2, 2 });
    }

    public void GetLongestNet()
    {
        __longest_net = true;
    }

    public void LoseLongestNet()
    {
        __longest_net = false;
        int lowerScore = __human_score - 2;
        UpdateScore(lowerScore, false, false);
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

        GameObject.FindObjectOfType<BoardManager>().UpdatePlayerResourcesInUI(__resources);
    }

    public void ReimburseForNode()
    {
        __resources[2] += 2;
        __resources[3] += 2;

        GameObject.FindObjectOfType<BoardManager>().UpdatePlayerResourcesInUI(__resources);
    }

    public void PayForBranch()
    {
        __resources[0]--;
        __resources[1]--;

        GameObject.FindObjectOfType<BoardManager>().UpdatePlayerResourcesInUI(__resources);
    }

    public void ReimburseForBranch()
    {
        __resources[0]++;
        __resources[1]++;

        GameObject.FindObjectOfType<BoardManager>().UpdatePlayerResourcesInUI(__resources);
    }

    public void UpdateResources(List<int> update)
    {
        Debug.Log("Earned Resources: " + update[0] + ", " + update[1] + ", " + update[2] + ", " + update[3]);
        for (int i = 0; i < __resources.Count; i++)
        {
            __resources[i] += update[i];
        }

        GameObject.FindObjectOfType<BoardManager>().UpdatePlayerResourcesInUI(__resources);
    }

    public void UpdateScore(int newScore, bool temp1, bool temp2)
    {
        __human_score = newScore;
        GameObject.FindObjectOfType<BoardManager>().UpdatePlayerScoreInUI(__human_score);
    }

    public void AddNode(int index)
    {
        __owned_nodes.Add(index);
    }

    public void RemoveNode(int index)
    {
        __owned_nodes.Remove(index);
    }

    public void AddBranch(int index)
    {
        __owned_branches.Add(index);
    }

    public void RemoveBranch(int index)
    {
        __owned_branches.Remove(index);
    }
}
