using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2;
using MagnumFoundation2.System;
using UnityEngine.UI;

public class s_skillsMenu : s_menucontroller
{
    public enum BTN_TYPE
    {
        SKILL,
        ITEM,
        ASSIGN_SKILL
    }
    public BTN_TYPE BUTTON_TYPE;
    public List<o_battleCharData> bcs = new List<o_battleCharData>();
    public int limit;
    public int page = 1;
    public o_battleCharData target;
    public s_statReq str;
    public s_statReq vit;
    public s_statReq dex;
    public s_statReq gut;
    public s_statReq agi;
    int count;
    public Text skillSlotsRemaining;

    private void Start()
    {
        count = buttons.Length;
    }

    private void Update()
    {
        switch (BUTTON_TYPE) {

            case BTN_TYPE.ASSIGN_SKILL:
                if (target != null)
                {
                    int skillsleft = rpg_globals.gl.extraSkillAmount - target.extra_skills.Count;
                    if (skillsleft > 0)
                    {
                        skillSlotsRemaining.text = "" + skillsleft + " extra skill slots remaining";
                    }
                    else
                    {
                        skillSlotsRemaining.text = "No skill slots remaining.";
                    }
                }
                break;
        }
    }

    public void ChangeRequirements(s_move mov) {
        str.statNum = target.attack;
        vit.statNum = target.defence;
        dex.statNum = target.intelligence;
        agi.statNum = target.speed;
        gut.statNum = target.guts;

        str.requirementNum = mov.str_req;
        vit.requirementNum = mov.vit_req;
        dex.requirementNum = mov.dex_req;
        agi.requirementNum = mov.agi_req;
        gut.requirementNum = mov.gut_req;

        if (str.requirementNum > 0)
            str.gameObject.SetActive(true);
        else
            str.gameObject.SetActive(false);

        if (vit.requirementNum > 0)
            vit.gameObject.SetActive(true);
        else
            vit.gameObject.SetActive(false);

        if (dex.requirementNum > 0)
            dex.gameObject.SetActive(true);
        else
            dex.gameObject.SetActive(false);

        if (agi.requirementNum > 0)
            agi.gameObject.SetActive(true);
        else
            agi.gameObject.SetActive(false);

        if (gut.requirementNum > 0)
            gut.gameObject.SetActive(true);
        else
            gut.gameObject.SetActive(false);
    }

    public void TurnPage(bool forward) {
        if (forward)
        {
            if (page * limit > GetCount())
            {
                page++;
            }
        }
        else
        {
            if (page > 1)
            {
                page--;
            }
        }
    }

    public int GetCount()
    {
        int c = 0;
        foreach (o_battleCharData bcD in bcs)
        {
             c+= bcD.currentMoves.Count;
        }
        return c;
    }

    public override void OnOpen()
    {
        base.OnOpen();
        int ind = 0;
        ResetButton();

        switch (BUTTON_TYPE)
        {
            case BTN_TYPE.ITEM:
                foreach (KeyValuePair<string, int> it in rpg_globals.gl.inventory)
                {
                    s_move item = rpg_globals.gl.GetItem(it.Key);
                    GetButton<s_skillButton>(ind);
                    GetButton<s_skillButton>(ind).usable = true;
                    GetButton<s_skillButton>(ind).txt.text = it.Key + " - " + it.Value;
                    GetButton<s_skillButton>(ind).move = item;
                    GetButton<s_skillButton>(ind).BUTTON_TYPE = s_skillButton.BTN_TYPE.ITEM;
                    ind++;
                }
                break;

            case BTN_TYPE.SKILL:

                foreach (s_move mv in target.currentMoves)
                {
                    if (ind < page * limit)
                        continue;
                    GetButton<s_skillButton>(ind).gameObject.SetActive(true);
                    GetButton<s_skillButton>(ind).move = mv;
                    GetButton<s_skillButton>(ind).usable = false;
                    GetButton<s_skillButton>(ind).SetMenu(this);
                    GetButton<s_skillButton>(ind).txt.text = target.name + " - " + mv.name;
                    if (mv.moveType == MOVE_TYPE.STATUS)
                    {
                        if (mv.statusMoveType == STATUS_MOVE_TYPE.HEAL)
                        {
                            /*
                            if (bcD.skillPoints < mv.cost)
                            {
                                continue;
                            }
                            */
                            GetButton<s_skillButton>(ind).usable = true;
                            GetButton<s_skillButton>(ind).BUTTON_TYPE = s_skillButton.BTN_TYPE.SKILL;
                        }
                    }
                    ind++;
                }
                foreach (s_move mv in target.extra_skills)
                {
                    if (ind < page * limit)
                        continue;
                    GetButton<s_skillButton>(ind).gameObject.SetActive(true);
                    GetButton<s_skillButton>(ind).move = mv;
                    GetButton<s_skillButton>(ind).usable = false;
                    GetButton<s_skillButton>(ind).SetMenu(this);
                    GetButton<s_skillButton>(ind).txt.text = target.name + " - " + mv.name;
                    if (mv.moveType == MOVE_TYPE.STATUS)
                    {
                        if (mv.statusMoveType == STATUS_MOVE_TYPE.HEAL)
                        {
                            /*
                            if (bcD.skillPoints < mv.cost)
                            {
                                continue;
                            }
                            */
                            GetButton<s_skillButton>(ind).usable = true;
                            GetButton<s_skillButton>(ind).BUTTON_TYPE = s_skillButton.BTN_TYPE.SKILL;
                        }
                    }
                    ind++;
                }
                break;

            case BTN_TYPE.ASSIGN_SKILL:

                bcs = rpg_globals.gl.partyMembers;

                foreach (s_move mv in rpg_globals.gl.extraSkills)
                {
                    o_battleCharData naturalAbi = bcs.Find(x => x.currentMoves.Contains(mv));
                    if (naturalAbi != null) {

                        //print(naturalAbi.name);
                        if (naturalAbi == target)
                            continue;
                    }

                    o_battleCharData di = bcs.Find(x => x.extra_skills.Contains(mv));
                    if (di != null)
                    {
                        //print(di.name);
                        if(di != target)
                            continue;
                    }
                    GetButton<s_skillButton>(ind).move = mv;
                    ind++;
                }
                break;
        }

    }
}
