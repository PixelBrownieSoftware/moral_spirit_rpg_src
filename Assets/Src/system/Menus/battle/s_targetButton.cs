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
        BATTLE
    }
    public TARGET_TYPE targType;
    public s_move mov;

    public Slider HPSlider;
    public Slider SPSlider;

    public Image weaknessIcon;

    public Sprite normDmg;
    public Sprite weakDmg;
    public Sprite resDmg;

    public bool isAll = false;

    protected override void OnHover()
    {
        base.OnHover();
        switch (targType)
        {
            case TARGET_TYPE.BATTLE:
                print("HEY");
                s_soundmanager.GetInstance().PlaySound("cursor_move");
                //s_camera.cam.SetTargPos(battleCharButton.transform.position);
                break;
        }
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
            if (mov.element != ELEMENT.UNKNOWN) {
                switch (mov.moveType) {
                    case MOVE_TYPE.PHYSICAL:
                    case MOVE_TYPE.SPECIAL:

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
                        break;

                    case MOVE_TYPE.TALK:

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
                        break;
                }
            }
        }
    }

    protected override void OnButtonClicked()
    {
        switch (targType) {
            case TARGET_TYPE.BATTLE:
                switch (mov.moveType) {
                    case MOVE_TYPE.PHYSICAL:
                        if (s_battlesyst.GetInstance().currentCharacter.hitPoints <= mov.cost) {

                            return;
                        }
                            break;
                    case MOVE_TYPE.SPECIAL:
                    case MOVE_TYPE.TALK:
                    case MOVE_TYPE.STATUS:
                        if (s_battlesyst.GetInstance().currentCharacter.skillPoints < mov.cost)
                        {
                            return;
                        }
                        break;
                }
                if (isAll)
                    s_battlesyst.GetInstance().SelectTarget(mov);
                else
                    s_battlesyst.GetInstance().SelectTarget(battleCharButton, mov);
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                s_soundmanager.GetInstance().PlaySound("selectOption");
                break;
        }
    }
}
