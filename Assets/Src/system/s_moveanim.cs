using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_moveanim : MonoBehaviour
{
    public Animator anim;

    public void StopAnimation()
    {
        anim.Play("");
        anim.enabled = false;
    }
}
