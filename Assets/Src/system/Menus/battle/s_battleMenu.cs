using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2;
using UnityEngine.UI;

public class s_battleMenu : s_menucontroller
{
    public List<s_move> rpgSkills = new List<s_move>();
    public Dictionary<s_move, int> items = new Dictionary<s_move, int>();
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
    public Color reservedColour;
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
    public Sprite reserved_picture;

    public Sprite heal_picture;
    public Sprite heal_stamina_picture;
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
        
        string damage = "";

        if (move.power < 11)
        {
            damageSize = "Small";
        }
        else if (move.power >= 11 && move.power < 21)
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

            case ACTION_TYPE.RESERVED:
                actionT = ReturnColouredText(actionT, reservedColour);
                break;

            case ACTION_TYPE.PSYCHIC:
                actionT = ReturnColouredText(actionT, psychicColour);
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
        
        switch (move.statusMoveType)
        {
            case STATUS_MOVE_TYPE.DEBUFF:
            case STATUS_MOVE_TYPE.BUFF:
                //Dexterity
                {
                    string letter = ReturnColouredText("dexterity", dexColour);
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
                    string letter = ReturnColouredText("strength", strColour);
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
                    string letter = ReturnColouredText("vitality", vitColour);
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
                    string letter = ReturnColouredText("guts", gutColour);
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
                    string letter = ReturnColouredText("agility", agiColour);
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
        
        switch (move.statusMoveType)
        {
            case STATUS_MOVE_TYPE.DEBUFF:
                if (smallInc.Count > 0)
                {
                    statusEff += "Slightly decreases ";
                    foreach (string str in smallInc)
                    {
                        statusEff += str + ",";
                    }
                }
                if (moderateInc.Count > 0)
                {
                    statusEff += "Moderately decreases ";
                    foreach (string str in moderateInc)
                    {
                        statusEff += str + ",";
                    }
                }
                if (largeInc.Count > 0)
                {
                    statusEff += "Largely decreases ";
                    foreach (string str in largeInc)
                    {
                        statusEff += str + ",";
                    }
                }
                if (sharpInc.Count > 0)
                {
                    statusEff += "Sharply decreases ";
                    foreach (string str in sharpInc)
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
                damage = damageSize + " " + actionT + " " + moveT + " resolve damage.";
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
        if (!isItem)
        {
            for (int i = 0; i < rpgSkills.Count; i++)
            {
                if (rpgSkills[i].target == TARGET_MOVE_TYPE.NONE)
                    continue;
                s_buttonSkill sb = GetButton<s_buttonSkill>(i);
                sb.BMenu = this;
                sb.gameObject.SetActive(true);
                sb.moveButton = rpgSkills[i];
                sb.txt.text = rpgSkills[i].name;
                sb.item = false;
                Sprite draw = null;
                /*
                switch (sb.moveButton.element)
                {
                    case ELEMENT.NORMAL:
                        sb.buttonTex.color = strikeColour;
                        draw = strike_picture;
                        break;
                    case ELEMENT.PEIRCE:
                        sb.buttonTex.color = peirceColour;
                        draw = perice_picture;
                        break;
                    case ELEMENT.FIRE:
                        sb.buttonTex.color = fireColour;
                        draw = fire_picture;
                        break;
                    case ELEMENT.ICE:
                        sb.buttonTex.color = iceColour;
                        draw = ice_picture;
                        break;
                    case ELEMENT.ELECTRIC:
                        sb.buttonTex.color = electricColour;
                        draw = electric_picture;
                        break;
                    case ELEMENT.FORCE:
                        sb.buttonTex.color = forceColour;
                        draw = force_picture;
                        break;
                    case ELEMENT.EARTH:
                        sb.buttonTex.color = earthColour;
                        draw = earth_picture;
                        break;
                    case ELEMENT.LIGHT:
                        sb.buttonTex.color = lightColour;
                        draw = light_picture;
                        break;
                    case ELEMENT.DARK:
                        sb.buttonTex.color = darkColour;
                        draw = dark_picture;
                        break;
                    case ELEMENT.PSYCHIC:
                        sb.buttonTex.color = psychicColour;
                        draw = psychic_picture;
                        break;
                    case ELEMENT.WIND:
                        sb.buttonTex.color = windColour;
                        draw = wind_picture;
                        break;
                }
                */
                switch (sb.moveButton.action_type)
                {
                    case ACTION_TYPE.FLIRT:
                        sb.buttonTex.color = flirtColour;
                        draw = flirt_picture;
                        break;

                    case ACTION_TYPE.PLAYFUL:
                        sb.buttonTex.color = playfulColour;
                        draw = playful_picture;
                        break;

                    case ACTION_TYPE.THREAT:
                        sb.buttonTex.color = threatColour;
                        draw = threat_picture;
                        break;

                    case ACTION_TYPE.COMFORT:
                        sb.buttonTex.color = comfortColour;
                        draw = comfort_picture;
                        break;

                    case ACTION_TYPE.RESERVED:
                        sb.buttonTex.color = reservedColour;
                        draw = reserved_picture;
                        break;
                    case ACTION_TYPE.LIGHT:
                        sb.buttonTex.color = lightColour;
                        draw = light_picture;
                        break;
                    case ACTION_TYPE.DARK:
                        sb.buttonTex.color = darkColour;
                        draw = dark_picture;
                        break;
                    case ACTION_TYPE.PSYCHIC:
                        sb.buttonTex.color = psychicColour;
                        draw = psychic_picture;
                        break;
                }
                sb.buttonTex.color += new Color(0.3f, 0.3f, 0.3f);
                if (sb.moveButton.moveType == MOVE_TYPE.STATUS) {

                    sb.buttonTex.color = Color.white;
                    switch (sb.moveButton.statusMoveType) {
                        case STATUS_MOVE_TYPE.HEAL:
                            draw = heal_picture;
                            break;

                        case STATUS_MOVE_TYPE.HEAL_STAMINA:
                            draw = heal_stamina_picture;
                            break;

                        default:
                            draw = support_picture;
                            break;
                    }
                }
                sb.element.sprite = draw;
            }
        }
        else {
            int ind = 0;
            foreach (KeyValuePair<s_move, int> it in items) {
                if (it.Value == 0)
                    continue;
                Sprite draw = null;
                s_buttonSkill sb = GetButton<s_buttonSkill>(ind);
                sb.moveButton = it.Key;
                sb.itemCount = it.Value;
                sb.item = true;
                if (sb.moveButton.moveType == MOVE_TYPE.STATUS)
                {

                    sb.buttonTex.color = Color.white;
                    switch (sb.moveButton.statusMoveType)
                    {
                        case STATUS_MOVE_TYPE.HEAL:
                            draw = heal_picture;
                            break;

                        case STATUS_MOVE_TYPE.HEAL_STAMINA:
                            draw = heal_stamina_picture;
                            break;

                        default:
                            draw = support_picture;
                            break;
                    }
                }
                sb.element.sprite = draw;
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
