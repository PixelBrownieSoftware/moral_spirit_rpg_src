using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoundation2.System.Core;

public class s_targCharButton : s_button
{
    public o_battleCharData battleChar;
    public s_move mov;
    public string itemName;
    public enum BTN_TYPE
    {
        SKILL_USE,
        ITEM,
        ASSIGN_SKILL,
        SKILL_LOOK,
        ASSIGN_PARTY_MEMBER
    }
    public BTN_TYPE targType;
    public Image buttonColour;

    protected override void OnButtonClicked()
    {
        switch (targType) {
            case BTN_TYPE.ITEM:
                rpg_globals.gl.UseItem(mov.name);
                switch (mov.statusMoveType)
                {
                    case STATUS_MOVE_TYPE.HEAL:
                        battleChar.hitPoints += mov.power;
                        battleChar.hitPoints = Mathf.Clamp(battleChar.hitPoints, 0, battleChar.maxHitPoints);
                        s_soundmanager.GetInstance().PlaySound("healsound2");
                        break;

                        /*
                    case STATUS_MOVE_TYPE.HEAL_STAMINA:
                        if (s_battlesyst.GetInstance().players.Contains(battleChar)) {
                            float maxCP = s_battlesyst.GetInstance().playerCPMax;

                            s_battlesyst.GetInstance().playerCP += mov.power;
                            s_battlesyst.GetInstance().playerCP = Mathf.Clamp(s_battlesyst.GetInstance().playerCP, 0, maxCP);
                        }
                        s_soundmanager.GetInstance().PlaySound("spHealSound");
                        break;
                        */
                }
                break;

            case BTN_TYPE.ASSIGN_PARTY_MEMBER:
                if (battleChar.inBattle)
                {
                    if(rpg_globals.gl.partyMembers.FindAll(x => x.inBattle).Count > 1)
                        battleChar.inBattle = false;
                }
                else
                {
                    if (rpg_globals.gl.CheckPartyMemberBounds())
                    {
                        battleChar.inBattle = true;
                    }
                }
                base.OnButtonClicked();
                break;

            case BTN_TYPE.SKILL_LOOK:
                s_menuhandler.GetInstance().GetMenu<s_skillsMenu>("SkillsMenu").target = battleChar;
                base.OnButtonClicked();
                break;

            case BTN_TYPE.SKILL_USE:

                s_battlesyst.GetInstance().playerCP -= mov.cost;
                battleChar.hitPoints += (int)((float)mov.power * (float)(battleChar.intelligence / 4));

                battleChar.hitPoints = Mathf.Clamp(battleChar.hitPoints, 0, battleChar.maxHitPoints);
                break;

            case BTN_TYPE.ASSIGN_SKILL:

                s_menuhandler.GetInstance().GetMenu<s_skillsMenu>("ExtraSkillsMenu").target = battleChar;
                base.OnButtonClicked();
                break;
        }
       // 
    }

    private void Update()
    {
        switch (targType)
        {
            case BTN_TYPE.SKILL_USE:
            if (s_battlesyst.GetInstance().playerCP < mov.cost)
            {
                gameObject.SetActive(false);
            }
                break;
        }
        switch (targType)
        {
            case BTN_TYPE.ASSIGN_PARTY_MEMBER:
                if (battleChar.inBattle)
                {
                    buttonColour.color = Color.white;
                }
                else
                {
                    buttonColour.color = Color.grey;
                }
                break;
            case BTN_TYPE.SKILL_USE:
            case BTN_TYPE.ITEM:
                switch (mov.statusMoveType)
                {
                    case STATUS_MOVE_TYPE.HEAL:
                        if (battleChar.hitPoints >= battleChar.maxHitPoints)
                        {
                            gameObject.SetActive(false);
                        }
                        else
                        {
                            gameObject.SetActive(true);
                        }
                        break;

                    case STATUS_MOVE_TYPE.HEAL_STAMINA:
                        if (s_battlesyst.GetInstance().playerCP >= battleChar.maxSkillPoints)
                        {
                            gameObject.SetActive(false);
                        }
                        else
                        {
                            gameObject.SetActive(true);
                        }
                        break;
                }
                break;
        }
        if (battleChar != null)
        {
            if (battleChar.name != "")
            {
                if (mov != null)
                {
                        txt.text = battleChar.name + " HP: " + battleChar.hitPoints;
                }
            }
        }
    }
}
