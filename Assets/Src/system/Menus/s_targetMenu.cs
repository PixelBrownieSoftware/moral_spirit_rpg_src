using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_targetMenu : s_menucontroller
{
    public s_move move;
    public rpg_item item;
    public bool isItem = false;
    public s_button backButton;

    public enum TARGET_TYPE {
        RECOVERY,
        ASSIGN_SKILL,
        SKILL,
        ASSIGN_PARTY_MEMBER
    }
    public TARGET_TYPE targType;

    public override void OnOpen()
    {
        base.OnOpen();
        int ind = 0;
        ResetButton();
        List<o_battleCharData> bcs = rpg_globals.gl.partyMembers;
        switch (targType)
        {
            case TARGET_TYPE.ASSIGN_PARTY_MEMBER:

                foreach (o_battleCharData bcD in bcs)
                {
                    //SkillsMenu
                    s_targCharButton chrBTN = GetButton<s_targCharButton>(ind);
                    chrBTN.targType = s_targCharButton.BTN_TYPE.ASSIGN_PARTY_MEMBER;
                    if (bcD.inBattle)
                    {
                        chrBTN.buttonColour.color = Color.white;
                    }
                    else
                    {
                        chrBTN.buttonColour.color = Color.grey;
                    }
                    chrBTN.txt.text = bcD.name;
                    chrBTN.battleChar = bcD;
                    ind++;
                }
                break;
            case TARGET_TYPE.SKILL:

                foreach (o_battleCharData bcD in bcs)
                {
                    //SkillsMenu
                    s_targCharButton chrBTN = GetButton<s_targCharButton>(ind);
                    chrBTN.buttonColour.color = Color.white;
                    chrBTN.targType = s_targCharButton.BTN_TYPE.SKILL_LOOK;
                    chrBTN.txt.text = bcD.name;
                    chrBTN.battleChar = bcD;
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
                        if (s_battlesyst.GetInstance().playerCP >= s_battlesyst.GetInstance().playerCPMax)
                        {
                            continue;
                        }
                    }

                    GetButton<s_targCharButton>(ind);
                    if (isItem)
                    {
                        GetButton<s_targCharButton>(ind).targType = s_targCharButton.BTN_TYPE.ITEM;
                        GetButton<s_targCharButton>(ind).mov = move;
                        GetButton<s_targCharButton>(ind).battleChar = bcD;
                        GetButton<s_targCharButton>(ind).itemName = move.name;
                    }
                    else
                    {
                        GetButton<s_targCharButton>(ind).targType = s_targCharButton.BTN_TYPE.SKILL_USE;
                        GetButton<s_targCharButton>(ind).battleChar = bcD;
                        GetButton<s_targCharButton>(ind).mov = move;
                    }

                    if (move.statusMoveType == STATUS_MOVE_TYPE.HEAL)
                        GetButton<s_targCharButton>(ind).txt.text = bcD.name + " - " + bcD.hitPoints;
                    /*
                    if (move.statusMoveType == STATUS_MOVE_TYPE.HEAL)
                        GetButton<s_targCharButton>(ind).txt.text = bcD.name + " - " + bcD.hitPoints;
                    else
                        GetButton<s_targCharButton>(ind).txt.text = bcD.name + " - " + bcD.skillPoints;
                    */
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
