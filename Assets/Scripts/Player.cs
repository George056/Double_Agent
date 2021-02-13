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

    void Start()
    {
        __player = (short)PlayerPrefs.GetInt("Human_Player");
        __piece_type = (Owner)PlayerPrefs.GetInt("Human_Piece");
    }

    public void HumanMove(int human_score)
    {
        __human_score = human_score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
