using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class RoomButton : MonoBehaviour
{
    public Text nameText;
    public string roomName;

    public void SetRoom(string name)
    {
        nameText.text = name;
    }

    public void JoinRoomOnClick()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
