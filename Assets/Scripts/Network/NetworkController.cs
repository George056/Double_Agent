using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private GameObject player;

    public static List<int> nodesPlaced;
    public static List<int> branchesPlaced;
    public static string gameBoardSeed = "";

    public static NetworkController NetController;

    

    private static bool playerTurn = false;

    private void Awake()
    {
        NetController = this;
    }
  
    void Start()
    {
         player = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }

    void Update()
    {
        
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
        while (gameBoardSeed == "")
            yield return null;
        
        gameBoardSeed = "";
        boardManager.ReceiveSeedFromNetwork();
        
    }

    public void SendMove()
    {
        NetworkPlayer.networkPlayer.SendMove(nodesPlaced, branchesPlaced);
    }

    public void SendSeed()
    {
        /*NetworkPlayer.networkPlayer.SendSeed(gameBoardSeed);*/
        player.GetComponent<NetworkPlayer>().SendSeed(gameBoardSeed);
    }

    public void SetNodesPlaced(List<int> newNodesPlaced)
    {
        nodesPlaced = newNodesPlaced;
    }

    public void SetBranchesPlaced(List<int> newBranchesPlaced)
    {
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

    public List<int> GetNodesPlaced()
    {
        return nodesPlaced;
    }

    public List<int> GetBranchesPlaced()
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
