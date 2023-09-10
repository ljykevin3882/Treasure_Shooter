using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    private Player myPlayer;
    [SerializeField] private Image fill;

    public Player MyPlayer { get { return myPlayer; } set { myPlayer = value; } }

    void Update()
    {
        FillRatioUpdate();
    }

    private void FillRatioUpdate()
    {
        fill.fillAmount = myPlayer.Hp / myPlayer.MaxHp;
    }
}
