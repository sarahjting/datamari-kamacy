using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class RoomController : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform spawnPoint;

    public GameObject serverNameText;
    public GameObject leaveServerButton;
    public GameObject playerList;


    // Start is called before the first frame update
    void Start()
    {
        //In case we started this demo with the wrong scene being active, simply load the menu scene
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Is not in the room, returning back to Lobby");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby");
            return;
        }

        //We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, 0);

        serverNameText.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.CurrentRoom.Name;
        leaveServerButton.GetComponent<Button>().onClick.AddListener(() => {
            PhotonNetwork.LeaveRoom();
        });
    }

    void OnGUI()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        //Show the list of the players connected to this Room
        string playerNames = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //Show if this player is a Master Client. There can only be one Master Client per Room so use this to define the authoritative logic etc.)
            playerNames += PhotonNetwork.PlayerList[i].NickName + "\n";
        }
        playerList.GetComponent<TextMeshProUGUI>().text = playerNames;
    }

    public override void OnLeftRoom()
    {
        //We have left the Room, return back to the GameLobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
