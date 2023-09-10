using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Result : MonoBehaviour
{
    public void ToLobby()
    {
        gameObject.SetActive(false);
        GameManager.instance.InitFlag();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        RoomList.Instance = null;
        SceneManager.LoadScene("Lobby");
    }
    public void QuitGame()
    {
        gameObject.SetActive(false);
        GameManager.instance.InitFlag();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
}
