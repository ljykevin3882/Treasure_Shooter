using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    private Rigidbody rigid;
    [SerializeField] private LayerMask layerMask;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void Update()
    {
        PreventGoThroughWall();
        Debug.DrawRay(transform.position, transform.forward, Color.red, 1f);
    }
    private void PreventGoThroughWall()
    {
        if(Physics.Raycast(transform.position, transform.forward, 1f, layerMask))
        {
            rigid.velocity = Vector3.zero;
        }
    }
}
