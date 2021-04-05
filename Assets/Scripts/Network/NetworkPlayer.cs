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
    public void RPC_SendMove(int[] nodesPlaced, int[] branchesPlaced, int[] resources)
    {
        Debug.Log("NetworkPlayer.RPC_SendMove()");
        networkController.SetNodesPlaced(nodesPlaced);
        networkController.SetBranchesPlaced(branchesPlaced);
        networkController.SetResources(resources);
        networkController.SetPlayerTurn(true);
    }

    [PunRPC]
    public void RPC_SendSeed(string gameBoardSeed)
    {
        if (photonView.IsMine)
            return;

        networkController.SetBoardSeed(gameBoardSeed);
    }

    [PunRPC]
    public void RPC_UpdateResourcesinOpponentUI(int[] resources)
    {
        networkController.SetUpdateResourcesInOpponentUI(resources);
    }

    public void SendMove(int[] nodesPlaced, int[] branchesPlaced, int[] resources)
    {
        Debug.Log("NetworkPlayer.SendMove()");
        photonView.RPC("RPC_SendMove", RpcTarget.All, nodesPlaced, branchesPlaced, resources);
    }

    public void SetUpdateResourcesinOpponentUI(int[] resources)
    {
        photonView.RPC("RPC_UpdateResourcesinOpponentUI", RpcTarget.All, resources);
    }
    public void SendSeed(string gameBoardSeed)
    {
        photonView.RPC("RPC_SendSeed", RpcTarget.All, gameBoardSeed);
    }


}
