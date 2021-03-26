using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private BoardManager boardManager;

    public static List<int> nodesPlaced;
    public static List<int> branchesPlaced;

    public static NetworkController NetController;

    private static bool playerTurn = false;

    private void Awake()
    {
        NetController = this;
    }
  
    void Start()
    {
        GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
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

    }


    public void SetNodesPlaced(List<int> newNodesPlaced)
    {
        nodesPlaced = newNodesPlaced;
    }

    public void SetBranchesPlaced(List<int> newBranchesPlaced)
    {
        branchesPlaced = newBranchesPlaced;
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
