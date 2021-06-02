using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_statReq : MonoBehaviour
{
    public Text requirement;
    public int requirementNum;
    public int statNum;
    public Image reqImg;

    public void Update()
    {
        if (requirementNum <= statNum) {
            requirement.color = Color.green;
            reqImg.color = new Color(0.7f,0.9f,0.7f);
        } else {
            requirement.color = Color.red;
            reqImg.color = new Color(0.9f, 0.7f, 0.7f);
        }
        requirement.text = "" + requirementNum;
    }
}
