using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private NetworkController networkController;
    [SerializeField]
    private PhotonView photonView;

    public static NetworkPlayer networkPlayer;
    
    private void Awake()
    {
        Debug.Log("NetworkPlayerAwake Function");
        photonView = PhotonView.Get(this);
        networkPlayer = this;
    }


    [PunRPC]
    public void RPC_SendMove(List<int> nodesPlaced, List<int> branchesPlaced)
    {

        networkController.SetNodesPlaced(nodesPlaced);
        networkController.SetBranchesPlaced(branchesPlaced);
        networkController.SetPlayerTurn(true);
    }

    [PunRPC]
    public void RPC_SendSeed(string gameBoardSeed)
    {
        Debug.Log("NetworkPlayer.RPC_SendSeed and gameboard = " + gameBoardSeed);
        if (photonView.IsMine)
            return;

        Debug.Log("NetworkPlayer.RPC_SendSeed after photonView");
        networkController.SetBoardSeed(gameBoardSeed);
    }



    public void SendMove(List<int> nodesPlaced, List<int> branchesPlaced)
    {
        photonView.RPC("RPC_SendMove", RpcTarget.All, nodesPlaced, branchesPlaced);
    }

    public void SendSeed(string gameBoardSeed)
    {
        Debug.Log("NetworkPlayer.SendSee()");
        photonView.RPC("RPC_SendSeed", RpcTarget.All, gameBoardSeed);
    }


}
