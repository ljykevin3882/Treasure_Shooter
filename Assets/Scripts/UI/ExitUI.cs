using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitUI : MonoBehaviour
{
    [SerializeField] private GameObject checkWindow;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GameObject.Find("SoundEffect").GetComponent<AudioSource>();
    }
    public void LobbyButtonPressed()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button1"]);
        checkWindow.SetActive(true);
    }
    public void ExitButtonPressed()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button2"]);
        checkWindow.SetActive(false);
        gameObject.SetActive(false);

        GameManager.instance.InitFlag();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        RoomList.Instance = null;
        SceneManager.LoadScene("Lobby");
    }
    public void CancelCheckButtonPressed()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button1"]);
        checkWindow.SetActive(false);
    }
}
