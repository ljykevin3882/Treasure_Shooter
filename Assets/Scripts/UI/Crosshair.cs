using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private Animator anim;

    private float gunAccuracy;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void WalkingAnimation(bool _flag)
    {
        anim.SetBool("Walking", _flag);
    }
    public void RunningAnimation(bool _flag)
    {
        anim.SetBool("Running", _flag);
    }
    public void JumpAnimation()
    {
        anim.SetTrigger("Jump");
    }
    public void FireAnimation()
    {
        if (anim.GetBool("Running"))
            anim.SetTrigger("RunFire");
        else if (anim.GetBool("Walking"))
            anim.SetTrigger("WalkFire");
        else
            anim.SetTrigger("IdleFire");
    }
    public float GetAccuracy()
    {
        if (anim.GetBool("Walking"))
            gunAccuracy = 0.06f;
        else if (anim.GetBool("Running"))
            gunAccuracy = 0.1f;
        else
            gunAccuracy = 0.035f;

        return gunAccuracy;
    }
}
