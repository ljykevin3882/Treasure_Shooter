using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public bgmController bgm;
    private AudioSource audioSource;
    void Start()
    {
        bgm.PlayBGM("LobbyBGM");
        audioSource = GameObject.Find("SoundEffect").GetComponent<AudioSource>();
        DontDestroyOnLoad(audioSource.gameObject);
        RoomManager.allPlayerIn = false;
    }
    public void StartButtonPressed()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button1"]);
        Debug.Log("start");
        SceneManager.LoadScene("Game");
    }
    public void SettingButtonPressed()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button1"]);
        GetComponent<DataSave>().LoadData();
    }
    public void AppearanceButtonPressed()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button1"]);
        SceneManager.LoadScene("Appearance");
    }
    public void ExitButtonPressed()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button2"]);
        Application.Quit();
    }
    public void MenuClosePressed()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button2"]);
        GetComponent<DataSave>().SaveData();
    }
}
