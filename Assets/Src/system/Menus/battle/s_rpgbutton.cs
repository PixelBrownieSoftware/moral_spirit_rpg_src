using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoundation2.System.Core;
using MagnumFoundation2.System;

[RequireComponent(typeof(Button))]
public class s_rpgbutton : s_button
{
    public s_button backButton;
    public string buttonT;
    
    protected override void OnButtonClicked()
    {
        switch (buttonT) {
            case "fight":
                /*
                backButton.buttonType = "BattleMenu";
                s_battleEngine.engineSingleton.SelectSkillOption();
                s_menuhandler.GetInstance().GetMenu<s_targetMenu>
                ("TargetMenu").bcs = s_battleEngine.engineSingleton.GetTargets(false);
                s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").mov 
                    = s_battleEngine.engineSingleton.defaultAttack;

                s_menuhandler.GetInstance().SwitchMenu("TargetMenu");
                s_soundmanager.GetInstance().PlaySound("selectOption");
                */
                break;

            case "skill":
                backButton.buttonType = "BattleMainMenu";
                s_menuhandler.GetInstance().GetMenu<s_battleMenu>("BattleMenuSkillsItemsGUI").rpgSkills = 
                    s_battlesyst.GetInstance().currentCharacter.skillMoves;
                s_menuhandler.GetInstance().SwitchMenu("BattleMenuSkillsItemsGUI");
                s_soundmanager.GetInstance().PlaySound("selectOption");
                break;
                
            case "item":
            case "party":
            case "guard":
                s_soundmanager.GetInstance().PlaySound("selectOption");
                base.OnButtonClicked();
                break;

            case "pass":
                //s_battleEngine.engineSingleton.battleAction.type = s_battleEngine.s_battleAction.MOVE_TYPE.PASS;
                s_battlesyst.GetInstance().EndAction();
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                s_soundmanager.GetInstance().PlaySound("selectOption");
                break;

            case "back":
                s_soundmanager.GetInstance().PlaySound("back");
                base.OnButtonClicked();
                break;
                
        }
    }

}
