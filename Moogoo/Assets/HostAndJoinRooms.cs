using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class HostAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomCodeInputField;

    public void HostRoom()
    {
        if (roomCodeInputField.text == "")
        {
            PhotonNetwork.CreateRoom("DEFAULT");
            return;
        }

        PhotonNetwork.CreateRoom(roomCodeInputField.text);
    }

    public void JoinRoom()
    {
        if (roomCodeInputField.text == "")
        {
            PhotonNetwork.JoinRoom("DEFAULT");
            return;
        }

        PhotonNetwork.JoinRoom(roomCodeInputField.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}
