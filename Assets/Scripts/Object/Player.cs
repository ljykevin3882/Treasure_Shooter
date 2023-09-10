using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id = 0;

    //스피드 조정 변수
    [SerializeField] public float walkSpeed = 20f;
    [SerializeField] public float runSpeed = 30f;
    [SerializeField] public float applySpeed;
    [SerializeField] public float jumpForce = 20f;
    [SerializeField] public float hp = 100f;
    [SerializeField] public float maxHp = 100f;
    [SerializeField] public static int appearance=1;
    public float Hp { get { return hp; } set { hp = value; } }
    public float MaxHp { get { return maxHp; } set { maxHp = value; } }

    public float moveDirX;
    public float moveDirZ;

    //상태 변수
    public bool isWalk = false;
    public bool isRun = false;
    public bool isGround = true;

    public bool myTreasurefound = false;

    public Animator anim;
    private AudioSource playerAudioSource;
    private AudioSource gunAudioSource;

    void Start()
    {
        playerAudioSource = GetComponents<AudioSource>()[0];
        gunAudioSource = GetComponents<AudioSource>()[1];

        if(GetComponent<PlayerController>() != null)
        {
            GetComponent<PlayerController>().playerAudioSource = playerAudioSource;
            GetComponent<GunController>().gunAudioSource = gunAudioSource;
        }

        SetId();
    }
    private void SetId()
    {
        if (GameManager.isHost)
            id = 1;
        else
            id = 2;
    }
    [PunRPC]
    public void DecreaseHP(float _damage, int _shooter)
    {
        hp -= _damage;
        if(_shooter != id)
            GameManager.instance.damagePanel.GetComponent<DamagePanel>().AlphaUpdate();
        print("현재남은 HP:" + hp);
        if (hp <= 0)
        {
            anim.SetTrigger("Dead");
            //PhotonNetwork.Destroy(gameObject);
        }
    }
    [PunRPC]
    public void TreasureFoundByEnemy()
    {
        myTreasurefound = true;
    }
    [PunRPC]
    public void NoticeMonsterDead()
    {
        if(GameManager.instance.slime.GetComponent<SlimeController>().Catcher == null)
        {
            SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["MonsterKill"]);
            StartCoroutine(GameManager.instance.CoNoticeMonsterDead());
        }
    }
    [PunRPC]
    public void PlayGunSound(string _clipKey)
    {
        SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips[_clipKey]);
    }
    [PunRPC]
    public void PlayPlayerSound(string _clipKey)
    {
        SoundManager.instance.PlayClip(playerAudioSource, SoundManager.instance.audioClips[_clipKey]);
    }
}
