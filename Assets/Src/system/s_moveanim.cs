using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.Objects;

public class s_moveanim : o_generic
{
    public Animator anim;
    public GameObject subObj;
    

    public void StopAnimation()
    {
        anim.Play("");
        transform.localScale = new Vector3(1, 1, 1);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        subObj.transform.localScale = new Vector3(1, 1, 1);
        subObj.transform.localEulerAngles = new Vector3(0, 0, 0);
        DespawnObject();
    }
}
