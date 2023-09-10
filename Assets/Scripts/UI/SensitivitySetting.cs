using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySetting : MonoBehaviour
{
    [SerializeField] private Slider sensitivityBar;
    [SerializeField] private TextMeshProUGUI valueText;

    void Start()
    {
        if (GameManager.instance != null)
            sensitivityBar.value = GameManager.instance.mySensitivity / 100;
    }
    public void SetSensitivity()
    {
        GameManager.instance.mySensitivity = sensitivityBar.value * 100;
    }
    public void UpdateValueText()
    {
        valueText.text = (Mathf.Floor(sensitivityBar.value * 10000f) / 100f).ToString();
    }
}
