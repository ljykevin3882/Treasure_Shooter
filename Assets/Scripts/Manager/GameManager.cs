using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
//using UnityEditor.SearchService;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    #region Singleton
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        //DontDestroyOnLoad(gameObject);
    }
    #endregion

    public static bool roomJoined = false;
    public static bool treasureSettingEnd = false;
    public static bool isHost = false;
    public static bool gameEnd = false;
    public static int gameResult = 0; //1: win, 2: lose
    public static int playerCount = 0;
    public GameObject[] _players,_tcs;
    public bool textOn = false;
    private bool tcSpawned = false;
    private bool treasureChecked = false;
    private bool playerChecked = false;
    public bool menuOn = false;
    public float mySensitivity;
    private bool resultSoundPlayed = false;

    [SerializeField] private GameObject treasureCam;

    //[SerializeField] private GameObject[] myPlayerPrefab;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private GameObject bulletStatusUI;
    [SerializeField] private GameObject hpBarUI;
    [SerializeField] private GameObject CrosshairUI;
    [SerializeField] private GameObject WinUI;
    [SerializeField] private GameObject LoseUI;
    [SerializeField] private GameObject MenuUI;
    public TextMeshProUGUI treasureText;
    public Image damagePanel;

    public GameObject enemyTreasure;
    private AudioSource audioSource;
    [SerializeField] private Transform[] spawnLocation;
    [SerializeField] private Transform camSpawnLocation;
    [SerializeField] private Transform slimeSpawnLocation;
    [SerializeField] private Transform canvas;

    public GameObject myPlayer;
    public GameObject enemyPlayer;
    public GameObject slime;

    private GameObject tc;

    private void Start()
    {
        audioSource = GameObject.Find("SoundEffect").GetComponent<AudioSource>();
    }
    void Update()
    {
        if (roomJoined && !tcSpawned)
        {
            gameEnd = false;
            tcSpawned = true;
            tc = Instantiate(treasureCam, camSpawnLocation);
            SetCursorInvisible();
        }

        CheckAllTreasureSpawned();
        _players = GameObject.FindGameObjectsWithTag("Player");
        if (treasureSettingEnd)
        {
            treasureSettingEnd = false;
            StartCoroutine(CoSetWaitText());
            
            Invoke("StartInstantiate", 5f);
        }

        if (!playerChecked)
            SetLocalOrEnemy();
        else
            SetGameResult();

        GameResultCheck();

        if (Input.GetKeyDown(KeyCode.Escape) && !MenuUI.activeSelf) //메뉴창 열기
            OpenMenu();
    }
    public void Button1PressSound()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button1"]);
    }
    public void Button2PressSound()
    {
        SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Button2"]);
    }

    public void InitFlag()
    {
        roomJoined = false;
        treasureSettingEnd = false;
        isHost = false;
        gameResult = 0;
        tcSpawned = false;
        treasureChecked = false;
        playerChecked = false;
        //RoomManager.allPlayerIn = false;
    }
    private IEnumerator CoSetWaitText()
    {
        treasureText.gameObject.SetActive(true);
        int waitTime = 5;
        while(waitTime > 0)
        {
            treasureText.text = "게임이 " + waitTime.ToString() + " 초 후에 시작됩니다.";
            yield return new WaitForSeconds(1f);
            waitTime -= 1;
        }
    }
    private void StartInstantiate()
    {
        treasureText.gameObject.SetActive(false);
        if (isHost)
            slime = PhotonNetwork.Instantiate(slimePrefab.name, slimeSpawnLocation.position, Quaternion.identity);
        GameObject bs = Instantiate(bulletStatusUI, canvas);
        GameObject hb = Instantiate(hpBarUI, canvas);
        GameObject ch = Instantiate(CrosshairUI, canvas);
        GameObject player = PhotonNetwork.Instantiate("Player "+Player.appearance, spawnLocation[0].position, Quaternion.identity);

        player.GetComponent<PlayerSetup>().IsLocalPlayer();
        tc.GetComponent<TreasureSettingController>().tr.GetComponent<TreasureController>().Owner = player.GetComponent<Player>();
        Destroy(tc);
        player.GetComponent<PlayerController>().Cross = ch.GetComponent<Crosshair>();
        player.GetComponent<GunController>().Cross = ch.GetComponent<Crosshair>();
        bs.GetComponent<BulletStatus>().PlayerController = player.GetComponent<PlayerController>();
        hb.GetComponent<HpBar>().MyPlayer = player.GetComponent<Player>();
    }
    private void SetLocalOrEnemy()
    {

        if (_players.Length == 2)
        {

            playerChecked = true;
            foreach (GameObject _player in _players)
            {
                if (_player.GetComponent<PlayerSetup>().isLocalPlayer)
                    myPlayer = _player;
                else
                    enemyPlayer = _player;
            }
            enemyPlayer.GetComponent<AudioListener>().enabled = false;
            if (PhotonNetwork.IsMasterClient) 
            {
                //myPlayer.GetComponent<PlayerController>().CharacterChange();
                //enemyPlayer.GetComponent<PlayerController>().CharacterChange();
                myPlayer.GetComponent<PlayerController>().CharacterMove(spawnLocation[1]);
            }
            else
            {
                //myPlayer.GetComponent<PlayerController>().CharacterChange();
                //enemyPlayer.GetComponent<PlayerController>().CharacterChange(); 
            }
            SetEnemyTreasure();
        }
    }    
    private void SetGameResult()
    {
        if (myPlayer.GetComponent<Player>().Hp <= 0 || myPlayer.GetComponent<Player>().myTreasurefound)
            gameResult = 2; //패배
        if (enemyPlayer != null && (enemyPlayer.GetComponent<Player>().Hp <= 0 || enemyPlayer.GetComponent<Player>().myTreasurefound))
            gameResult = 1; //승리
        if (gameResult == 0 && _players.Length < 2)
            gameResult = 1;
    }
    private void GameResultCheck()
    {
        switch (gameResult)
        {
            case 0:
                break;
            case 1:
                if (!resultSoundPlayed)
                {
                    resultSoundPlayed = true;
                    SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Win"]);
                }                    
                WinUI.SetActive(true);
                gameEnd = true;
                SetCursorVisible();
                break;
            case 2:
                if (!resultSoundPlayed)
                {
                    resultSoundPlayed = true;
                    SoundManager.instance.PlayClip(audioSource, SoundManager.instance.audioClips["Lose"]);
                }                    
                LoseUI.SetActive(true);
                gameEnd = true;
                SetCursorVisible();
                break;
        }
    }
    public void ShowTreasureText(TreasureController _treasure)
    {
        treasureText.text = "(E)를 연타해 보물을 획득하세요! \n" + PlayerController.treasureClickCount.ToString() + "/10";
        treasureText.gameObject.SetActive(true);
    }
    public void HideTreasureText()
    {
        treasureText.gameObject.SetActive(false);
    }
    private void CheckAllTreasureSpawned()
    {
        if (!treasureChecked)
        {
            GameObject[] treasures = GameObject.FindGameObjectsWithTag("Treasure");

            if (treasures.Length == 2)
            {
                treasureSettingEnd = true;
                treasureChecked = true;
            }
            else
                treasureSettingEnd = false;
        }
    }
    private void OpenMenu()
    {
        SetMenuFlag();
        MenuUI.SetActive(true);
    }
    public void CloseMenu()
    {
        SetMenuFlag();
        GetComponent<DataSave>().SaveData();
        MenuUI.SetActive(false);
    }
    public void SetMenuFlag()
    {
        menuOn = !menuOn;
        if (menuOn)
            SetCursorVisible();
        else if (roomJoined)
            SetCursorInvisible();
    }
    private void SetCursorVisible()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void SetCursorInvisible()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void SetEnemyTreasure()
    {
        TreasureController[] _tresures = FindObjectsOfType<TreasureController>();
        foreach(TreasureController tr in _tresures)
            if (tr.Owner != myPlayer.GetComponent<Player>())
                enemyTreasure = tr.gameObject;
    }
    public IEnumerator CoNoticeMonsterDead()
    {
        treasureText.text = "적이 몬스터를 처치했습니다.";
        textOn = true;
        treasureText.gameObject.SetActive(true);
        yield return new WaitForSeconds(10f);
        textOn = false;
        treasureText.gameObject.SetActive(false);
    }
}
