using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_targCharButton : s_button
{
    public o_battleCharData battleChar;
    s_move mov;
    public bool isItem = false;
    public string itemName;

    protected override void OnButtonClicked()
    {
        if (isItem)
        {
            rpg_globals.gl.UseItem(itemName);
        }
        else {
            battleChar.skillPoints -= mov.cost;
            battleChar.hitPoints += (int)((float)mov.power * (float)(battleChar.intelligence / 4));
            if (battleChar.hitPoints > battleChar.maxHitPoints)
            {
                battleChar.hitPoints = battleChar.maxHitPoints;
            }
        }
        base.OnButtonClicked();
    }

    private void Update()
    {
        mov = rpg_globals.gl.currentMove;
        if (battleChar.skillPoints < mov.cost)
        {
            gameObject.SetActive(false);
        }
        if (battleChar.hitPoints == battleChar.maxHitPoints)
        {
            gameObject.SetActive(false);
        }
        if (battleChar.name != "") {
            txt.text = battleChar.name + " HP: " + battleChar.hitPoints;
        }
    }
}
