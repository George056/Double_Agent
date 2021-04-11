using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LobbyStart : MonoBehaviourPunCallbacks, ILobbyCallbacks
{

    public static LobbyStart room;
    public bool intentionalDisconnect = false;
    public string roomName;

    public GameObject CreateGameButton;
    public GameObject JoinGameButton;
    public GameObject createOrJoinGameBackButton;
    public GameObject unintentionalDisconnectBackButton;
    public GameObject roomListing;
    public GameObject startButton;
    public GameObject waitingForPlayerText;
    public GameObject waitingForHostText;
    public GameObject playerLeftText;
    public GameObject hostLeftText;
    public GameObject waitingForPlayerBackButton;

    public Transform lobbyPanel;

    public Canvas CreateOrJoinCanvas;
    public Canvas LoadingCanvas;
    public Canvas RoomLobbyListCanvas;
    public Canvas WaitingForPlayerCanvas;
    public Canvas UnintentionalDisconnectCanvas;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private void Awake()
    {
        room = this;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        PhotonNetwork.AutomaticallySyncScene = true;
        LoadingCanvas.gameObject.SetActive(true);

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
        if(PhotonNetwork.NetworkClientState == ClientState.Disconnected && intentionalDisconnect == false)
        {
            CreateOrJoinCanvas.gameObject.SetActive(false);
            LoadingCanvas.gameObject.SetActive(false);
            RoomLobbyListCanvas.gameObject.SetActive(false);
            WaitingForPlayerCanvas.gameObject.SetActive(false);

            UnintentionalDisconnectCanvas.gameObject.SetActive(true);
        }
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
        Debug.Log("Connected to master");
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

        roomName = GameInfo.user_name;
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

    public override void OnJoinedRoom()
    {
        CreateOrJoinCanvas.gameObject.SetActive(false);
        RoomLobbyListCanvas.gameObject.SetActive(false);

        WaitingForPlayerCanvas.gameObject.SetActive(true);

        if (GameInfo.host == false)
        {
            waitingForHostText.gameObject.SetActive(true);
        }
        else if(GameInfo.host == true)
        {
            waitingForPlayerText.gameObject.SetActive(true);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (intentionalDisconnect == true)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }
        roomListEntries.Clear();
    }

    private void UpdateRoomListView()
    {
        foreach(RoomInfo item in cachedRoomList.Values)
        {
            ListRoom(item);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            if(!roomInfo.IsOpen || !roomInfo.IsVisible || roomInfo.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(roomInfo.Name))
                {
                    cachedRoomList.Remove(roomInfo.Name);
                }
                continue;
            }

            if (cachedRoomList.ContainsKey(roomInfo.Name))
            {
                cachedRoomList[roomInfo.Name] = roomInfo;
            }
            else
            {
                cachedRoomList.Add(roomInfo.Name, roomInfo);
            }
        }
    }
    public void ListRoom(RoomInfo room)
    {

        if (room.IsOpen && room.IsVisible)
        {
            GameObject tempListing = Instantiate(roomListing, lobbyPanel);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.roomName = room.Name;
            Debug.Log("RoomName: " + room.Name);
            tempButton.SetRoom();

            roomListEntries.Add(room.Name, tempListing);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                waitingForPlayerText.gameObject.SetActive(false);
                playerLeftText.gameObject.SetActive(false);
                startButton.SetActive(true);
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
       if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            if(GameInfo.host == false)
            {
                waitingForHostText.gameObject.SetActive(false);
                hostLeftText.gameObject.SetActive(true);
            }
            else
            {
                startButton.gameObject.SetActive(false);
                playerLeftText.gameObject.SetActive(true);
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
        }
    }

    public override void OnLeftRoom()
    {
        LoadingCanvas.gameObject.SetActive(false);
        RoomLobbyListCanvas.gameObject.SetActive(false);
        WaitingForPlayerCanvas.gameObject.SetActive(false);
        CreateOrJoinCanvas.gameObject.SetActive(true);

        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        PhotonNetwork.Disconnect();
    }

    public void CreateRoom()
    {
        RoomOptions roomOps = new RoomOptions()
        {
            EmptyRoomTtl = 1,
            PlayerTtl = 1,
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = 2
        };

        roomName = GameInfo.user_name;
        Debug.Log("Create Room roomName: " + roomName);
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public void RemoveRoomListings()
    {
        while (lobbyPanel.childCount != 0)
        {
            Destroy(lobbyPanel.GetChild(0).gameObject);
        }
    }

    public void OnCreateGameButtonClicked()
    {        
        GameInfo.host = true;
        CreateRoom();

    }
    public void OnJoinGameButtonClicked()
    {
        GameInfo.host = false;
        CreateOrJoinCanvas.gameObject.SetActive(false);
        RoomLobbyListCanvas.gameObject.SetActive(true);

        lobbyPanel.gameObject.SetActive(true);
    }

    public void OnStartButtonClicked()
    {
        PhotonNetwork.LoadLevel("PVP");
    }

    public void onCreateOrJoinBackButtonClicked()
    {
        intentionalDisconnect = true;
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenuScene");
    }
    
    public void onWaitingForPlayersBackButtonClicked()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;

            PhotonNetwork.LeaveRoom(); 
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }

        intentionalDisconnect = true;
        PhotonNetwork.Disconnect();
    }

    public void onUnintentionalDisconnectBackButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
