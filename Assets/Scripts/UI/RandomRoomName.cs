using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomRoomName : MonoBehaviour
{
    private TMP_InputField text;

    void Start()
    {
        text = GetComponent<TMP_InputField>();
        int num = Random.Range(0, 10);
        text.text = SetRoomName(num);
        RoomList.Instance.ChangeRoomToCreateName(text.text);
    }
    private string SetRoomName(int _num)
    {
        string[] roomNames =
        {
            "초보만 오세요",
            "매너겜 해요",
            "드루와",
            "트레져슈터 갓겜",
            "너 T야?",
            "뉴진스의 하입보이요",
            "이거 완전 MZ인데요?",
            "라잇 웨잇",
            "삼대 50 미만 사절",
            "아오 페리시치"
        };

        return roomNames[_num];
    }
}
