using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardScript;
    public GameObject player;

    [HideInInspector]
    [Tooltip("When set true the gameplay loop ends")]
    public bool gameOver = false;

    [Tooltip("What piece is the AI")]
    private Owner aiPiece;
    [Tooltip("What piece is the human")]
    private Owner humanPiece;
    [Tooltip("What piece is the first player")]
    private Owner firstPlayer;

    private GameObject player1;
    private GameObject player2;

    private int turnCount = 1;

    void Awake()
    {
        boardScript = GetComponent<BoardManager>();
        if(!Relationships.built)
            Relationships.SetUpConnections();
        Instantiate(player);
        InitGame();
    }
    void InitGame()
    {
        boardScript.SetupScene();
    }

    //private void Start()
    //{
    //    aiPiece = (Owner)PlayerPrefs.GetInt("AI_Piece", 1);
    //    humanPiece = (Owner)PlayerPrefs.GetInt("Human_Piece", 0);
    //    firstPlayer = (PlayerPrefs.GetInt("AI_Player", 1) == 0) ? aiPiece : humanPiece;

    //    //check to see if it is an AI or network game
    //    player1 = GameObject.FindGameObjectWithTag("Player");
    //    player2 = GameObject.FindGameObjectWithTag("AI");
    //    player2.GetComponent<AI>().SetOpener();

    //    //find turn order and set up
    //    if (firstPlayer == aiPiece)
    //    {
    //        player2.GetComponent<AI>().AIMove(turnCount);
    //        //boardScript.EndTurn();
    //        //human
    //        //human
    //        player2.GetComponent<AI>().AIMove(turnCount);
    //        //boardScript.EndTurn();
    //    }
    //    else
    //    {
    //        //human
    //        player2.GetComponent<AI>().AIMove(turnCount);
    //        player2.GetComponent<AI>().AIMove(turnCount);
    //        //boardScript.EndTurn();
    //        //human
    //    }

    //    //opener set up


    //    //normal turn gameplay
    //    while (!gameOver)
    //    {

    //        turnCount++;
    //    }
    //}
}
