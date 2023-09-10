using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public float sensitivity;
    public float volume;
    public bool bgmOn;
}
public class DataSave : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SettingValue.txt";

    private AudioSource bgmSource;

    void Start()
    {
        SAVE_DATA_DIRECTORY = "C:/TreasureShooterData";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY))
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);

        bgmSource = GameObject.Find("BGM").GetComponent<AudioSource>();

        LoadData();
    }
    public void SaveData()
    {
        if (GameManager.instance != null)
            saveData.sensitivity = GameManager.instance.mySensitivity;
        if (GetComponent<Lobby>() != null)
            saveData.sensitivity = GetComponent<Lobby>().sensitivitySlider.value * 100;

        saveData.volume = SoundManager.instance.volume;
        saveData.bgmOn = !bgmSource.mute;

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("Save complete");
        Debug.Log(json);
    }
    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            if(GameManager.instance != null)
                GameManager.instance.mySensitivity = saveData.sensitivity;
            if(GetComponent<Lobby>() != null)
                GetComponent<Lobby>().sensitivitySlider.value = saveData.sensitivity / 100;

            SoundManager.instance.volume = saveData.volume;
            bgmSource.mute = !saveData.bgmOn;

            Debug.Log("Load Complete");
        }
        else
        {
            if (GameManager.instance != null)
                GameManager.instance.mySensitivity = 10f;
            if (GetComponent<Lobby>() != null)
                GetComponent<Lobby>().sensitivitySlider.value = 0.1f;

            SoundManager.instance.volume = 0.5f;
            Debug.Log("No load data");
        }
    }
}