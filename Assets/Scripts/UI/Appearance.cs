using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
public class Appearance : MonoBehaviour
{
    private AudioSource audioSource;
    public Toggle[] toggles;
    private void Start()
    {
        audioSource = GameObject.Find("SoundEffect").GetComponent<AudioSource>();
        toggles = FindObjectsOfType<Toggle>().OrderBy(toggle => toggle.gameObject.name).ToArray();

        for (int i = 0; i < toggles.Length; i++)
        {
            int currentIndex = i + 1;
            toggles[i].onValueChanged.AddListener((isSelected) => PressToggle(currentIndex, isSelected));
        }
    }

    public void PressToggle(int ToggleIndex, bool isSelected)
    {
        if (isSelected)
        {
            Debug.Log(ToggleIndex + "¹ø ¼±ÅÃµÊ");
            SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button1"]);
            Player.appearance = ToggleIndex;
        }
    }
    

    public void pressBack()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button2"]);
        SceneManager.LoadScene("Lobby");
    }
    public void pressStart()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button1"]);
        SceneManager.LoadScene("Game");
    }
}
