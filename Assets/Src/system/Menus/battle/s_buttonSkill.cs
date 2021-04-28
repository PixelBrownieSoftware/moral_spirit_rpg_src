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
    public rpg_item itemButton;
    public Image element; 
    public enum SKILL_TYPE {
        BATTLE,
        GROUP_SELECT
    }
    public SKILL_TYPE typeOfButton;
    public bool isComb = false;
    

    protected override void OnHover()
    {
        //BMenu.moveDescription.text = ""
        base.OnHover();
    }

    protected override void OnButtonClicked()
    {
        switch (typeOfButton) {
            case SKILL_TYPE.BATTLE:
                /*
                s_battleEngine.engineSingleton.SelectSkillOption(moveButton);
                s_menuhandler.GetInstance().GetMenu<s_targetMenu>
                ("TargetMenu").bcs = s_battleEngine.engineSingleton.GetTargets(moveButton.onParty);
                s_menuhandler.GetInstance().SwitchMenu("TargetMenu");
                s_soundmanager.GetInstance().PlaySound("selectOption");
                */
                break;
        }
        base.OnButtonClicked();
    }
}
