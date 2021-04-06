using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class RoomButton : MonoBehaviour
{
    public Text nameText;
    public string roomName;

    public void SetRoom()
    {
        nameText.text = PlayerPrefs.GetString("UserName");
    }

    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
