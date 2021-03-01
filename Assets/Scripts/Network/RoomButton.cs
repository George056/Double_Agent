using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class RoomButton : MonoBehaviour
{
    public Text nameText;
    public string roomName;

    public void SetRoom()
    {
        nameText.text = roomName;
    }

    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
