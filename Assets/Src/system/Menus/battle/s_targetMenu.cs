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
        ResetButton();
        base.OnOpen();
        switch (skillType) {
            case SKILL_TYPE.BATTLE:
                s_targetButton tg = null;
                switch (mov.target)
                {
                    case TARGET_MOVE_TYPE.SINGLE:
                        for (int i = 0; i < bcs.Count; i++)
                        {
                            tg = GetButton<s_targetButton>(i);

                            tg.battleCharButton = bcs[i];
                            tg.transform.position = Camera.main.WorldToScreenPoint(bcs[i].transform.position);
                            tg.txt.text = bcs[i].name;
                        }
                        break;

                    case TARGET_MOVE_TYPE.ALL:
                        tg = GetButton<s_targetButton>(0);
                        tg.txt.text = "All";
                        tg.gameObject.SetActive(true);
                        break;
                }
                break;
        }

    }
}