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
        DEPLOY_PARTY
    }
    public SKILL_TYPE skillType;

    public s_move mov;
    
    public List<o_battleChar> bcs = new List<o_battleChar>();

    public override void OnOpen()
    {
        base.OnOpen();
        ResetButton();
        s_targetButton tg = null;

        switch (mov.target)
        {
            case TARGET_MOVE_TYPE.SINGLE:
                for (int i = 0; i < bcs.Count; i++)
                {
                    if (mov.excludeUser && 
                        bcs[i] == s_battlesyst.GetInstance().currentCharacter) {
                        continue;
                    }
                    tg = GetButton<s_targetButton>(i);
                    tg.isAll = false;
                    tg.mov = mov;
                    tg.battleCharButton = bcs[i];
                    tg.transform.position = Camera.main.WorldToScreenPoint(bcs[i].transform.position);
                    tg.txt.text = bcs[i].name;
                }
                break;

            case TARGET_MOVE_TYPE.ALL:
                for (int i = 0; i < bcs.Count; i++)
                {
                    if (mov.excludeUser &&
                        bcs[i] == s_battlesyst.GetInstance().currentCharacter)
                    {
                        continue;
                    }
                    tg = GetButton<s_targetButton>(i);
                    tg.isAll = true;
                    tg.mov = mov;
                    tg.transform.position = Camera.main.WorldToScreenPoint(bcs[i].transform.position);
                    tg.txt.text = bcs[i].name;
                }
                break;
        }

    }
}