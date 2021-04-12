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

        if (GameInfo.host == true)
        {
            Debug.Log("zz RPC_send seed has been called, returning to board manager");
            return;
        }
        else
        {
            Debug.Log("qq RPC_send seed has been called with seed: " + gameBoardSeed);
        }
        networkController.SetBoardSeed(gameBoardSeed);
    }

    [PunRPC]
    public void RPC_UpdateResourcesinOpponentUI(int[] resources)
    {
        networkController.UpdateResourcesInOpponentUI(resources);
    }

    [PunRPC]
    public void RPC_PlayerHasLoaded(bool playerLoaded)
    {
        Debug.Log("zz qq RPC_PlayerHasLoaded called");
        networkController.SetPlayerLoaded(playerLoaded);
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
        if(GameInfo.host == true)
        {
            Debug.Log("zz Network Player send seed has been called with seed: " + gameBoardSeed);
        }
        else
        {
            Debug.Log("qq Network Player send seed has been called with seed: " + gameBoardSeed);
        }
        photonView.RPC("RPC_SendSeed", RpcTarget.All, gameBoardSeed);
    }

    public void PlayerHasLoaded(bool loaded)
    {
        Debug.Log("qq Network Player player has loaded: " + loaded);
        photonView.RPC("RPC_PlayerHasLoaded", RpcTarget.All, loaded);
    }
}
