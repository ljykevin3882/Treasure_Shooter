using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasurePreviewController : MonoBehaviour
{
    private List<Collider> colliderList = new List<Collider>();

    [SerializeField] private LayerMask layerMap;

    [SerializeField] private Material green;
    [SerializeField] private Material red;
    //static public bool canBuild; 
    void Update()
    {
        ChangeColor();
    }

    private void ChangeColor() //보물 설치 가능 여부에 따라 보물 색 변경
    {
        if (!TreasureSettingController.canBuild) 
            SetColor("red"); 
        else 
            SetColor("green");
    }
    public void SetColor(string color)
    {
        Material mat=green;
        if (color == "green") {
            mat = green;
        }else if (color == "red")
        {
            mat = red;
        }
        var newMaterials = new Material[transform.GetComponent<Renderer>().materials.Length];
        for (int i = 0; i < newMaterials.Length; i++)
            newMaterials[i] = mat;
        transform.GetComponent<Renderer>().materials = newMaterials;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerMap)
            colliderList.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == layerMap)
            colliderList.Remove(other);
    }

    public bool isBuildable()
    {
        return colliderList.Count == 0;
    }
}
