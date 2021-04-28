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

    public bool isItem = false;

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
                Sprite draw = null;
                switch (sb.moveButton.element)
                {
                    case ELEMENT.NORMAL:
                        draw = strike_picture;
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
                sb.element.sprite = draw;
            }
        }
        else {
            int ind = 0;
            foreach (KeyValuePair<rpg_item, int> it in items) {

                s_buttonSkill sb = GetButton<s_buttonSkill>(ind);
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
