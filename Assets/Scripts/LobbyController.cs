using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI libraries
using UnityEngine.UI;
using TMPro;

// pun libraries
using Photon.Pun;
using Photon.Realtime;

public class LobbyController : MonoBehaviourPunCallbacks // <-- inherit from pun's monobehaviour so we can use pun's hooks
{
    //The list of created rooms
    List<RoomInfo> createdRooms = new List<RoomInfo>();

    Vector2 roomListScroll = Vector2.zero;
    bool joiningRoom = false;

    public GameObject statusText;
    public GameObject playerNameInput;
    public GameObject createServerInput;
    public GameObject createServerButton;
    public GameObject joinServerList;

    // Start is called before the first frame update
    void Start()
    {
        //This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();

        createServerButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            joiningRoom = true;

            PhotonNetwork.NickName = GetNickName();

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = (byte)20; //Set any number

            String roomName = createServerInput.GetComponent<TMP_InputField>().text;
            if (roomName == "") roomName = "New Room";

            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        });

    }

    // PUN CALLBACKS
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        createdRooms = roomList;
        foreach (Transform child in joinServerList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // room name & player name
        if (createdRooms.Count == 0)
        {
            GameObject listItem = Instantiate(Resources.Load<GameObject>("RoomListItem"));
            listItem.transform.SetParent(joinServerList.transform);
            listItem.transform.localScale = new Vector3(1f, 1f, 1f);
            listItem.transform.position = new Vector3(listItem.transform.position.x, listItem.transform.position.y, 1);

            listItem.transform.Find("ServerName").GetComponent<TextMeshProUGUI>().text = "No rooms available.";
            Destroy(listItem.transform.Find("ServerCapacity").gameObject);
            Destroy(listItem.transform.Find("ServerIcon").gameObject);
        }
        else
        {
            for (int i = 0; i < createdRooms.Count; i++)
            {
                GameObject listItem = Instantiate(Resources.Load<GameObject>("RoomListItem"));
                listItem.transform.SetParent(joinServerList.transform);
                listItem.transform.localScale = new Vector3(1f, 1f, 1f);
                listItem.transform.position = new Vector3(listItem.transform.position.x, listItem.transform.position.y, 1);

                listItem.transform.Find("ServerName").GetComponent<TextMeshProUGUI>().text = createdRooms[i].Name;
                listItem.transform.Find("ServerCapacity").GetComponent<TextMeshProUGUI>().text = createdRooms[i].PlayerCount + "/" + createdRooms[i].MaxPlayers + " Players";
                int currentIndex = i;
                listItem.GetComponent<Button>().onClick.AddListener(() =>
                {
                    joiningRoom = true;
                    PhotonNetwork.NickName = GetNickName();
                    PhotonNetwork.JoinRoom(createdRooms[currentIndex].Name);
                });
            }
        }
    }

    // ROOM CREATION
    void OnGUI()
    {
        // set status
        String status = "Connecting";
        switch (PhotonNetwork.NetworkClientState)
        {
            case ClientState.JoinedLobby:
                status = "Connected";
                break;
            case ClientState.Disconnected:
                status = "Disconnected";
                break;
        }
        foreach (Transform eachChild in statusText.transform)
        {
            if (eachChild.name == status || eachChild.name == "Title") eachChild.gameObject.SetActive(true);
            else eachChild.gameObject.SetActive(false);
        }

        // disable GUI if we're not connected
        if ((PhotonNetwork.NetworkClientState == ClientState.JoinedLobby || PhotonNetwork.NetworkClientState == ClientState.Disconnected) && !joiningRoom)
        {
            gameObject.transform.Find("Panel").GetComponent<CanvasGroup>().interactable = true;
            gameObject.transform.Find("Panel").GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            gameObject.transform.Find("Panel").GetComponent<CanvasGroup>().interactable = false;
            gameObject.transform.Find("Panel").GetComponent<CanvasGroup>().alpha = 0.8f;
        }

    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
        joiningRoom = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
        joiningRoom = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed got called. This can happen if the room is not existing or full or closed.");
        joiningRoom = false;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        PhotonNetwork.NickName = GetNickName();
        PhotonNetwork.LoadLevel("GameLevel");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    private String GetNickName()
    {
        String playerName = playerNameInput.GetComponent<TMP_InputField>().text;
        if (playerName == "") playerName = "Anonymous";
        return playerName;
    }
}
