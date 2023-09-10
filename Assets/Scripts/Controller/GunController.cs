using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GunController : MonoBehaviour
{
    //Ȱ��ȭ ����
    public static bool isActivate = false;
    //���� ������ ��
    [SerializeField] private Gun myGun;
    //ȿ����
    public AudioSource gunAudioSource;
    //���� �ӵ� 
    private float currentFireRate;
    //���� ����
    public bool isReload = false;

    //������ �浹 ���� �޾ƿ�
    private RaycastHit hitInfo;

    [SerializeField] private Camera theCam;
    private Crosshair crosshair;

    //�ǰ� ����Ʈ
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
    //����ӵ� ����
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; // 1���� ���� = 60����1, �ᱹ 1�ʿ� 1 �� ���ҽ�Ŵ
    }
    //�߻�õ�
    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && isReload == false)
            Fire();
    }
    //�߻� �� ���
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
    //������
    IEnumerator ReloadCoroutine()
    {
        if (myGun.carryBulletCount > 0)
        {
            isReload = true;
            myGun.anim.SetTrigger("Reload");
            SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Reload"]);
            myGun.carryBulletCount += myGun.currentBulletCount; //�Ѿ� ����ä�� �����ϸ� �����Ѿ� ����
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
            Debug.Log("�Ѿ� ����");
    }
    //������ �õ�
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
    //�߻���
    private void Shoot()
    {
        myGun.currentBulletCount--;
        myGun.anim.SetTrigger("Shoot");
        SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Fire"]);
        GetComponent<PhotonView>().RPC("PlayGunSound", RpcTarget.All, "Fire");
        crosshair.FireAnimation();
        currentFireRate = myGun.fireRate; //����ӵ� ����
        Hit();
        //�ѱ�ݵ� �ڷ�ƾ ����
        StopAllCoroutines();
    }
    private void Hit()
    {
        if (Physics.Raycast(bulletStartPoint.position, theCam.transform.forward + 
            new Vector3(Random.Range(-crosshair.GetAccuracy() - myGun.accuracy, crosshair.GetAccuracy() + myGun.accuracy),//x�� �������� �ݿ�
            Random.Range(-crosshair.GetAccuracy() - myGun.accuracy, crosshair.GetAccuracy() + myGun.accuracy), 0), //y�� �������� �ݿ�
            out hitInfo, myGun.range, layerMask))
        {
            var effect = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)); //�Ѿ� ���� ����Ʈ ����
            Destroy(effect, 2f); //2�ʵڿ� �Ѿ� ����Ʈ ����
            if (hitInfo.transform.gameObject.GetComponent<Player>()) //�÷��̾ �������� ��
            {
                hitInfo.transform.gameObject.GetComponent<PhotonView>().RPC("DecreaseHP", RpcTarget.All, myGun.damage, GetComponent<Player>().id);
                SoundManager.instance.PlayClip(gunAudioSource, SoundManager.instance.audioClips["Fire"]);
                SoundManager.instance.PlayClip(GetComponent<PlayerController>().playerAudioSource, SoundManager.instance.audioClips["Damage"]);
            }
            if (hitInfo.transform.gameObject.GetComponent<SlimeController>()) //���Ϳ� �������� ��
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