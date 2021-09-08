using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoundation2;

public class s_skillButton : s_button
{
    public s_move move;
    public Image img;
    public Image skillButtonImg;
    public Text cost;
    public bool usable = false;
    public bool taken = false;

    public Sprite fire_picture;
    public Sprite water_picture;
    public Sprite ice_picture;
    public Sprite electric_picture;
    public Sprite wind_picture;
    public Sprite earth_picture;
    public Sprite dark_picture;
    public Sprite light_picture;
    public Sprite poison_picture;
    public Sprite psychic_picture;
    public Sprite strike_picture;
    public Sprite force_picture;
    public Sprite perice_picture;

    public Sprite threaten_picture;
    public Sprite flirt_picture;
    public Sprite comfort_picture;
    public Sprite reserved_picture;
    public Sprite playful_picture;

    public Sprite heal_picture;
    public Sprite support_picture;

    s_skillsMenu men;

    public enum BTN_TYPE {
        SKILL,
        ITEM,
        ASSIGN_SKILL
    }
    public BTN_TYPE BUTTON_TYPE;

    public void SetMenu(s_skillsMenu menu) {
        men = menu;
    }

    protected new void Start()
    {
        base.Start();
        switch (BUTTON_TYPE) {
            case BTN_TYPE.ASSIGN_SKILL:
                men = s_menuhandler.GetInstance().GetMenu<s_skillsMenu>("ExtraSkillsMenu");
                break;
        }
    }

    protected override void OnHover()
    {
        switch (BUTTON_TYPE)
        {
            case BTN_TYPE.ASSIGN_SKILL:
                men.ChangeRequirements(move);
                break;
        }
        base.OnHover();
    }

    public void DrawButton()
    {
        if (move != null)
        {
            txt.text = "" + move.name;
            switch (move.moveType)
            {
                case MOVE_TYPE.PHYSICAL:
                case MOVE_TYPE.SPECIAL:
                    switch (move.element)
                    {
                        case ELEMENT.NORMAL:
                            img.sprite = strike_picture;
                            break;

                        case ELEMENT.FIRE:
                            img.sprite = fire_picture;
                            break;

                        case ELEMENT.ICE:
                            img.sprite = ice_picture;
                            break;

                        case ELEMENT.ELECTRIC:
                            img.sprite = electric_picture;
                            break;

                        case ELEMENT.WIND:
                            img.sprite = wind_picture;
                            break;

                        case ELEMENT.EARTH:
                            img.sprite = earth_picture;
                            break;

                        case ELEMENT.DARK:
                            img.sprite = dark_picture;
                            break;

                        case ELEMENT.PSYCHIC:
                            img.sprite = psychic_picture;
                            break;

                        case ELEMENT.PEIRCE:
                            img.sprite = perice_picture;
                            break;

                        case ELEMENT.FORCE:
                            img.sprite = force_picture;
                            break;
                    }
                    break;
                case MOVE_TYPE.TALK:
                    switch (move.action_type)
                    {
                        case ACTION_TYPE.COMFORT:
                            img.sprite = comfort_picture;
                            break;

                        case ACTION_TYPE.FLIRT:
                            img.sprite = flirt_picture;
                            break;

                        case ACTION_TYPE.PLAYFUL:
                            img.sprite = playful_picture;
                            break;

                        case ACTION_TYPE.THREAT:
                            img.sprite = threaten_picture;
                            break;

                        case ACTION_TYPE.RESERVED:
                            img.sprite = reserved_picture;
                            break;
                    }
                    break;
                case MOVE_TYPE.STATUS:

                    switch (move.statusMoveType)
                    {
                        case STATUS_MOVE_TYPE.BUFF:
                            img.sprite = support_picture;
                            break;

                        case STATUS_MOVE_TYPE.HEAL_STAMINA:
                        case STATUS_MOVE_TYPE.HEAL:
                            img.sprite = heal_picture;
                            break;
                    }
                    break;
            }
            switch (move.moveType)
            {
                case MOVE_TYPE.PHYSICAL:
                    cost.text = "" + Mathf.RoundToInt(((float)move.cost / 100) * men.target.maxHitPoints) + " <color=#05E5B6>HP</color> " + men.target.name;
                    break;
                case MOVE_TYPE.STATUS:
                case MOVE_TYPE.TALK:
                case MOVE_TYPE.SPECIAL:
                    cost.text = move.cost + " <color=#E705A3>SP</color> " + men.target.name;
                    break;
            }
        }
    }

