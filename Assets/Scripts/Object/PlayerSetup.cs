using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public PlayerController playerController;
    public GunController gunController;
    public new GameObject camera;

    public bool isLocalPlayer = false;

    public void IsLocalPlayer()
    {
        playerController.enabled = true;
        gunController.enabled = true;
        camera.SetActive(true);
        isLocalPlayer = true;
    }
}
