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
    public s_mainBattleMenu battleMenMain;
    float _CP = 0;
    float _MaxCP = 0;

    protected override void OnHover()
    {
        base.OnHover();
        if (battleMenMain != null) {
            string txt = "";
            switch (buttonT)
            {
                case "analyze":
                    txt = "Analyse - View stats of both sides.";
                    break;
                case "skill":
                    txt = "Act - Talk to the target.";
                    break;
                case "item":
                    txt = "Item - Use an item";
                    break;
                case "guard":
                    txt = "Guard - Take less resolve damage and negate weaknesses.";
                    break;
                case "pass":
                    txt = "Pass - Your turn goes to the next character without consuming a full turn icon.";
                    break;
            }
            switch (buttonT)
            {
                default:
                    battleMenMain.ChangeDesc(txt, false);
                    break;
                case "run":
                    battleMenMain.ChangeDesc(txt,true);
                    break;
            }
        }
    }

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
                backButton.buttonType = "BattleMainMenu";
                base.OnButtonClicked();
                break;

            case "skill":
                backButton.buttonType = "BattleMainMenu";
                s_menuhandler.GetInstance().GetMenu<s_battleMenu>("BattleMenuSkillsItemsGUI").rpgSkills = 
                    s_battlesyst.GetInstance().currentCharacter.allSkills;
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
                s_battlesyst.GetInstance().finalTurnPressFlag = PRESS_TURN_TYPE.PASS;
                s_battlesyst.GetInstance().CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_PROCESS;
                s_battlesyst.GetInstance().EndAction();
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                s_soundmanager.GetInstance().PlaySound("selectOption");
                break;

            case "analyze":
                s_menuhandler.GetInstance().GetMenu<s_battletargetMenu>("BattleMenuTarget").skillType = s_battletargetMenu.SKILL_TYPE.ANALYZE_TEAM_OR_PARTY;
                base.OnButtonClicked();
                break;

            case "spare":
                s_battlesyst.GetInstance().CurrentBattleEngineState = BATTLE_ENGINE_STATE.NEGOTIATE_MENU;
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                break;

            case "run":
                _MaxCP = s_battlesyst.GetInstance().playerCPMax;
                _CP = s_battlesyst.GetInstance().playerCP;
                if (_CP >= (_MaxCP/2))
                {
                    s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                    s_battlesyst.GetInstance().RunTurn();
                }
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
