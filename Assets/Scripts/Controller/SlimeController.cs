using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    //stat
    [SerializeField] private float hp;
    [SerializeField] private int speed;
    private float currentMoveTime = 0f;
    [SerializeField] private float moveTime;

    private Vector3 rotation;

    private PlayerController catcher;
    private Rigidbody rigid;
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject DieEffect;

    public float Hp { get { return hp; } set { hp = value; } }
    public PlayerController Catcher { get { return catcher; } set { catcher = value; } }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        SetRotation();
    }
    void Update()
    {
        TimeElapse();
        Move();
    }
    private void TimeElapse()
    {
        currentMoveTime += Time.deltaTime;
        if (currentMoveTime >= moveTime)
            SetRotation();
    }
    private void SetRotation() //방향 전환 함수
    {
        currentMoveTime = 0f;

        int _x = Random.Range(0, 360);
        int _y = Random.Range(0, 360);
        int _z = Random.Range(0, 360);

        rotation.Set(_x, _y, _z);
        transform.eulerAngles = rotation;
    }
    private void Move() //전방 이동 함수
    {
        rigid.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        CheckWall(); 
    }
    private void CheckWall() //벽을 감지하는 함수
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 2f, layerMask))
            SetRotation();
    }
    public void SetBuff()
    {
        //버프 적용 로직
        if (catcher != null)
        {
            catcher.buffArrow.SetActive(true);
            GameManager.instance.enemyPlayer.GetComponent<PhotonView>().RPC("NoticeMonsterDead", RpcTarget.All, null);
        }         
    }
    [PunRPC]
    public void DecreaseHP(float damage)
    {
        hp -= damage;
        print("몬스터 HP:" + hp);
        if (hp <= 0)
        {
            GameObject _effect = Instantiate(DieEffect, transform.position, Quaternion.identity);
            Destroy(_effect, 3f);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
