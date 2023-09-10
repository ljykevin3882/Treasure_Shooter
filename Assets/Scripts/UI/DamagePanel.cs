using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePanel : MonoBehaviour
{
    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
    }
    public void AlphaUpdate()
    {
        StopCoroutine(CoAlphaUpdate());
        StartCoroutine(CoAlphaUpdate());
    }
    private IEnumerator CoAlphaUpdate()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.3f);
        yield return new WaitForSeconds(0.5f);
        for(int i = 1; i<=60; i++)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - 0.005f);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
