using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2;
using MagnumFoundation2.System;

public class s_skillsMenu : s_menucontroller
{

    public enum BTN_TYPE
    {
        SKILL,
        ITEM
    }
    public BTN_TYPE BUTTON_TYPE;

    public override void OnOpen()
    {
        base.OnOpen(); int ind = 0;
        ResetButton();

        switch (BUTTON_TYPE)
        {
            case BTN_TYPE.ITEM:
                foreach (KeyValuePair<string, int> it in rpg_globals.gl.inventory)
                {
                    rpg_item item = rpg_globals.gl.GetItem(it.Key);
                    GetButton<s_skillButton>(ind).gameObject.SetActive(true);
                    GetButton<s_skillButton>(ind).txt.text = it.Key + " - " + it.Value;
                    GetButton<s_skillButton>(ind).item = item;
                    ind++;
                }
                break;

            case BTN_TYPE.SKILL:

                List<o_battleCharData> bcs = rpg_globals.gl.partyMembers;

                foreach (o_battleCharData bcD in bcs)
                {
                    foreach (s_move mv in bcD.currentMoves)
                    {
                        if (mv.moveType == MOVE_TYPE.STATUS)
                        {
                            if (mv.statusMoveType == STATUS_MOVE_TYPE.HEAL)
                            {
                                if (bcD.skillPoints < mv.cost)
                                {
                                    continue;
                                }
                                GetButton<s_skillButton>(ind).gameObject.SetActive(true);
                                GetButton<s_skillButton>(ind).txt.text = bcD.name + " - " + mv.name + " SP -" + mv.cost;
                                GetButton<s_skillButton>(ind).move = mv;
                                ind++;
                            }
                        }
                    }
                }
                break;
        }

    }
}
