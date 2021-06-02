using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_analyzeStats : MonoBehaviour
{
    public o_battleChar battleChar;
    public Text str;
    public Text def;
    public Text dex;
    public Text gut;
    public Text agil;
    public Text nameChar;
    public Text status;
    public Text HP;
    public Text SP;

    private void Awake()
    {
        HideText();
    }

    public void HideText() {
        gameObject.SetActive(false);
    }

    public void SetTextTo(float amount, ref Text txt) {
        if (amount > 0)
        {
            txt.color = Color.green;
            txt.text = "+" + amount;
        } else if(amount == 0){

            txt.color = Color.black;
            txt.text = "" + amount;
        }
        else
        {
            txt.color = Color.red;
            txt.text = "" + amount;
        }
    }

    public void SetText() {
        gameObject.SetActive(true);
        if(battleChar.data.shortName == "")
            nameChar.text = battleChar.name;
        else
            nameChar.text = battleChar.data.shortName;
        SetTextTo(battleChar.attackBuff, ref str);
        SetTextTo(battleChar.defenceBuff, ref def);
        SetTextTo(battleChar.intelligenceBuff, ref dex);
        SetTextTo(battleChar.gutsBuff, ref gut);
        SetTextTo(battleChar.speedBuff, ref agil);
        HP.text = "HP:" +(float)((float)battleChar.hitPoints / (float)battleChar.maxHitPoints) * 100 + "% " + 
            battleChar.hitPoints + "/ " + battleChar.maxHitPoints;
        SP.text = "SP:" +(float)((float)battleChar.skillPoints / (float)battleChar.maxSkillPoints) * 100 + "% " +
            battleChar.skillPoints + "/ " + battleChar.maxSkillPoints;
        switch (battleChar.currentStatus) {
            case STATUS_EFFECT.NONE:
                status.text = "OK";
                break;
            case STATUS_EFFECT.POISON:
                status.text = "Poison";
                break;
            case STATUS_EFFECT.PARALYZED:
                status.text = "Paralyzed";
                break;
            case STATUS_EFFECT.SLEEP:
                status.text = "Asleep";
                break;
        }
        if (battleChar.hitPoints <= 0) {
            status.text = "Unconsious";
        }
    }
}
