using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GunController : MonoBehaviour
{
    //활성화 여부
    public static bool isActivate = false;
    //현재 장착된 총
    [SerializeField] private Gun myGun;
    //효과음
    public AudioSource gunAudioSource;
    //연사 속도 
    private float currentFireRate;
    //상태 변수
    public bool isReload = false;

    //레이저 충돌 정보 받아옴
    private RaycastHit hitInfo;

    [SerializeField] private Camera theCam;
    private Crosshair crosshair;

    //피격 이펙트
    [SerializeField] private GameObject hit_effect_prefab;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Transform bulletStartPoint;

    public Crosshair Cross { get { return crosshair; } set { crosshair = value; } }

    void Update()
    {
        if (!GetComponent<PlayerController>().dead && !GameManager.instance.menuOn && !GameManager.gameEnd)
        {
            GunFireRateCalc();
            TryFire();
            TryReload();
        }
    }
    //연사속도 재계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 1초의 역수 = 60분의1, 결국 1초에 1 씩 감소시킴
    }
    //발사시도
    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && isReload == false)
            Fire();
    }
    //발사 전 계산
    private void Fire()
    {
        if (!isReload)
        {
            if (myGun.currentBulletCount > 0)
                Shoot();
            else
                StartCoroutine(ReloadCoroutine());
        }
    }
    //재장전
    IEnumerator ReloadCoroutine()
    {
        if (myGun.carryBulletCount > 0)
        {
            isReload = true;
            myGun.anim.SetTrigger("Reload");
            SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Reload"]);
            myGun.carryBulletCount += myGun.currentBulletCount; //총알 남은채로 장전하면 남은총알 유지
            myGun.currentBulletCount = 0;
            yield return new WaitForSeconds(myGun.reloadTime);
            if (myGun.carryBulletCount >= myGun.reloadBulletCount)
            {
                myGun.currentBulletCount = myGun.reloadBulletCount;
                myGun.carryBulletCount -= myGun.reloadBulletCount;
            }
            else
            {
                myGun.currentBulletCount = myGun.carryBulletCount;
                myGun.carryBulletCount = 0;
            }
            isReload = false;
        }
        else
            Debug.Log("총알 없음");
    }
    //재장전 시도
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && myGun.currentBulletCount < myGun.reloadBulletCount)
            StartCoroutine(ReloadCoroutine());
    }
    public void CancelReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }
    //발사후
    private void Shoot()
    {
        myGun.currentBulletCount--;
        myGun.anim.SetTrigger("Shoot");
        SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Fire"]);
        GetComponent<PhotonView>().RPC("PlayGunSound", RpcTarget.All, "Fire");
        crosshair.FireAnimation();
        currentFireRate = myGun.fireRate; //연사속도 재계산
        Hit();
        //총기반동 코루틴 실행
        StopAllCoroutines();
    }
    private void Hit()
    {
        if (Physics.Raycast(bulletStartPoint.position, theCam.transform.forward + 
            new Vector3(Random.Range(-crosshair.GetAccuracy() - myGun.accuracy, crosshair.GetAccuracy() + myGun.accuracy),//x축 오차범위 반영
            Random.Range(-crosshair.GetAccuracy() - myGun.accuracy, crosshair.GetAccuracy() + myGun.accuracy), 0), //y축 오차범위 반영
            out hitInfo, myGun.range, layerMask))
        {
            var effect = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)); //총알 맞은 이펙트 생성
            Destroy(effect, 2f); //2초뒤에 총알 이펙트 삭제
            if (hitInfo.transform.gameObject.GetComponent<Player>()) //플레이어에 적중했을 때
            {
                hitInfo.transform.gameObject.GetComponent<PhotonView>().RPC("DecreaseHP", RpcTarget.All, myGun.damage, GetComponent<Player>().id);
                SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Fire"]);
                SoundManager.instance.PlayClip(GetComponent<PlayerController>().playerAudioSource, SoundManager.instance.audioClips["Damage"]);
            }
            if (hitInfo.transform.gameObject.GetComponent<SlimeController>()) //몬스터에 적중했을 때
            {
                hitInfo.transform.gameObject.GetComponent<PhotonView>().RPC("DecreaseHP", RpcTarget.All, myGun.damage);
                SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Fire"]);
                SoundManager.instance.PlayClip(GetComponent<PlayerController>().playerAudioSource, SoundManager.instance.audioClips["Damage"]);
                if (hitInfo.transform.gameObject.GetComponent<SlimeController>().Hp <= 0)
                {
                    hitInfo.transform.GetComponent<SlimeController>().Catcher = transform.GetComponent<PlayerController>();
                    hitInfo.transform.GetComponent<SlimeController>().SetBuff();
                    SoundManager.instance.PlayClip(GetComponent<PlayerController>().playerAudioSource, SoundManager.instance.audioClips["MonsterKill"]);
                    Destroy(hitInfo.transform.gameObject);
                }
            }
        }
    }
}