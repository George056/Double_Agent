using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

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
    public GameObject lobbyListBackButton;

    public Transform lobbyPanel;

    public Canvas CreateOrJoinCanvas;
    public Canvas LoadingCanvas;
    public Canvas RoomLobbyListCanvas;
    public Canvas WaitingForPlayerCanvas;
    public Canvas UnintentionalDisconnectCanvas;

    private IEnumerator coroutine;
    public TextMeshProUGUI roomNameText;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;

    public AudioSource mainMusic;

    private IEnumerator playLobbyMusic(float delay)
    {
        yield return new WaitForSeconds(delay);
        mainMusic.Play(0);
    }

    private void Awake()
    {
        room = this;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        PhotonNetwork.AutomaticallySyncScene = true;
        LoadingCanvas.gameObject.SetActive(true);

        coroutine = playLobbyMusic(8.5f);
        StartCoroutine(coroutine);
    }
    void Start()
    {
   /*     if (PhotonNetwork.IsConnected)
        {
            StartCoroutine(DisconnectReconnect());
        }*/

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
        Debug.Log("OnJoinedLobby");
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

        Debug.Log("OnCreateRoomfailed");
        RoomOptions roomOps = new RoomOptions()
        {
            EmptyRoomTtl = 1,
            PlayerTtl = 1,
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = 2
        };

        roomName = GameInfo.user_name + Random.Range(0, 100);
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("OnJoinRoomfailed");
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

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        waitingForPlayerText.gameObject.SetActive(false);
        waitingForHostText.gameObject.SetActive(false);
        playerLeftText.gameObject.SetActive(false);
        hostLeftText.gameObject.SetActive(false);
        WaitingForPlayerCanvas.gameObject.SetActive(true);

        Debug.Log("OnJoinedRoom");
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
        Debug.Log("ClearRoomListView");
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }
        roomListEntries.Clear();
    }

    private void UpdateRoomListView()
    {
        Debug.Log("UpdateRoomListView1");
        foreach(RoomInfo item in cachedRoomList.Values)
        {
            Debug.Log("UpdateRoomListView2");
            ListRoom(item);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        Debug.Log("UpdateCachedRoomList1");
        foreach (RoomInfo roomInfo in roomList)
        {
            Debug.Log("UpdateCachedRoomList2");
            if (!roomInfo.IsOpen || !roomInfo.IsVisible || roomInfo.RemovedFromList)
            {
                Debug.Log("UpdateCachedRoomList3");
                if (cachedRoomList.ContainsKey(roomInfo.Name))
                {
                    cachedRoomList.Remove(roomInfo.Name);
                }
                Debug.Log("UpdateCachedRoomList4");
                continue;
            }

            if (cachedRoomList.ContainsKey(roomInfo.Name))
            {
                Debug.Log("UpdateCachedRoomList5");
                cachedRoomList[roomInfo.Name] = roomInfo;
            }
            else
            {
                Debug.Log("UpdateCachedRoomList6");
                cachedRoomList.Add(roomInfo.Name, roomInfo);
            }
        }
    }
    public void ListRoom(RoomInfo room)
    {
        Debug.Log("ListRoom");
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
        Debug.Log("OnPlayerEnteredRoom");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
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
        Debug.Log("OnPlayerLeftRoom");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
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
        Debug.Log("CreateRoom");
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
        CreateOrJoinCanvas.gameObject.SetActive(false);
        waitingForHostText.gameObject.SetActive(false);
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

        startButton.gameObject.SetActive(false);
        intentionalDisconnect = true;
        PhotonNetwork.Disconnect();
    }

    public void onUnintentionalDisconnectBackButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void onLobbyListBackButtonClicked()
    {
        RoomLobbyListCanvas.gameObject.SetActive(false);
        CreateOrJoinCanvas.gameObject.SetActive(true);
    }
}
