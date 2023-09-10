using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletStatus : MonoBehaviour
{
    private PlayerController playerController;
    private Gun playerGun;
    [SerializeField] private TextMeshProUGUI bulletCountText;

    public PlayerController PlayerController { get { return playerController; } set { playerController = value; } }

    void Start()
    {
        playerGun = playerController.GetComponent<Gun>();
    }
    void Update()
    {
        UpdateText();
    }
    private void UpdateText()
    {
        string currentCount = playerGun.currentBulletCount.ToString();
        string carryCount = playerGun.carryBulletCount.ToString();

        bulletCountText.text = currentCount + " / " + carryCount;
    }
}
