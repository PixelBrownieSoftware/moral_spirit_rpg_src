using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoundation2.System.Core;
using MagnumFoundation2.System;

public class s_buttonSkill : s_button
{
    public s_battleMenu BMenu;
    public s_move moveButton;
    public Image element;
    public enum SKILL_TYPE {
        BATTLE,
        GROUP_SELECT
    }
    public SKILL_TYPE typeOfButton;
    public bool item = false;
    public int itemCount = 0;
    o_battleChar bc;

    private void Update()
    {
        if (s_battlesyst.GetInstance().currentCharacter != null) {
            bc = s_battlesyst.GetInstance().currentCharacter;
        }
        if (bc != null)
        {
            if (!item)
            {
                if (moveButton.cost > 0)
                {
                    switch (moveButton.moveType)
                    {
                        case MOVE_TYPE.PHYSICAL:

                            if (bc.hitPoints <= moveButton.cost)
                                txt.color = Color.red;
                            else
                                txt.color = Color.black;
                            txt.text = moveButton.name + " - " + Mathf.RoundToInt(((float)moveButton.cost / 100) * bc.maxHitPoints) + " HP";
                            break;

                        case MOVE_TYPE.TALK:
                        case MOVE_TYPE.SPECIAL:
                            if (bc.skillPoints < moveButton.cost)
                                txt.color = Color.red;
                            else
                                txt.color = Color.black;
                            txt.text = moveButton.name + " - " + moveButton.cost + " SP";
                            break;

                        case MOVE_TYPE.STATUS:

                            if (!moveButton.isFixed)
                            {
                                switch (moveButton.statusMoveType)
                                {
                                    default:
                                        if (bc.skillPoints < moveButton.cost)
                                            txt.color = Color.red;
                                        else
                                            txt.color = Color.black;
                                        txt.text = moveButton.name + " - " + moveButton.cost + " SP";
                                        break;

                                    case STATUS_MOVE_TYPE.HEAL_STAMINA:

                                        if (bc.hitPoints <= ((float)(moveButton.cost / 100) * bc.maxHitPoints))
                                            txt.color = Color.red;
                                        else
                                            txt.color = Color.black;
                                        txt.text = moveButton.name + " - " + Mathf.RoundToInt(((float)moveButton.cost / 100) * bc.maxHitPoints) + " HP";
                                        break;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    txt.color = Color.black;
                    txt.text = moveButton.name;
                }
            }
            else {
                txt.color = Color.black;
                txt.text = moveButton.name + " - " + itemCount;
            }
        }
    }

    protected override void OnHover()
    {
        BMenu.SetDescription(moveButton);
        base.OnHover();
    }

    protected override void OnButtonClicked()
    {
        s_move m = null;
        if (item)
        {
            m = moveButton;
            rpg_globals.gl.UseItem(moveButton.name);
        }
        else
        {
            m = moveButton;
            if (moveButton.cost > 0)
            {
                switch (moveButton.moveType)
                {
                    case MOVE_TYPE.PHYSICAL:

                        if (bc.hitPoints <= ((float)(moveButton.cost / 100) * bc.maxHitPoints))
                        {
                            return;
                        }
                        break;

                    case MOVE_TYPE.STATUS:
                        if (!moveButton.isFixed) {
                            switch (moveButton.statusMoveType) {
                                default:
                                    if (bc.skillPoints < moveButton.cost)
                                    {
                                        return;
                                    }
                                    break;

                                case STATUS_MOVE_TYPE.HEAL_STAMINA:

                                    if (bc.hitPoints <= ((float)(moveButton.cost / 100) * bc.maxHitPoints))
                                    {
                                        return;
                                    }
                                    break;
                            }
                        }
                        break;
                    case MOVE_TYPE.TALK:
                    case MOVE_TYPE.SPECIAL:

                        if (bc.skillPoints < moveButton.cost)
                        {
                            return;
                        }
                        break;
                }
            }
        }
        switch (typeOfButton)
        {
            case SKILL_TYPE.BATTLE:
                if (m.onTeam)
                {
                    if (m.onSelf)
                    {
                        s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>
                        ("BattleMenuTarget").bcs = new List<o_battleChar>();
                        s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>
                        ("BattleMenuTarget").bcs.Add(s_battlesyst.GetInstance().currentCharacter);
                    }
                    else
                    {
                        s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>
                        ("BattleMenuTarget").bcs = new List<o_battleChar>();
                        foreach (o_battleChar bc in s_battlesyst.GetInstance().players)
                        {
                            s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>
                        ("BattleMenuTarget").bcs.Add(bc);
                        }
                    }

                }
                else
                {
                    s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>
                    ("BattleMenuTarget").bcs = new List<o_battleChar>();
                    foreach (o_battleChar bc in s_battlesyst.GetInstance().opposition)
                    {
                        if (bc.hitPoints > 0)
                            s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>("BattleMenuTarget").bcs.Add(bc);
                        else
                            continue;
                    }

                }
                s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>
                ("BattleMenuTarget").mov = m;
                s_soundmanager.GetInstance().PlaySound("selectOption");
                break;
        }
        base.OnButtonClicked();
    }
}
