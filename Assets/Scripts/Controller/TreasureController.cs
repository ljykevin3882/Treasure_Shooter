using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TreasureController : MonoBehaviour
{
    private Player owner;
    public Player Owner { get { return owner; } set { owner = value; } }

    private bool nameSetEnd = false;

    public string nameText = "";

    void Start()
    {
        owner = null;
    }
    void Update()
    {
        if (owner != null && !nameSetEnd)
            SetName();
    }
    private void SetName()
    {
        nameText = owner.name + "'s treasure";
        nameSetEnd = true;
    }
}
