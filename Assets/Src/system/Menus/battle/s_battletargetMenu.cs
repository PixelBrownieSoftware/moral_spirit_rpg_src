using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.System;

public class s_battletargetMenu : s_menucontroller
{
    public enum SKILL_TYPE
    {
        BATTLE,
        GROUP_SELECT,
        DEPLOY_PARTY,
        ANALYZE_TEAM_OR_PARTY,
        ANALYZE_TARGETS
    }
    public SKILL_TYPE skillType;

    public s_move mov;
    public s_button back;
    
    public List<o_battleChar> bcs = new List<o_battleChar>();
    public bool isItem = false;

    public override void OnOpen()
    {
        base.OnOpen();
        ResetButton();
        s_targetButton tg = null;
        switch (skillType) {
            default:
                back.buttonType = "BattleMainMenu";
                switch (mov.target)
                {
                    case TARGET_MOVE_TYPE.SINGLE:
                        for (int i = 0; i < bcs.Count; i++)
                        {
                            if (mov.statusMoveType == STATUS_MOVE_TYPE.BUFF && mov.moveType == MOVE_TYPE.STATUS)
                            {
                                if (bcs[i].hitPoints <= 0)
                                    continue;
                            }
                            if (mov.excludeUser &&
                                bcs[i] == s_battlesyst.GetInstance().currentCharacter)
                            {
                                continue;
                            }
                            tg = GetButton<s_targetButton>(i);
                            tg.targType = s_targetButton.TARGET_TYPE.BATTLE;
                            tg.isItem = isItem;
                            tg.isAll = false;
                            tg.mov = mov;
                            tg.battleCharButton = bcs[i];
                            tg.GetMemory();
                            tg.transform.position = Camera.main.WorldToScreenPoint(bcs[i].transform.position);
                            tg.txt.text = bcs[i].name;
                        }
                        break;

                    case TARGET_MOVE_TYPE.RANDOM:
                    case TARGET_MOVE_TYPE.ALL:
                        for (int i = 0; i < bcs.Count; i++)
                        {
                            if (mov.statusMoveType == STATUS_MOVE_TYPE.BUFF && mov.moveType == MOVE_TYPE.STATUS)
                            {
                                if (bcs[i].hitPoints <= 0)
                                    continue;
                            }
                            if (mov.excludeUser &&
                                bcs[i] == s_battlesyst.GetInstance().currentCharacter)
                            {
                                continue;
                            }
                            tg = GetButton<s_targetButton>(i);
                            tg.targType = s_targetButton.TARGET_TYPE.BATTLE;
                            tg.isItem = isItem;
                            tg.mov = mov;
                            tg.battleCharButton = bcs[i];
                            tg.isAll = true;
                            tg.GetMemory(bcs[i]);
                            tg.transform.position = Camera.main.WorldToScreenPoint(bcs[i].transform.position);
                            tg.txt.text = bcs[i].name;
                        }
                        break;
                }
                break;
            case SKILL_TYPE.ANALYZE_TEAM_OR_PARTY:

                back.buttonType = "BattleMainMenu";
                //tg.transform.position = Camera.main.WorldToScreenPoint(s_camera.GetInstance().GetCentroid(bcs));
                tg = GetButton<s_targetButton>(0);
                tg.targType = s_targetButton.TARGET_TYPE.ANALYZE_TEAM_SELECT_PLAYERS;
                tg = GetButton<s_targetButton>(1);
                tg.targType = s_targetButton.TARGET_TYPE.ANALYZE_TEAM_SELECT_ENEMIES;
                break;

            case SKILL_TYPE.ANALYZE_TARGETS:
                back.buttonType = "BattleMainMenu";
                for (int i = 0; i < bcs.Count; i++)
                {
                    tg = GetButton<s_targetButton>(i);
                    tg.targType = s_targetButton.TARGET_TYPE.ANALYZE;
                    tg.isItem = isItem;
                    tg.isAll = false;
                    tg.mov = mov;
                    tg.battleCharButton = bcs[i];
                    tg.transform.position = Camera.main.WorldToScreenPoint(bcs[i].transform.position);
                    tg.txt.text = bcs[i].name;
                }
                break;
        }

    }
}