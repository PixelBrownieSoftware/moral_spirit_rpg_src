using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoundation2.System;
using MagnumFoundation2.System.Core;

public class s_targetButton : s_button
{
    public o_battleChar battleCharButton;
    public enum TARGET_TYPE {
        BATTLE,
        ANALYZE_TEAM_SELECT_PLAYERS,
        ANALYZE_TEAM_SELECT_ENEMIES,
        ANALYZE
    }
    public TARGET_TYPE targType;
    public s_move mov;

    public Slider HPSlider;
    public Slider SPSlider;

    public Image weaknessIcon;

    public Sprite normDmg;
    public Sprite weakDmg;
    public Sprite resDmg;
    public Sprite unkDmg;

    public bool isItem = false;
    public bool isAll = false;
    RPG_battleMemory memory;

    protected override void OnHover()
    {
        base.OnHover();
        switch (targType)
        {
            case TARGET_TYPE.BATTLE:
                s_soundmanager.GetInstance().PlaySound("cursor_move");
                //s_camera.cam.SetTargPos(battleCharButton.transform.position);
                break;
        }
    }

    public void GetMemory(o_battleChar bc)
    {
        memory = rpg_globals.GetInstance().GetComponent<rpg_globals>().GetBattleMemory(bc);
    }

    public void GetMemory()
    {
        memory = rpg_globals.GetInstance().GetComponent<rpg_globals>().GetBattleMemory(battleCharButton);
    }

    private void Update()
    {
        if (battleCharButton != null)
        {
            float HP = ((float)battleCharButton.hitPoints / (float)battleCharButton.maxHitPoints) * 100;
            float SP = ((float)battleCharButton.skillPoints / (float)battleCharButton.maxSkillPoints) * 100;
            HP = Mathf.Round(HP);
            SP = Mathf.Round(SP);

            HPSlider.value = HP;
            SPSlider.value = SP;
            transform.position = Camera.main.WorldToScreenPoint(battleCharButton.transform.position);
            switch (targType) {
                default:
                    if (mov.element != ELEMENT.UNKNOWN)
                    {
                        weaknessIcon.color = Color.white;
                        switch (mov.moveType)
                        {
                            case MOVE_TYPE.PHYSICAL:
                            case MOVE_TYPE.SPECIAL:
                                if (memory.knownElementAffinites[(int)mov.element])
                                {
                                    if (battleCharButton.elementTypeCharts[(int)mov.element] >= 2)
                                    {
                                        weaknessIcon.sprite = weakDmg;
                                    }
                                    else if (battleCharButton.elementTypeCharts[(int)mov.element] > 0 &&
                                        battleCharButton.elementTypeCharts[(int)mov.element] < 2)
                                    {
                                        weaknessIcon.sprite = normDmg;
                                    }
                                    else if (battleCharButton.elementTypeCharts[(int)mov.element] <= 0)
                                    {
                                        weaknessIcon.sprite = resDmg;
                                    }
                                    if (battleCharButton.hitPoints <= 0)
                                    {
                                        weaknessIcon.sprite = normDmg;
                                    }
                                }
                                else {

                                    weaknessIcon.sprite = unkDmg;
                                }
                                break;

                            case MOVE_TYPE.STATUS:
                                weaknessIcon.sprite = null;
                                break;

                            case MOVE_TYPE.TALK:

                                if (memory.knownTalkAffinites[(int)mov.action_type])
                                {
                                    if (battleCharButton.actionTypeCharts[(int)mov.action_type] >= 2)
                                    {
                                        weaknessIcon.sprite = weakDmg;
                                    }
                                    else if (battleCharButton.actionTypeCharts[(int)mov.action_type] > 0 &&
                                        battleCharButton.actionTypeCharts[(int)mov.action_type] < 2)
                                    {
                                        weaknessIcon.sprite = normDmg;
                                    }
                                    else if (battleCharButton.actionTypeCharts[(int)mov.action_type] <= 0)
                                    {
                                        weaknessIcon.sprite = resDmg;
                                    }
                                    if (battleCharButton.hitPoints <= 0 || battleCharButton.skillPoints <= 0)
                                    {
                                        weaknessIcon.sprite = normDmg;
                                    }
                                }
                                else {
                                    weaknessIcon.sprite = unkDmg;
                                }
                                break;
                        }
                    }
                    break;

                case TARGET_TYPE.ANALYZE:
                    weaknessIcon.color = Color.clear;
                    buttonType = "BattleStatusMenu";
                    weaknessIcon.sprite = null;
                    break;
                case TARGET_TYPE.ANALYZE_TEAM_SELECT_ENEMIES:
                case TARGET_TYPE.ANALYZE_TEAM_SELECT_PLAYERS:
                    buttonType = "BattleMenuTarget";
                    weaknessIcon.color = Color.clear;
                    weaknessIcon.sprite = null;
                    break;
            }
        }
    }

    protected override void OnButtonClicked()
    {
        switch (targType) {
            case TARGET_TYPE.ANALYZE_TEAM_SELECT_ENEMIES:
                s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>("BattleMenuTarget").bcs = s_battlesyst.GetInstance().opposition;
                s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>("BattleMenuTarget").skillType = s_battletargetMenu.SKILL_TYPE.ANALYZE_TARGETS; 
                base.OnButtonClicked();
                break;

            case TARGET_TYPE.ANALYZE_TEAM_SELECT_PLAYERS:
                s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>("BattleMenuTarget").bcs = s_battlesyst.GetInstance().players;
                s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>("BattleMenuTarget").skillType = s_battletargetMenu.SKILL_TYPE.ANALYZE_TARGETS;
                base.OnButtonClicked();
                break;

            case TARGET_TYPE.ANALYZE:
                s_menuhandler.GetInstance().GetMenu<s_battleStatus>("BattleStatusMenu").character = battleCharButton;
                base.OnButtonClicked();
                break;

            case TARGET_TYPE.BATTLE:
                switch (mov.moveType) {
                    case MOVE_TYPE.PHYSICAL:
                        if (s_battlesyst.GetInstance().currentCharacter.hitPoints <= mov.cost) {

                            return;
                        }
                            break;
                    case MOVE_TYPE.SPECIAL:
                    case MOVE_TYPE.TALK:
                        if (s_battlesyst.GetInstance().currentCharacter.skillPoints < mov.cost)
                        {
                            return;
                        }
                        break;
                    case MOVE_TYPE.STATUS:
                        switch (mov.statusMoveType) {
                            case STATUS_MOVE_TYPE.HEAL_STAMINA:
                                if (s_battlesyst.GetInstance().currentCharacter.hitPoints < mov.cost)
                                {
                                    return;
                                }
                                break;
                            default:
                                if (s_battlesyst.GetInstance().currentCharacter.skillPoints < mov.cost)
                                {
                                    return;
                                }
                                break;
                        }
                        break;
                }
                if (isAll)
                    s_battlesyst.GetInstance().SelectTarget(mov);
                else
                    s_battlesyst.GetInstance().SelectTarget(battleCharButton, mov);
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                if(isItem)
                    rpg_globals.gl.UseItem(mov.name);
                s_soundmanager.GetInstance().PlaySound("selectOption");
                break;
        }
    }
}
