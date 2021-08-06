using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_elementalWeaknessGUI : MonoBehaviour
{
    public Image weakImg;
    public Text weakTXT;
    public Text weakTXTShadow;
    public Text number;
    o_battleCharData bcD;
    public ELEMENT el;
    public ACTION_TYPE talkEl;

    public bool isElement = true;

    public Color normal;
    public Color frail;
    public Color voidDMG;
    public Color resist;
    public Color absorb;
    public Color reflect;

    public void SetToDat(o_battleCharData pd)
    {
        bcD = pd;
    }

    void Update()
    {
        if (bcD != null)
        {
            float aff = 0;
            if (isElement)
            {
                aff = bcD.dataSrc.elementTypeCharts[(int)el];
            }
            else
            {
                aff = bcD.dataSrc.actionTypeCharts[(int)talkEl];
            }

            number.text ="" + aff;
            
            if (aff > 1.999f)
            {
                weakTXT.text = "Weak";
                weakImg.color = frail;
            }
            else if (aff < 2 && aff >= 1)
            {
                weakTXT.text = "----";
                weakImg.color = normal;
            }
            else if (aff < 1 && aff > 0)
            {

                weakTXT.text = "Res";
                weakImg.color = resist;
            }
            else if (aff == 0)
            {
                weakTXT.text = "Void";
                weakImg.color = voidDMG;
            }
            else if (aff < 0 && aff > -1.999f)
            {
                weakTXT.text = "Ref";
                weakImg.color = reflect;
            }
            else if (aff < -2)
            {
                weakTXT.text = "Abs";
                weakImg.color = absorb;
            }
            weakTXTShadow.text = weakTXT.text;
        }
    }
}
