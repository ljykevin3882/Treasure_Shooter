using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemButton : MonoBehaviour
{
    public string RoomName;
    public GameManager gm;
    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void OnButtonPressed()
    {
        RoomList.Instance.JoinRoomByName(RoomName);
        gm.Button1PressSound();
    }
}
