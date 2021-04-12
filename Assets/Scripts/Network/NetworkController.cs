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
    public static int[] resources = new int[] { 0, 0, 0, 0 };
    public static string gameBoardSeed = "";
 

    public static NetworkController NetController;
    




    private static bool playerTurn = false;
    private static bool playersLoaded = false;

    private void Awake()
    {
        NetController = this;
        if (GameInfo.host == true)
        {
            PhotonNetwork.Instantiate("networkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
        }
    }

    public void SetBoardManagerReference(BoardManager manager)
    {
        boardManager = manager;
    }

    public void InstantiateNetworkPlayer()
    {
        PhotonNetwork.Instantiate("networkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }


    public IEnumerator WaitForTurn()
    {
        Debug.Log("NetworkController.WaitforTurn() called");
        while (playerTurn == false)
            yield return null;

        playerTurn = false;
        boardManager.TurnReceived();

    }

    public IEnumerator WaitForSeed()
    {
        while (gameBoardSeed == "")
        {
            yield return null;  
        }

        Debug.Log("qq Wait for seed has received gameboard seed: " + gameBoardSeed);
        boardManager.ReceiveSeedFromNetwork();
        
    }

    public IEnumerator WaitForOtherPlayersLoaded()
    {
        Debug.Log("NetworkController WaitingForOtherPlayersLoaded");
        while (playersLoaded == false)
        {
            yield return null;
        }

        Debug.Log("PlayersLoaded set to true");
        boardManager.OtherPlayersHaveLoaded();
    }

    public void SendMove()
    {
        Debug.Log("NetworkController.SendMove()");
        if (resources != null)
        {
            NetworkPlayer.networkPlayer.SendMove(nodesPlaced, branchesPlaced, resources);
        }
    }

    public void SendSeed()
    {
        Debug.Log("zz Network controller send seed has been called with seed: " + gameBoardSeed);
        NetworkPlayer.networkPlayer.SendSeed(gameBoardSeed);
        
    }

    public void PlayerHasLoaded()
    {
        Debug.Log("qq Network controller player has loaded");
        bool load = true;
        NetworkPlayer.networkPlayer.PlayerHasLoaded(load);
    }

    public void SetPlayerLoaded(bool loaded)
    {
        playersLoaded = loaded;
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

        if(GameInfo.host == true)
        {
            Debug.Log("zz Network Controller set seed: " + gameBoardSeed);
        }
        else
        {
            Debug.Log("qq Network Controller set seed: " + gameBoardSeed);
        }

        /*if (GameInfo.host == true)
        {
            BoardManager.boardManager.SetupScene();
        }*/
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

    public bool GetPlayerTurn()
    {
        return playerTurn;
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

    public void SendUpdateResourcesInOpponentUI()
    {
        NetworkPlayer.networkPlayer.SetUpdateResourcesinOpponentUI(resources);
    }

    public void UpdateResourcesInOpponentUI(int[] newResources)
    {

        if (playerTurn == false && newResources.Length != 0 && BoardManager.boardManager.activeSide != BoardManager.boardManager.netPiece)
        {
            List<int> r = new List<int>(newResources);
            BoardManager.boardManager.UpdateOpponentResourcesInUI(r);
        }

    }

    /*public override void OnLeftRoom()
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
    }*/
}
