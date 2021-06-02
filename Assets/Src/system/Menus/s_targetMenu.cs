using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_targetMenu : s_menucontroller
{
    public s_move move;
    public rpg_item item;
    public bool isItem = false;

    public enum TARGET_TYPE {
        RECOVERY,
        ASSIGN_SKILL,
        SKILL
    }
    public TARGET_TYPE targType;

    public override void OnOpen()
    {
        base.OnOpen();
        List<o_battleCharData> bcs = rpg_globals.gl.partyMembers;
        int ind = 0;
        ResetButton();
        switch (targType)
        {
            case TARGET_TYPE.SKILL:

                foreach (o_battleCharData bcD in bcs)
                {
                    //SkillsMenu
                    GetButton<s_targCharButton>(ind);
                    GetButton<s_targCharButton>(ind).targType = s_targCharButton.BTN_TYPE.SKILL_LOOK;
                    GetButton<s_targCharButton>(ind).txt.text = bcD.name + " - " + bcD.skillPoints;
                    GetButton<s_targCharButton>(ind).battleChar = bcD;
                    ind++;
                }
                break;

            case TARGET_TYPE.RECOVERY:
                foreach (o_battleCharData bcD in bcs)
                {
                    if (move.statusMoveType == STATUS_MOVE_TYPE.HEAL)
                    {
                        if (bcD.hitPoints >= bcD.maxHitPoints)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (bcD.skillPoints >= bcD.maxSkillPoints)
                        {
                            continue;
                        }
                    }

                    GetButton<s_targCharButton>(ind);
                    if (isItem)
                    {
                        GetButton<s_targCharButton>(ind).targType = s_targCharButton.BTN_TYPE.ITEM;
                        GetButton<s_targCharButton>(ind).mov = item.action;
                        GetButton<s_targCharButton>(ind).itemName = item.name;
                    }
                    else
                    {
                        GetButton<s_targCharButton>(ind).targType = s_targCharButton.BTN_TYPE.SKILL_USE;
                        GetButton<s_targCharButton>(ind).mov = move;
                    }

                    if (move.statusMoveType == STATUS_MOVE_TYPE.HEAL)
                        GetButton<s_targCharButton>(ind).txt.text = bcD.name + " - " + bcD.hitPoints;
                    else
                        GetButton<s_targCharButton>(ind).txt.text = bcD.name + " - " + bcD.skillPoints;
                    GetButton<s_targCharButton>(ind).battleChar = bcD;
                    ind++;
                }
                break;

            case TARGET_TYPE.ASSIGN_SKILL:

                foreach (o_battleCharData bcD in bcs)
                {
                    GetButton<s_targCharButton>(ind);
                    GetButton<s_targCharButton>(ind).battleChar = bcD;
                    GetButton<s_targCharButton>(ind).txt.text = bcD.name;
                    ind++;
                }
                break;
        }
    }
}
