using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyStart : MonoBehaviourPunCallbacks, ILobbyCallbacks
{

    public static LobbyStart room;
    public bool intentionalDisconnect = false;
    public string roomName = "Player1";

    public GameObject CreateGameButton;
    public GameObject JoinGameButton;

    public Canvas CreateOrJoinCanvas;
    public Canvas LoadingCanvas;
    public Canvas RoomLobbyListCanvas;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private void Awake()
    {
        room = this;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        PhotonNetwork.AutomaticallySyncScene = true;

    }
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            StartCoroutine(DisconnectReconnect());
        }

        PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.AutomaticallySyncScene == false)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    void Update()
    {
        
    }

    IEnumerator DisconnectReconnect()
    {
        intentionalDisconnect = true;
        PhotonNetwork.Disconnect();

        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
    }


    public override void OnConnectedToMaster()
    {
        intentionalDisconnect = false;

        if (!CreateOrJoinCanvas.gameObject.activeSelf)
        {
            CreateOrJoinCanvas.gameObject.SetActive(true);
        }

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        CreateGameButton.SetActive(true);
        JoinGameButton.SetActive(true);
        LoadingCanvas.gameObject.SetActive(false);
    }

    public override void OnCreatedRoom()
    {
        if(PhotonNetwork.AutomaticallySyncScene == false)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        RoomOptions roomOps = new RoomOptions()
        {
            EmptyRoomTtl = 1,
            PlayerTtl = 1,
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = 2
        };

        //Need to figure out how to get a room name

        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        if (RoomLobbyListCanvas.gameObject.activeSelf)
        {
            RoomLobbyListCanvas.gameObject.SetActive(false);
        }
        if (LoadingCanvas.gameObject.activeSelf)
        {
            LoadingCanvas.gameObject.SetActive(false);
        }

        intentionalDisconnect = true;
        PhotonNetwork.Disconnect();
    }
}
