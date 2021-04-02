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
    public void RPC_SendMove(int[] nodesPlaced, int[] branchesPlaced)
    {
        Debug.Log("NetworkPlayer.RPC_SendMove()");
        networkController.SetNodesPlaced(nodesPlaced);
        networkController.SetBranchesPlaced(branchesPlaced);
        networkController.SetPlayerTurn(true);
    }

    [PunRPC]
    public void RPC_SendSeed(string gameBoardSeed)
    {
        if (photonView.IsMine)
            return;

        networkController.SetBoardSeed(gameBoardSeed);
    }



    public void SendMove(int[] nodesPlaced, int[] branchesPlaced)
    {
        Debug.Log("NetworkPlayer.SendMove()");
        photonView.RPC("RPC_SendMove", RpcTarget.All, nodesPlaced, branchesPlaced);
    }

    public void SendSeed(string gameBoardSeed)
    {
        photonView.RPC("RPC_SendSeed", RpcTarget.All, gameBoardSeed);
    }


}
