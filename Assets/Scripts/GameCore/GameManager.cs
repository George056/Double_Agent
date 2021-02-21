using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BoardManager boardScript;
    public GameObject player;

    // Start is called before the first frame update
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
