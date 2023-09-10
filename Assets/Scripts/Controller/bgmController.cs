using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmController : MonoBehaviour
{
    GameObject BGM;
    AudioSource backmusic;
    private string NowBGMname = "";
    [System.Serializable]
    public struct BgmType
    {
        public string name;
        public AudioClip audio;
    }

    // Inspector 에표시할 배경음악 목록
    public BgmType[] BGMList;
    void Awake()
    {
        BGM = GameObject.Find("BGM");
        backmusic = BGM.GetComponent<AudioSource>(); //배경음악 저장해둠
        if (backmusic.isPlaying) return; //배경음악이 재생되고 있다면 패스
        else
        {
            PlayBGM("LobbyBGM");
            DontDestroyOnLoad(BGM); //배경음악 계속 재생하게(이후 버튼매니저에서 조작)
        }
    }
    void Update()
    {
        backmusic.volume = SoundManager.instance.volume;
    }
    public void PlayBGM(string name)
    {
        if (NowBGMname.Equals(name)) return;

        for (int i = 0; i < BGMList.Length; ++i)
            if (BGMList[i].name.Equals(name))
            {
                backmusic.clip = BGMList[i].audio;
                backmusic.Play();
                NowBGMname = name;
            }
    }
}