    private void Update()
    {
        switch (BUTTON_TYPE)
        {
            case BTN_TYPE.ASSIGN_SKILL:

                o_battleCharData d = s_menuhandler.GetInstance().GetMenu<s_skillsMenu>("ExtraSkillsMenu").target;
                if (SkillReq(d))
                {
                    if (hasSkill(d))
                    {
                        skillButtonImg.color = Color.green;
                    }
                    else
                    {
                        skillButtonImg.color = Color.white;
                    }
                }
                else
                {
                    if (d.extra_skills.Find(x => x == move) == null)
                        skillButtonImg.color = Color.grey + new Color(0,0,0,-0.5f);
                    else
                        skillButtonImg.color = Color.grey;
                }
                if (d.extra_skills.Count >= rpg_globals.gl.extraSkillAmount)
                {
                    if (hasSkill(d))
                        skillButtonImg.color = Color.green + new Color(0, 0, 0, -0.5f);
                    else
                        skillButtonImg.color = Color.grey + new Color(0, 0, 0, -0.5f);
                }
                DrawButton();
                break;
            case BTN_TYPE.SKILL:
                DrawButton();
                break;
        }
    }

    public bool hasSkill(o_battleCharData d) {
        if (d.extra_skills.Find(x => x == move)) {
            return true;
        }
        return false;
    }

    public bool SkillReq(o_battleCharData d)
    {
        if (d.currentMoves.Find(x => x.name == move.name) != null)
            return false;

        if (d.attack >= move.str_req
        && d.defence >= move.vit_req
        && d.intelligence >= move.dex_req
        && d.speed >= move.agi_req
        && d.guts >= move.gut_req)
            return true;
        else
            return false;
    }

    public bool isGoodForAssignment(o_battleCharData d) {

        if (d.extra_skills.Count >= rpg_globals.gl.extraSkillAmount)
            return false;

        if (d.currentMoves.Find(x => x.name == move.name) != null)
            return false;

        if (d.attack >= move.str_req
        && d.defence >= move.vit_req
        && d.intelligence >= move.dex_req
        && d.speed >= move.agi_req
        && d.guts >= move.gut_req)
            return true;
        else
            return false;
    }

    protected override void OnButtonClicked()
    {
        switch (BUTTON_TYPE)
        {
            case BTN_TYPE.ASSIGN_SKILL:
                o_battleCharData d = s_menuhandler.GetInstance().GetMenu<s_skillsMenu>("ExtraSkillsMenu").target;
                if (isGoodForAssignment(d))
                {
                    if (d.extra_skills.Find(x => x == move) == null)
                    {
                        d.extra_skills.Add(move);
                    }
                    else
                    {
                        d.extra_skills.Remove(move);
                    }
                }
                else {
                    if (d.extra_skills.Find(x => x == move) != null) {
                        d.extra_skills.Remove(move);
                    }
                }
                break;
        }
        if (usable)
        {
            switch (BUTTON_TYPE)
            {
                case BTN_TYPE.SKILL:
                    s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").move = move;
                    s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").isItem = false;
                    s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").targType = s_targetMenu.TARGET_TYPE.SKILL;
                    s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").backButton.buttonType = "SkillsMenu";
                    break;

                case BTN_TYPE.ITEM:
                    s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").move = move;
                    s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").isItem = true;
                    s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").targType = s_targetMenu.TARGET_TYPE.RECOVERY;
                    s_menuhandler.GetInstance().GetMenu<s_targetMenu>("TargetMenu").backButton.buttonType = "ItemsMenu";
                    s_menuhandler.GetInstance().SwitchMenu("TargetMenu");
                    break;
            }
            base.OnButtonClicked();
        }
    }
}
