using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    #region Singleton
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    [SerializeField] private AudioClip[] clips;
    public float volume = 1f;

    void Start()
    {
        AddClip();
    }
    private void AddClip()
    {
        audioClips.Add("Fire", clips[0]);
        audioClips.Add("Walk", clips[1]);
        audioClips.Add("MonsterKill", clips[2]);
        audioClips.Add("Win", clips[3]);
        audioClips.Add("Lose", clips[4]);
        audioClips.Add("Button1", clips[5]);
        audioClips.Add("Button2", clips[6]);
        audioClips.Add("Damage", clips[7]);
        audioClips.Add("Reload", clips[8]);
        audioClips.Add("Run", clips[9]);
    }
    public void PlayClip(AudioSource _audioSource, AudioClip _audioClip)
    {
        _audioSource.clip = _audioClip;
        _audioSource.volume = volume;
        Debug.Log("Volume: " + _audioSource.volume);
        _audioSource.Play();
    }
}
