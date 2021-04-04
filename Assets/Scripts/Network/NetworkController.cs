﻿using Photon.Pun;
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
    public static int[] resources;
    public static string gameBoardSeed = "";
 

    public static NetworkController NetController;
    




    private static bool playerTurn = false;

    private void Awake()
    {
        NetController = this;
        PhotonNetwork.Instantiate("networkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }

    public void SetBoardManagerReference(BoardManager manager)
    {
        boardManager = manager;
    }


    public IEnumerator WaitForTurn()
    {
        Debug.Log("NetworkController.WaitforTurn() called");
        while (playerTurn == false)
            yield return null;


        playerTurn = false;
        Debug.Log("NetworkController.WaitforTurn() playerTurn has been set to turn");
        boardManager.TurnReceived();

    }

    public IEnumerator WaitForSeed()
    {
        while (gameBoardSeed == "")
        {
            yield return null;  
        }

        boardManager.ReceiveSeedFromNetwork();
        
    }

    public void SendMove()
    {
        Debug.Log("NetworkController.SendMove()");
        NetworkPlayer.networkPlayer.SendMove(nodesPlaced, branchesPlaced, resources);
    }

    public void SendSeed()
    {
        NetworkPlayer.networkPlayer.SendSeed(gameBoardSeed);
        
    }

    public void SetNodesPlaced(int[] newNodesPlaced)
    {
        Debug.Log("SetNodesPlacedCalled");
        nodesPlaced = newNodesPlaced;
    }

    public void SetBranchesPlaced(int[] newBranchesPlaced)
    {
        Debug.Log("SetBranchesPlacedCalled");
        branchesPlaced = newBranchesPlaced;
    }

    public void SetBoardSeed(string boardSeed)
    {
        gameBoardSeed = boardSeed;
    }

    public void SetPlayerTurn(bool b)
    {
        playerTurn = b;
    }

    public void SetResources(int[] newResources)
    {
        resources = newResources;
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

    public int[] GetResources()
    {
        return resources;
    }

    public void ClearBranchesandNodesandResources()
    {
        if (branchesPlaced != null && branchesPlaced.Length > 0 )
        {
            System.Array.Clear(branchesPlaced, 0, branchesPlaced.Length);
        }

        if (nodesPlaced != null && nodesPlaced.Length > 0)
        {
            System.Array.Clear(nodesPlaced, 0, nodesPlaced.Length);
        }
        if(resources != null && resources.Length > 0)
        {
            System.Array.Clear(resources, 0, resources.Length);
        }

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
