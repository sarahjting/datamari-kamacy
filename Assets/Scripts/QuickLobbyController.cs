using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// pun libraries
using Photon.Pun;
using Photon.Realtime;

public class QuickLobbyController : MonoBehaviourPunCallbacks // use pun's monobehaviour
{
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    // PUN CALLBACK
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.NickName = "Anonymous"; 

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = (byte)20; //Set any number

        PhotonNetwork.JoinOrCreateRoom("New Room", roomOptions, TypedLobby.Default);
    }
    
    public override void OnCreatedRoom()
    {
        PhotonNetwork.LoadLevel("GameLevel");
    }
}
