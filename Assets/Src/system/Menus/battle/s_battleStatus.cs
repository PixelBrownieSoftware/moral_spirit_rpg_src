using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoundation2.System;

public class s_battleStatus : s_menucontroller
{
    public o_battleChar character;
    public s_guiList str;
    public s_guiList dx;
    public s_guiList vit;
    public s_guiList gut;
    public s_guiList agi;

    public Text strText;
    public Text dxText;
    public Text vitText;
    public Text gutText;
    public Text agiText;

    public Text strBuffText;
    public Text dxBuffText;
    public Text vitBuffText;
    public Text gutBuffText;
    public Text agiBuffText;

    public Text nameChar;
    public Slider hp;
    public Slider sp;
    public Slider exp;
    public Text hpText;
    public Text spText;

    public s_elementalWeaknessGUI[] elementalAffinities;
    public s_elementalWeaknessGUI[] talkAffinities;
    RPG_battleMemory memory;

    public override void OnOpen()
    {
        base.OnOpen();
        ResetButton();
        memory = rpg_globals.GetInstance().GetComponent<rpg_globals>().GetBattleMemory(character);
    }

    public void SetTextTo(float amount, ref Text txt)
    {
        if (amount > 0)
        {
            txt.color = Color.green;
            txt.text = ""+ amount;
        }
        else if (amount == 0)
        {

            txt.color = Color.grey;
            txt.text = "" + amount;
        }
        else
        {
            txt.color = Color.red;
            txt.text = "" + amount;
        }
    }

    void Update()
    {
        nameChar.text = character.name + " - Level " + character.level;

        float health = ((float)character.hitPoints / (float)character.maxHitPoints) * 100;
        float stamina = ((float)character.skillPoints / (float)character.maxSkillPoints) * 100f;

        hp.value = Mathf.Round(health);
        sp.value = Mathf.Round(stamina);

        string mSpVal = "?";
        string mHpVal = "?";
        string spVal = "?";
        string hpVal = "?";
        switch (character.maxSkillPoints.ToString().Length)
        {
            case 1:
                mSpVal = "?";
                break;
            case 2:
                mSpVal = "??";
                break;
            case 3:
                mSpVal = "???";
                break;
            case 4:
                mSpVal = "????";
                break;
        }
        switch (character.maxHitPoints.ToString().Length)
        {
            case 1:
                mHpVal = "?";
                break;
            case 2:
                mHpVal = "??";
                break;
            case 3:
                mHpVal = "???";
                break;
            case 4:
                mHpVal = "????";
                break;
        }
        
        switch (character.skillPoints.ToString().Length)
        {
            case 1:
                spVal = "?";
                break;
            case 2:
                spVal = "??";
                break;
            case 3:
                spVal = "???";
                break;
            case 4:
                spVal = "????";
                break;
        }
        switch (character.hitPoints.ToString().Length) {
            case 1:
                hpVal = "?";
                break;
            case 2:
                hpVal = "??";
                break;
            case 3:
                hpVal = "???";
                break;
            case 4:
                hpVal = "????";
                break;
        }

        if (memory.encountered)
        {
            hpText.text = character.hitPoints + "/" + character.maxHitPoints;
            spText.text = character.skillPoints + "/" + character.maxSkillPoints;
        }
        else {
            hpText.text = hpVal + "/" + mHpVal;
            spText.text = spVal + "/" + mSpVal;
        }

        str.amount = character.attack;
        vit.amount = character.defence;
        dx.amount = character.intelligence;
        agi.amount = character.speed;
        gut.amount = character.guts;

        strText.text = "" + character.attack;
        vitText.text = "" + character.defence;
        dxText.text = "" + character.intelligence;
        agiText.text = "" + character.speed;
        gutText.text = "" + character.guts;

        SetTextTo(character.attackBuff, ref strBuffText);
        SetTextTo(character.defenceBuff, ref vitBuffText);
        SetTextTo(character.intelligenceBuff, ref dxBuffText);
        SetTextTo(character.gutsBuff, ref gutBuffText);
        SetTextTo(character.speedBuff, ref agiBuffText);

        /*
        foreach (var g in elementalAffinities)
        {
            g.SetToDat(memory, character);
        }
        */
        foreach (var g in talkAffinities)
        {
            g.SetToDat(memory, character);
        }
    }
}
