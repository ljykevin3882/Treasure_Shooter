using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    [Space]
    public Transform spawnPoint;
    [Space]
    public string roomNameToJoin = "test";
    public static bool allPlayerIn = false;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    void Update()
    {
        if(PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            allPlayerIn = true;
    }
    public void JoinRoomButtonPressed()
    {
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
        GameManager.roomJoined = true;
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("∑Îø° ¿‘¿Â");

    }
}
