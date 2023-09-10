using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    [SerializeField] private Slider volumeBar;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Toggle bgmToggle;

    private AudioSource bgmSource;

    void Start()
    {
        if (SoundManager.instance != null)
            volumeBar.value = SoundManager.instance.volume;

        bgmSource = GameObject.Find("BGM").GetComponent<AudioSource>();

        bgmToggle.isOn = !bgmSource.mute;
    }
    public void SetVolume()
    {
        SoundManager.instance.volume = volumeBar.value;
    }
    public void UpdateValueText()
    {
        valueText.text = (Mathf.Floor(volumeBar.value * 10000f) / 100f).ToString();
    }
    public void BgmOnOff()
    {
        bgmSource.mute = !bgmToggle.isOn;
    }
}