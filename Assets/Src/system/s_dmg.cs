using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class s_dmg : MonoBehaviour
{
    public TextMesh text;
    public TextMesh textBK;
    public Animator anim;
    MeshRenderer rendTXT;
    MeshRenderer rendTXTBk;
    public bool isDone;
    public enum HIT_FX_TYPE {
        NONE,
        DMG_SP,
        WEAK_HP,
        WEAK_SP,
        CRIT_HP,
        CRIT_SP,
        MISS,
        HEAL,
        REFLECT,
        BLOCK,
        STAT_INC,
        STAT_DEC
    }

    public Color HPDamage;
    public Color SPDamage;

    public void MarkDone()
    {
        anim.Play("");
        anim.enabled = false;
        isDone = true;
        rendTXT.enabled = false;
        rendTXTBk.enabled = false;
        text.text = "";
        textBK.text = "";
    }

    private void Start()
    {
        rendTXT = text.GetComponent<MeshRenderer>();
        rendTXTBk = textBK.GetComponent<MeshRenderer>();
    }

    public void PlayAnim(string dmg, HIT_FX_TYPE pt)
    {
        anim.enabled = true;
        isDone = false;
        switch (pt)
        {
            default:
                rendTXT.sortingOrder = 11;
                rendTXTBk.sortingOrder = 10;
                rendTXT.enabled = true;
                rendTXTBk.enabled = true;
                text.text = "" + dmg;
                textBK.text = "" + dmg;
                break;
                
            case HIT_FX_TYPE.BLOCK:
            case HIT_FX_TYPE.MISS:
            case HIT_FX_TYPE.REFLECT:
            case HIT_FX_TYPE.STAT_INC:
            case HIT_FX_TYPE.STAT_DEC:
                rendTXT.enabled = false;
                rendTXTBk.enabled = false;
                text.text = "";
                textBK.text = "";
                break;
        }
        switch (pt)
        {
            default:
                text.color = HPDamage;
                break;
            case HIT_FX_TYPE.CRIT_HP:
                text.color = HPDamage;
                break;
            case HIT_FX_TYPE.CRIT_SP:
                text.color = SPDamage;
                break;
            case HIT_FX_TYPE.DMG_SP:
                text.color = SPDamage;
                break;
        }
        switch (pt) {
            default:
                anim.Play("hitAnimation");
                break;
            case HIT_FX_TYPE.BLOCK:
                anim.Play("block");
                break;
            case HIT_FX_TYPE.CRIT_HP:
            case HIT_FX_TYPE.CRIT_SP:
                anim.Play("criticalAnim");
                break;
            case HIT_FX_TYPE.MISS:
                anim.Play("blackAnim");
                break;
            case HIT_FX_TYPE.REFLECT:
                anim.Play("reflectAnim");
                break;
            case HIT_FX_TYPE.HEAL:
                anim.Play("healAnim");
                break;
            case HIT_FX_TYPE.STAT_DEC:
                anim.Play("lower_stat");
                break;
            case HIT_FX_TYPE.STAT_INC:
                anim.Play("increase_stat");
                break;
        }
    }
}
