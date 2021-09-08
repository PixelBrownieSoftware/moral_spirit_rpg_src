using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.System.Core;

public class s_targCharButton : s_button
{
    public o_battleCharData battleChar;
    public s_move mov;
    public string itemName;
    public enum BTN_TYPE
    {
        SKILL_USE,
        ITEM,
        ASSIGN_SKILL,
        SKILL_LOOK
    }
    public BTN_TYPE targType;

    protected override void OnButtonClicked()
    {
        switch (targType) {
            case BTN_TYPE.ITEM:
                rpg_globals.gl.UseItem(mov.name);
                switch (mov.statusMoveType)
                {
                    case STATUS_MOVE_TYPE.HEAL:
                        battleChar.hitPoints += mov.power;
                        battleChar.hitPoints = Mathf.Clamp(battleChar.hitPoints, 0, battleChar.maxHitPoints);
                        s_soundmanager.GetInstance().PlaySound("healsound2");
                        break;

                    case STATUS_MOVE_TYPE.HEAL_STAMINA:
                        battleChar.skillPoints += mov.power;
                        battleChar.skillPoints = Mathf.Clamp(battleChar.skillPoints, 0, battleChar.maxSkillPoints);
                        s_soundmanager.GetInstance().PlaySound("spHealSound");
                        break;
                }
                break;

            case BTN_TYPE.SKILL_LOOK:
                s_menuhandler.GetInstance().GetMenu<s_skillsMenu>("SkillsMenu").target = battleChar;
                break;

            case BTN_TYPE.SKILL_USE:

                battleChar.skillPoints -= mov.cost;
                battleChar.hitPoints += (int)((float)mov.power * (float)(battleChar.intelligence / 4));

                battleChar.hitPoints = Mathf.Clamp(battleChar.hitPoints, 0, battleChar.maxHitPoints);
                break;

            case BTN_TYPE.ASSIGN_SKILL:

                s_menuhandler.GetInstance().GetMenu<s_skillsMenu>("ExtraSkillsMenu").target = battleChar;
                base.OnButtonClicked();
                break;
        }
       // 
    }

    private void Update()
    {
        switch (targType)
        {
            case BTN_TYPE.SKILL_USE:
            if (battleChar.skillPoints < mov.cost)
            {
                gameObject.SetActive(false);
            }
                break;
        }
        switch (targType)
        {
            case BTN_TYPE.SKILL_USE:
            case BTN_TYPE.ITEM:
                switch (mov.statusMoveType)
                {
                    case STATUS_MOVE_TYPE.HEAL:
                        if (battleChar.hitPoints >= battleChar.maxHitPoints)
                        {
                            gameObject.SetActive(false);
                        }
                        else
                        {
                            gameObject.SetActive(true);
                        }
                        break;

                    case STATUS_MOVE_TYPE.HEAL_STAMINA:
                        if (battleChar.skillPoints >= battleChar.maxSkillPoints)
                        {
                            gameObject.SetActive(false);
                        }
                        else
                        {
                            gameObject.SetActive(true);
                        }
                        break;
                }
                break;
        }
        if (battleChar != null)
        {
            if (battleChar.name != "")
            {
                if (mov != null)
                {
                    if (mov.statusMoveType == STATUS_MOVE_TYPE.HEAL_STAMINA)
                        txt.text = battleChar.name + " SP: " + battleChar.skillPoints;
                    else
                        txt.text = battleChar.name + " HP: " + battleChar.hitPoints;
                }
            }
        }
    }
}
