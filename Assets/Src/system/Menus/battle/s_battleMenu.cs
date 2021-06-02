using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2;
using UnityEngine.UI;

public class s_battleMenu : s_menucontroller
{
    public List<s_move> rpgSkills = new List<s_move>();
    public Dictionary<rpg_item, int> items = new Dictionary<rpg_item, int>();
    public Text moveDescription;

    public Color strColour;
    public Color dexColour;
    public Color vitColour;
    public Color agiColour;
    public Color gutColour;

    public Color fireColour;
    public Color iceColour;
    public Color electricColour;
    public Color windColour;
    public Color earthColour;
    public Color psychicColour;
    public Color darkColour;
    public Color lightColour;
    public Color strikeColour;
    public Color forceColour;
    public Color peirceColour;

    public Color threatColour;
    public Color comfortColour;
    public Color flirtColour;
    public Color playfulColour;

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

    public Sprite threat_picture;
    public Sprite comfort_picture;
    public Sprite flirt_picture;
    public Sprite playful_picture;

    public Sprite heal_picture;
    public Sprite support_picture;

    public bool isItem = false;

    public void SetDescription(s_move move) {

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
        else if (move.power >= 21 && move.power < 28)
        {
            damageSize = "Large";
        }
        if (move.power >= 28)
        {
            damageSize = "Severe";
        }
        
        elementT = move.element.ToString();
        actionT = move.action_type.ToString();

        switch (move.element) {
            case ELEMENT.FIRE:
                elementT = ReturnColouredText(elementT, fireColour);
                break;
            case ELEMENT.ICE:
                elementT = ReturnColouredText(elementT, iceColour);
                break;
            case ELEMENT.ELECTRIC:
                elementT = ReturnColouredText(elementT, electricColour);
                break;
            case ELEMENT.EARTH:
                elementT = ReturnColouredText(elementT, earthColour);
                break;
            case ELEMENT.WIND:
                elementT = ReturnColouredText(elementT, windColour);
                break;
            case ELEMENT.LIGHT:
                elementT = ReturnColouredText(elementT, lightColour);
                break;
            case ELEMENT.DARK:
                elementT = ReturnColouredText(elementT, darkColour);
                break;
            case ELEMENT.PSYCHIC:
                elementT = ReturnColouredText(elementT, psychicColour);
                break;
            case ELEMENT.FORCE:
                elementT = ReturnColouredText(elementT, forceColour);
                break;
            case ELEMENT.NORMAL:
                elementT = ReturnColouredText(elementT, strikeColour);
                break;
            case ELEMENT.PEIRCE:
                elementT = ReturnColouredText(elementT, peirceColour);
                break;
        }
        switch (move.action_type)
        {
            case ACTION_TYPE.COMFORT:
                actionT = ReturnColouredText(actionT, comfortColour);
                break;

            case ACTION_TYPE.THREAT:
                actionT = ReturnColouredText(actionT, threatColour);
                break;

            case ACTION_TYPE.PLAYFUL:
                actionT = ReturnColouredText(actionT, playfulColour);
                break;

            case ACTION_TYPE.FLIRT:
                actionT = ReturnColouredText(actionT, flirtColour);
                break;
        }

        switch (move.moveType) {
            case MOVE_TYPE.PHYSICAL:
                moveT = "physical";
                break;

            case MOVE_TYPE.SPECIAL:
                moveT = "special";
                break;

            case MOVE_TYPE.TALK:
                moveT = "talk";
                break;
        }

        #region INCREASE
        switch (move.statusMoveType) {
            case STATUS_MOVE_TYPE.BUFF:
                //Dexterity
                {
                    string letter = ReturnColouredText("dexterity", dexColour);
                    if (move.dex_inc > 0 && move.dex_inc <= 0.35f)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.dex_inc > 0.35f && move.dex_inc <= 0.8f)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.dex_inc > 0.8f && move.dex_inc <= 1.5f)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.dex_inc > 1.5f)
                    {
                        sharpInc.Add(letter);
                    }
                }
                //Strength
                {
                    string letter = ReturnColouredText("strength", strColour);
                    if (move.str_inc > 0 && move.str_inc <= 0.35f)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.str_inc > 0.35f && move.str_inc <= 0.8f)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.str_inc > 0.8f && move.str_inc <= 1.5f)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.str_inc > 1.5f)
                    {
                        sharpInc.Add(letter);
                    }
                }
                //Vitality
                {
                    string letter = ReturnColouredText("vitality", vitColour);
                    if (move.vit_inc > 0 && move.vit_inc <= 0.35f)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.vit_inc > 0.35f && move.vit_inc <= 0.8f)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.vit_inc > 0.8f && move.vit_inc <= 1.5f)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.vit_inc > 1.5f)
                    {
                        sharpInc.Add(letter);
                    }
                }
                //Guts
                {
                    string letter = ReturnColouredText("guts", gutColour);
                    if (move.gut_inc > 0 && move.gut_inc <= 0.35f)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.gut_inc > 0.35f && move.gut_inc <= 0.8f)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.gut_inc > 0.8f && move.gut_inc <= 1.5f)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.gut_inc > 1.5f)
                    {
                        sharpInc.Add(letter);
                    }
                }
                //Agility
                {
                    string letter = ReturnColouredText("agility", agiColour);
                    if (move.agi_inc > 0 && move.agi_inc <= 0.35f)
                    {
                        smallInc.Add(letter);
                    }
                    else if (move.agi_inc > 0.35f && move.agi_inc <= 0.8f)
                    {
                        moderateInc.Add(letter);
                    }
                    else if (move.agi_inc > 0.8f && move.agi_inc <= 1.5f)
                    {
                        largeInc.Add(letter);
                    }
                    else if (move.agi_inc > 1.5f)
                    {
                        sharpInc.Add(letter);
                    }
                }
                break;
        }
        #endregion

        #region DECREASE
        switch (move.statusMoveType)
        {
            case STATUS_MOVE_TYPE.DEBUFF:
                //Dexterity
                {
                    string letter = ReturnColouredText("dexterity", dexColour);
                    if (move.dex_inc > 0 && move.dex_inc <= 0.35f)
                    {
                        smallDec.Add(letter);
                    }
                    else if (move.dex_inc > 0.35f && move.dex_inc <= 0.8f)
                    {
                        moderateDec.Add(letter);
                    }
                    else if (move.dex_inc > 0.8f && move.dex_inc <= 1.5f)
                    {
                        largeDec.Add(letter);
                    }
                    else if (move.dex_inc > 1.5f)
                    {
                        sharpDec.Add(letter);
                    }
                }
                //Strength
                {
                    string letter = ReturnColouredText("strength", strColour);
                    if (move.str_inc > 0 && move.str_inc <= 0.35f)
                    {
                        smallDec.Add(letter);
                    }
                    else if (move.str_inc > 0.35f && move.str_inc <= 0.8f)
                    {
                        moderateDec.Add(letter);
                    }
                    else if (move.str_inc > 0.8f && move.str_inc <= 1.5f)
                    {
                        largeDec.Add(letter);
                    }
                    else if (move.str_inc > 1.5f)
                    {
                        sharpDec.Add(letter);
                    }
                }
                //Vitality
                {
                    string letter = ReturnColouredText("vitality", vitColour);
                    if (move.vit_inc > 0 && move.vit_inc <= 0.35f)
                    {
                        smallDec.Add(letter);
                    }
                    else if (move.vit_inc > 0.35f && move.vit_inc <= 0.8f)
                    {
                        moderateDec.Add(letter);
                    }
                    else if (move.vit_inc > 0.8f && move.vit_inc <= 1.5f)
                    {
                        largeDec.Add(letter);
                    }
                    else if (move.vit_inc > 1.5f)
                    {
                        sharpDec.Add(letter);
                    }
                }
                //Guts
                {
                    string letter = ReturnColouredText("guts", gutColour);
                    if (move.gut_inc > 0 && move.gut_inc <= 0.35f)
                    {
                        smallDec.Add(letter);
                    }
                    else if (move.gut_inc > 0.35f && move.gut_inc <= 0.8f)
                    {
                        moderateDec.Add(letter);
                    }
                    else if (move.gut_inc > 0.8f && move.gut_inc <= 1.5f)
                    {
                        largeDec.Add(letter);
                    }
                    else if (move.gut_inc > 1.5f)
                    {
                        sharpDec.Add(letter);
                    }
                }
                //Agility
                {
                    string letter = ReturnColouredText("agility", agiColour);
                    if (move.agi_inc > 0 && move.agi_inc <= 0.35f)
                    {
                        smallDec.Add(letter);
                    }
                    else if (move.agi_inc > 0.35f && move.agi_inc <= 0.8f)
                    {
                        moderateDec.Add(letter);
                    }
                    else if (move.agi_inc > 0.8f && move.agi_inc <= 1.5f)
                    {
                        largeDec.Add(letter);
                    }
                    else if (move.agi_inc > 1.5f)
                    {
                        sharpDec.Add(letter);
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

        switch (move.moveType) {
            case MOVE_TYPE.STATUS:
                damage = moveT;
                break;

            case MOVE_TYPE.TALK:
                if (move.element == ELEMENT.NORMAL)
                    damage = damageSize + " " + actionT + " " + moveT + " damage.";
                else
                    damage = damageSize + " " + elementT + " " + actionT + " " + moveT + " damage.";
                break;
            case MOVE_TYPE.PHYSICAL:
            case MOVE_TYPE.SPECIAL:
                damage = damageSize + " " + moveT + " " + elementT + " damage.";
                break;
        }

        moveDescription.text = move.infoDescription + "\n" + damage + "\n" + statusEff;
    }

    public string ReturnColouredText(string str, Color colour) {

        return "<color=#" + ColorUtility.ToHtmlStringRGB(colour) + ">"+ str + "</color>";
    }

    public override void OnOpen()
    {
        base.OnOpen();
        ResetButton();
        //List<s_move> rpgSkills = s_battleEngine.engineSingleton.currentCharacter.currentMoves;
        print(rpgSkills.Count);
        if (!isItem)
        {
            for (int i = 0; i < rpgSkills.Count; i++)
            {
                s_buttonSkill sb = GetButton<s_buttonSkill>(i);
                sb.BMenu = this;
                sb.gameObject.SetActive(true);
                sb.moveButton = rpgSkills[i];
                sb.txt.text = rpgSkills[i].name;
                sb.item = false;
                Sprite draw = null;
                switch (sb.moveButton.element)
                {
                    case ELEMENT.NORMAL:
                        draw = strike_picture;
                        break;
                    case ELEMENT.PEIRCE:
                        draw = perice_picture;
                        break;
                    case ELEMENT.FIRE:
                        draw = fire_picture;
                        break;
                    case ELEMENT.ICE:
                        draw = ice_picture;
                        break;
                    case ELEMENT.ELECTRIC:
                        draw = electric_picture;
                        break;
                    case ELEMENT.FORCE:
                        draw = force_picture;
                        break;
                    case ELEMENT.EARTH:
                        draw = earth_picture;
                        break;
                    case ELEMENT.LIGHT:
                        draw = light_picture;
                        break;
                    case ELEMENT.PSYCHIC:
                        draw = psychic_picture;
                        break;
                    case ELEMENT.WIND:
                        draw = wind_picture;
                        break;
                }
                switch (sb.moveButton.action_type)
                {
                    case ACTION_TYPE.FLIRT:
                        draw = flirt_picture;
                        break;

                    case ACTION_TYPE.PLAYFUL:
                        draw = playful_picture;
                        break;

                    case ACTION_TYPE.THREAT:
                        draw = threat_picture;
                        break;

                    case ACTION_TYPE.COMFORT:
                        draw = comfort_picture;
                        break;
                }
                if (sb.moveButton.moveType == MOVE_TYPE.STATUS) {
                    draw = support_picture;
                }
                sb.element.sprite = draw;
            }
        }
        else {
            int ind = 0;
            foreach (KeyValuePair<rpg_item, int> it in items) {
                if (it.Value == 0)
                    continue;
                s_buttonSkill sb = GetButton<s_buttonSkill>(ind);
                sb.itemButton = it.Key;
                sb.itemCount = it.Value;
                sb.item = true;
                ind++;
            }
        }
    }

    public override void ResetButton<T>(T b)
    {
        base.ResetButton(b);
        s_buttonSkill d = b.GetComponent<s_buttonSkill>();
        d.element.sprite = null;
    }
}
