using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardScript;
    public GameObject player;

    [HideInInspector]
    [Tooltip("When set true the game-play loop ends")]
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
}
