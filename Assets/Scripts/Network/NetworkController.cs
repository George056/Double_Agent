using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private BoardManager boardManager;

    public GameObject networkPlayer;

    public static int[] nodesPlaced;
    public static int[] branchesPlaced;
    public static string gameBoardSeed = "";

    public static NetworkController NetController;
    




    private static bool playerTurn = false;

    private void Awake()
    {
        NetController = this;
        PhotonNetwork.Instantiate("networkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
        if (networkPlayer)
        {
            Debug.Log("Network Player Instantiated");
        }
        else
        {
            Debug.Log("Network Player not instantiated");
        }
    }

    public void SetBoardManagerReference(BoardManager manager)
    {
        boardManager = manager;
    }


    public IEnumerator WaitForTurn()
    {
        while (playerTurn == false)
            yield return null;


        playerTurn = false;
        boardManager.TurnReceived();

    }

    public IEnumerator WaitForSeed()
    {
        Debug.Log("WaitForSeed called");
        while (gameBoardSeed == "")
        {
            yield return null;  
        }

        Debug.Log("WaitForSeed got value: " + gameBoardSeed);
        boardManager.ReceiveSeedFromNetwork();
        
    }

    public void SendMove()
    {
        NetworkPlayer.networkPlayer.SendMove(nodesPlaced, branchesPlaced);
    }

    public void SendSeed()
    {
        Debug.Log("NetworkController sending seed: " + gameBoardSeed);
        NetworkPlayer.networkPlayer.SendSeed(gameBoardSeed);
        
    }

    public void SetNodesPlaced(int[] newNodesPlaced)
    {
        nodesPlaced = newNodesPlaced;
    }

    public void SetBranchesPlaced(int[] newBranchesPlaced)
    {
        branchesPlaced = newBranchesPlaced;
    }

    public void SetBoardSeed(string boardSeed)
    {
        if (PlayerPrefs.GetInt("Host") == 1)
        {
            Debug.Log("Host called SetBoardSeed: " + boardSeed);
        }
        else
        {
            Debug.Log("Network Player called SetBoardSeed: " + boardSeed);
        }
        gameBoardSeed = boardSeed;
    }

    public void SetPlayerTurn(bool b)
    {
        playerTurn = b;
    }

    public int[] GetNodesPlaced()
    {
        return nodesPlaced;
    }

    public int[] GetBranchesPlaced()
    {
        return branchesPlaced;
    }

    public string GetBoardSeed()
    {
        return gameBoardSeed;
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.EmptyRoomTtl = 10;
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
    }
}
