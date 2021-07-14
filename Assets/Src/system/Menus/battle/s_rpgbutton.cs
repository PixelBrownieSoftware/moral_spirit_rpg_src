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
    public s_skillsMenu menuSkill;
    
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
                backButton.buttonType = "BattleMainMenu";
                s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>
                ("BattleMenuTarget").bcs = new List<o_battleChar>();
                foreach (o_battleChar bc in s_battlesyst.GetInstance().opposition)
                {
                    if (bc.hitPoints > 0)
                        s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>("BattleMenuTarget").bcs.Add(bc);
                    else
                        continue;
                }
                s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>
                ("BattleMenuTarget").mov = s_battlesyst.GetInstance().normalAttack;
                base.OnButtonClicked();
                break;

            case "skill":
                backButton.buttonType = "BattleMainMenu";
                s_menuhandler.GetInstance().GetMenu<s_battleMenu>("BattleMenuSkillsItemsGUI").rpgSkills = 
                    s_battlesyst.GetInstance().currentCharacter.skillMoves;
                s_menuhandler.GetInstance().GetMenu<s_battleMenu>("BattleMenuSkillsItemsGUI").isItem = false;
                s_menuhandler.GetInstance().SwitchMenu("BattleMenuSkillsItemsGUI");
                s_soundmanager.GetInstance().PlaySound("selectOption");
                break;
                
            case "item":
                backButton.buttonType = "BattleMainMenu";
                s_menuhandler.GetInstance().GetMenu<s_battleMenu>("BattleMenuSkillsItemsGUI").isItem = true;
                s_menuhandler.GetInstance().GetMenu<s_battleMenu>("BattleMenuSkillsItemsGUI").items =
                    rpg_globals.gl.GetInventory();
                s_soundmanager.GetInstance().PlaySound("selectOption");
                base.OnButtonClicked();
                break;

            case "guard":
                s_soundmanager.GetInstance().PlaySound("selectOption");
                s_battlesyst.GetInstance().SelectTarget(s_battlesyst.GetInstance().guardMove);
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                break;

            case "pass":
                //s_battleEngine.engineSingleton.battleAction.type = s_battleEngine.s_battleAction.MOVE_TYPE.PASS;
                s_battlesyst.GetInstance().turnPressFlag = PRESS_TURN_TYPE.PASS;
                s_battlesyst.GetInstance().CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_PROCESS;
                s_battlesyst.GetInstance().EndAction();
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                s_soundmanager.GetInstance().PlaySound("selectOption");
                break;

            case "spare":
                s_battlesyst.GetInstance().CurrentBattleEngineState = BATTLE_ENGINE_STATE.NEGOTIATE_MENU;
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                break;

            case "run":
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                s_battlesyst.GetInstance().RunTurn();
                break;

            case "back":
                s_soundmanager.GetInstance().PlaySound("back");
                base.OnButtonClicked();
                break;

            case "turnPageF":
                menuSkill.TurnPage(true);
                break;

            case "turnPageB":
                menuSkill.TurnPage(false);
                break;

            case "skillMenu":
                s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").targType = s_targetMenu.TARGET_TYPE.SKILL;
                s_menuhandler.GetInstance().SwitchMenu("TargetMenu");
                break;

        }
    }

}
