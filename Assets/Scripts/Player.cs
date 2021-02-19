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

    private int userNameLength = 20;

    [Tooltip("A list of all human resources with indexes 0 = red, 1 = blue, 2 = yellow, and 3 = green.")]
    List<int> __resources = new List<int>(4) { 0, 0, 0, 0 };

    List<int> __owned_nodes;

    void Start()
    {
        __player = (short)PlayerPrefs.GetInt("Human_Player");
        __piece_type = (Owner)PlayerPrefs.GetInt("Human_Piece");
        __longest_net = false;
        __owned_nodes = new List<int>();
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
}
