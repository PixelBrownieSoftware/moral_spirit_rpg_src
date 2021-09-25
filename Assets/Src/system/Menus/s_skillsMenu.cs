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
    bool hasMove = false;
    public Text skillSlotsRemaining;
    public Text description;

    private void Start()
    {
        count = buttons.Length;
    }

    private void Update()
    {
        switch (BUTTON_TYPE) {

            case BTN_TYPE.ASSIGN_SKILL:
                if (!hasMove) {
                    str.statNum = 0;
                    vit.statNum = 0;
                    dex.statNum = 0;
                    agi.statNum = 0;
                    gut.statNum = 0;
                    str.gameObject.SetActive(false);
                    vit.gameObject.SetActive(false);
                    dex.gameObject.SetActive(false);
                    agi.gameObject.SetActive(false);
                    gut.gameObject.SetActive(false);
                }

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
    public void SetDescription(s_move move)
    {

        string damageSize = "";
        string statusEff = "";
        string elementT = "";
        string actionT = "";
        string moveT = "";

        List<string> smallInc = new List<string>();
        List<string> moderateInc = new List<string>();
        List<string> largeInc = new List<string>();
        List<string> sharpInc = new List<string>();

        List<string> smallDec = new List<string>();
        List<string> moderateDec = new List<string>();
        List<string> largeDec = new List<string>();
        List<string> sharpDec = new List<string>();

        string damage = "";

        if (move.power < 9)
        {
            damageSize = "Small";
        }
        else if (move.power >= 9 && move.power < 21)
        {
            damageSize = "Moderate";
        }
        else if (move.power >= 21 && move.power < 35)
        {
            damageSize = "Large";
        }
        if (move.power >= 35)
        {
            damageSize = "Severe";
        }

        elementT = move.element.ToString();
        actionT = move.action_type.ToString();
        
        #region BUFFS
        switch (move.statusMoveType)
        {
            case STATUS_MOVE_TYPE.DEBUFF:
            case STATUS_MOVE_TYPE.BUFF:
                //Dexterity
                {
                    string letter = "dexterity";
                    if (move.dex_inc > 0 && move.dex_inc <= 1)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.dex_inc > 1 && move.dex_inc <= 2)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.dex_inc > 2 && move.dex_inc <= 3)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.dex_inc > 3)
                    {
                        sharpInc.Add(letter);
                    }
                }
                //Strength
                {
                    string letter = "strength";
                    if (move.str_inc > 0 && move.str_inc <= 1)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.str_inc > 1f && move.str_inc <= 2)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.str_inc > 2 && move.str_inc <= 3)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.str_inc > 3)
                    {
                        sharpInc.Add(letter);
                    }
                }
                //Vitality
                {
                    string letter = "vitality";
                    if (move.vit_inc > 0 && move.vit_inc <= 1)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.vit_inc > 1 && move.vit_inc <= 2)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.vit_inc > 2 && move.vit_inc <= 3)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.vit_inc > 3)
                    {
                        sharpInc.Add(letter);
                    }
                }
                //Guts
                {
                    string letter = "guts";
                    if (move.gut_inc > 0 && move.gut_inc <= 1)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.gut_inc > 1 && move.gut_inc <= 2)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.gut_inc > 2 && move.gut_inc <= 3)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.gut_inc > 3)
                    {
                        sharpInc.Add(letter);
                    }
                }
                //Agility
                {
                    string letter = "agility";
                    if (move.agi_inc > 0 && move.agi_inc <= 1)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.agi_inc > 1 && move.agi_inc <= 2)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.agi_inc > 2 && move.agi_inc <= 3)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.agi_inc > 3)
                    {
                        sharpInc.Add(letter);
                    }
                }
                break;
        }
        #endregion


        switch (move.statusMoveType)
        {
            case STATUS_MOVE_TYPE.DEBUFF:
                if (smallDec.Count > 0)
                {
                    statusEff += "Slightly decreases ";
                    foreach (string str in smallDec)
                    {
                        statusEff += str + ",";
                    }
                }
                if (moderateDec.Count > 0)
                {
                    statusEff += "Moderately decreases ";
                    foreach (string str in moderateDec)
                    {
                        statusEff += str + ",";
                    }
                }
                if (largeDec.Count > 0)
                {
                    statusEff += "Largely decreases ";
                    foreach (string str in largeDec)
                    {
                        statusEff += str + ",";
                    }
                }
                if (sharpDec.Count > 0)
                {
                    statusEff += "Sharply decreases ";
                    foreach (string str in sharpDec)
                    {
                        statusEff += str + ",";
                    }
                }
                break;
        }
        switch (move.statusMoveType)
        {
            case STATUS_MOVE_TYPE.BUFF:
                if (smallInc.Count > 0)
                {
                    statusEff += "Slightly increases ";
                    foreach (string str in smallInc)
                    {
                        statusEff += str + ",";
                    }
                }
                if (moderateInc.Count > 0)
                {
                    statusEff += "Moderately increases ";
                    foreach (string str in moderateInc)
                    {
                        statusEff += str + ",";
                    }
                }
                if (largeInc.Count > 0)
                {
                    statusEff += "Largely increases ";
                    foreach (string str in largeInc)
                    {
                        statusEff += str + ",";
                    }
                }
                if (sharpInc.Count > 0)
                {
                    statusEff += "Sharply increases ";
                    foreach (string str in sharpInc)
                    {
                        statusEff += str + ",";
                    }
                }
                break;
        }

        switch (move.moveType)
        {
            case MOVE_TYPE.STATUS:
                damage = moveT;
                break;

            case MOVE_TYPE.TALK:
                damage = damageSize + " " + actionT + " " + moveT + " damage.";
                break;
        }

        description.text = move.infoDescription + "\n" + damage + "\n" + statusEff;
    }

    public void ChangeRequirements(s_move mov) {
        hasMove = true;
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
        SetDescription(mov);

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
                    GetButton<s_skillButton>(ind).cost.text = "" + it.Value;
                    GetButton<s_skillButton>(ind).txt.text = it.Key;
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
