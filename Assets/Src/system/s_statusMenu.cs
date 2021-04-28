using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_statusMenu : s_menucontroller
{
    public o_battleCharData currentChar;
    public int index;

    public s_statusButton[] buttons;

    public s_guiList str;
    public s_guiList dx;
    public s_guiList vit;
    public s_guiList gut;
    public s_guiList agi;

    public Text nameChar;
    public Slider hp;
    public Slider sp;
    public Text hpText;
    public Text spText;

    public void SetChar(ref o_battleCharData cha) {
        currentChar = cha;
    }

    public override void OnOpen()
    {
        base.OnOpen();
        List<o_battleCharData> partyMembers = rpg_globals.gl.partyMembers;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < partyMembers.Count; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttons[i].character = partyMembers[i];
            buttons[i].txt.text = partyMembers[i].name;
        }
    }

    void Update()
    {
        if (currentChar.name == "") {
            currentChar = rpg_globals.gl.partyMembers[0];
        }
        else
        {
            nameChar.text = currentChar.name;
            
            float health = ((float)currentChar.hitPoints / (float)currentChar.maxHitPoints) * 100;
            float stamina = ((float)currentChar.skillPoints / (float)currentChar.maxSkillPoints) * 100f;

            hp.value = Mathf.Round(health);
            sp.value = Mathf.Round(stamina);

            hpText.text = Mathf.Round(health) + "% " + currentChar.hitPoints + "/"+ currentChar.maxHitPoints;
            spText.text = Mathf.Round(stamina) + "% " + currentChar.skillPoints + "/" + currentChar.maxSkillPoints;

            str.amount = currentChar.attack;
            vit.amount = currentChar.defence;
            dx.amount = currentChar.intelligence;
            agi.amount = currentChar.speed;
            gut.amount = currentChar.guts;
        }
    }
}
