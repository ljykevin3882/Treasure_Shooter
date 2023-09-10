using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using Photon.Pun;
using Input = UnityEngine.Input;

public class PlayerController : MonoBehaviour
{
    //민감도
    [SerializeField] private float lookSensitivity;
    //카메라 한계
    [SerializeField] private float cameraRotationLimit;
    private float currentCameraRotationX = 0;

    //필요컴포넌트
    [SerializeField] private Transform camRotateAxis;
    [SerializeField] private Crosshair crosshair;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform rayStartPoint;
    [SerializeField] private Camera theCam;
    public GameObject buffArrow;

    private CapsuleCollider capsuleCollider;
    private Rigidbody myrigid;
    [PunRPC]
    public Player myPlayer;
    private RaycastHit hitInfo;
    public AudioSource playerAudioSource;

    public bool dead = false;
    static public int treasureClickCount = 0;
    public Crosshair Cross { get { return crosshair; } set { crosshair = value; } }

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myrigid = GetComponent<Rigidbody>();
        myPlayer = GetComponent<Player>();
        myPlayer.applySpeed = myPlayer.walkSpeed;
        UpdateSensitivity();
    }
    void Update()
    {
        if (!dead && !GameManager.instance.menuOn && !GameManager.gameEnd)
        {
            IsGround();
            TryRun();
            TryJump();
            Move();
            //Die();
            TreasureInteract();
            CharacterRotation();
            CameraRotation();
            CrosshairAnimation();
            UpdateSensitivity();
        }
    }
    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        MoveCheck(moveDirX, moveDirZ);

        Vector3 _moveHorizontal = transform.right * moveDirX; //1,0,0
        Vector3 _moveVertical = transform.forward * moveDirZ; //0,0,1

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * myPlayer.applySpeed; // 1,0,1 인데 정규화 해주면 0.5, 0 , 0.5 로 바뀜
        myrigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }
    private void MoveCheck(float _inputX, float _inputZ)
    {
        if (myPlayer.isGround)
        {
            if (_inputX != 0 || _inputZ != 0)
            {
                myPlayer.isWalk = true;
                myPlayer.anim.SetBool("Walk", true);
                if (!playerAudioSource.isPlaying)
                {
                    if (myPlayer.isRun)
                    {
                        SoundManager.instance.PlayClip(playerAudioSource, SoundManager.instance.audioClips["Run"]);
                        GetComponent<PhotonView>().RPC("PlayPlayerSound", RpcTarget.All, "Run");
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(playerAudioSource, SoundManager.instance.audioClips["Walk"]);
                        GetComponent<PhotonView>().RPC("PlayPlayerSound", RpcTarget.All, "Walk");
                    }
                }
            }
            else
            {
                myPlayer.isWalk = false;
                myPlayer.anim.SetBool("Walk", false);
            }
        }
    }
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }
    private void Running()
    {
        myPlayer.isRun = true;
        myPlayer.applySpeed = myPlayer.runSpeed;
        myPlayer.anim.speed = 1.3f;
    }
    private void RunningCancel()
    {
        myPlayer.isRun = false;
        myPlayer.applySpeed = myPlayer.walkSpeed;
        myPlayer.anim.speed = 1f;
    }
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && myPlayer.isGround)
        {
            Jump();
        }
    }
    private void Jump()
    {
        myrigid.velocity = transform.up * myPlayer.jumpForce;
        crosshair.JumpAnimation();
    }
    //private void Die()
    //{
    //    if (myPlayer.hp <= 0)
    //    {
    //        dead = true;
    //        myPlayer.anim.SetTrigger("Die");
    //    }
    //}
    private void TreasureInteract()
    {
        if (Physics.Raycast(rayStartPoint.position, theCam.transform.forward, out hitInfo, 5, layerMask))
        {
            if (hitInfo.transform.GetComponent<TreasureController>().Owner != gameObject.GetComponent<Player>())
            {
                GameManager.instance.ShowTreasureText(hitInfo.transform.GetComponent<TreasureController>());
                if (Input.GetKeyDown(KeyCode.E))
                {
                    treasureClickCount++;
                    if (treasureClickCount >= 10)
                    {
                        GameManager.gameResult = 1;
                        Debug.Log(hitInfo.transform.GetComponent<TreasureController>().nameText);
                        GameManager.instance.enemyPlayer.gameObject.GetComponent<PhotonView>().RPC("TreasureFoundByEnemy", RpcTarget.All, null);
                    }
                }
            }
        }
        else if (!GameManager.instance.textOn)
        {
            treasureClickCount = 0;
            GameManager.instance.HideTreasureText();
        }
    }
    private void CrosshairAnimation()
    {
        if (myPlayer.isWalk)
        {
            if (myPlayer.isRun)
                crosshair.RunningAnimation(true);
            else
            {
                crosshair.RunningAnimation(false);
                crosshair.WalkingAnimation(true);
            }
        }
        else
        {
            crosshair.WalkingAnimation(false);
        }
    }
    private void UpdateSensitivity()
    {
        lookSensitivity = GameManager.instance.mySensitivity;
    }
    private void IsGround()
    {
        myPlayer.isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f); // transform.down으로 하지않는 이유는 캐릭터가 뒤집혔을때 때문에, capsuleCollider.bounds.extents.y는 경계까지의 길이
    }
    private void CharacterRotation()
    {
        //좌우 캐릭터 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation * lookSensitivity, 0f);
        //myrigid.MoveRotation(myrigid.rotation * Quaternion.Euler(_characterRotationY)); // 벡터를 쿼터니언 값으로 변환시키는 함수 Quaternion.Euler()
        myrigid.transform.rotation = myrigid.rotation * Quaternion.Euler(_characterRotationY);
    }

    public void CharacterMove(Transform t)
    {
        transform.position = t.position;
    }
    private void CameraRotation()
    {
        //상하 카메라 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y") * (-1);
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX += _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // 범위 제한함수 
        camRotateAxis.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}