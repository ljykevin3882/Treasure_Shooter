using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;
using Photon.Pun;

public class TreasureSettingController : MonoBehaviour
{
    [SerializeField] private float camMoveSpeed;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private float range;

    private int index;
    private float currentCameraRotationX = 0;
    private float currentCameraRotationY = 0;

    private bool previewOn;
    private bool spawned = false;
    static public bool canBuild;

    private Vector3 originPos;

    private Rigidbody rigid;
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private GameObject[] treasurePreview;
    [SerializeField] private GameObject[] treasure;

    private GameObject goPreview,bgm;
    public GameObject tr;

    private int setTime = 30;
    private float timer = 0;
    void Start()
    {
        bgm = GameObject.Find("BGM");
        bgm.GetComponent<bgmController>().PlayBGM("IngameBGM");
        rigid = GetComponent<Rigidbody>();
        index = Random.Range(0, treasurePreview.Length);
        goPreview = Instantiate(treasurePreview[index]);
        previewOn = true;
        tr = null;
        originPos = transform.position;
    }
    void Update()
    {
        CameraMove();
        CameraRotate();
        PreviewPositionUpdate();
        if (Input.GetMouseButtonDown(0) && transform.position != originPos &&canBuild)
            Build();
        if(RoomManager.allPlayerIn)
            CheckTimeOver();
        if (spawned)
            this.enabled = false;
    }
    private void PreviewPositionUpdate() //보물 
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask)) //설치 가능 범위안에 지형이 감지되면
        {
            canBuild = true; //설치 가능한 상태
            Vector3 _location = hitInfo.point; //지형이 감지된 위치에 보물 두기
            goPreview.transform.position = _location;
        }
        else //설치 가능 범위안에 아무것도 없으면
        {
            canBuild = false; //설치 불가능 상태
            Vector3 _location = transform.position + transform.forward * range; //허공에 보물 두기
            goPreview.transform.position = _location;
        }
    }
    private void Build()
    {
        if (previewOn)
        {
            if(hitInfo.transform != null) //지형에 올바르게 설치 한 경우
                tr = PhotonNetwork.Instantiate(treasure[index].name, hitInfo.point, Quaternion.identity);
            else //제한시간 내에 설치를 못해 강제로 바닥에 설치되는 경우 
                tr = PhotonNetwork.Instantiate(treasure[index].name, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
            spawned = true;
            Destroy(goPreview); //미리보기 오브젝트 삭제
        }
    }
    private void CameraMove()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * moveDirX; //1,0,0
        Vector3 _moveVertical = transform.forward * moveDirZ; //0,0,1

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * camMoveSpeed; // 1,0,1 인데 정규화 해주면 0.5, 0 , 0.5 로 바뀜
        rigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }
    private void CameraRotate()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        float _xRotation = Input.GetAxisRaw("Mouse Y") * (-1);

        float _cameraRotationX = _xRotation * lookSensitivity;
        float _cameraRotationY = _yRotation * lookSensitivity;

        currentCameraRotationX += _cameraRotationX;
        currentCameraRotationY += _cameraRotationY;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -90, 90); // 범위 제한함수 
        transform.eulerAngles = new Vector3(currentCameraRotationX, currentCameraRotationY, 0f);
    }
    private void CheckTimeOver()
    {
        timer += Time.deltaTime;
        string _text = (setTime - (int)timer).ToString();
        GameManager.instance.treasureText.gameObject.SetActive(true);
        GameManager.instance.treasureText.text = "보물 위치 지정이 " + _text + "초 후에 종료됩니다.";
        if (timer >= setTime)
            Build();
    }
}
