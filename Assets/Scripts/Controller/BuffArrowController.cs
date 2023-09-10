using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffArrowController : MonoBehaviour
{
    private float curAliveTime = 0f;
    private float aliveTime = 10f;

    void Update()
    {
        TimeElapse();
        SetRotation();
    }
    private void TimeElapse() //지속 시간 측정 및 화살표 제거 함수
    {
        curAliveTime += Time.deltaTime;
        if(curAliveTime >= aliveTime)
        {
            gameObject.SetActive(false);
        }
    }
    private void SetRotation() //화살표 방향 설정 함수
    {
        transform.LookAt(GameManager.instance.enemyTreasure.transform.position);
    }
}
