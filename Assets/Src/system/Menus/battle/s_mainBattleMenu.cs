using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_mainBattleMenu : s_menucontroller
{
    public override void OnOpen()
    {
        base.OnOpen();
        buttons[0].gameObject.SetActive(false);
        buttons[6].gameObject.SetActive(false);
        if (rpg_globals.gl.inventory.Count > 0)
        {
            buttons[2].gameObject.SetActive(true);
        }
        else {
            buttons[2].gameObject.SetActive(false);
        }
        /*
        bool spareButtonOn = false;

        if (s_battlesyst.GetInstance().enemyGroup.allRelations)
        {
            if (s_battlesyst.GetInstance().opposition.FindAll(x => x.skillPoints <= 0) != null)
            {
                if (s_battlesyst.GetInstance().opposition.FindAll(x => x.skillPoints <= 0).Count ==
                    s_battlesyst.GetInstance().opposition.FindAll(x => x.hitPoints > 0).Count)
                    spareButtonOn = true;
                else
                    spareButtonOn = false;
            }
            else
                spareButtonOn = false;

        }
        else
        {
            if (s_battlesyst.GetInstance().opposition.FindAll(x => x.skillPoints <= 0).Count > 0)
            {
                spareButtonOn = true;
            }
        }

        if (spareButtonOn)
            buttons[6].gameObject.SetActive(true);
        else
            buttons[6].gameObject.SetActive(false);
        */

        if (s_battlesyst.GetInstance().enemyGroup.isFleeable)
            buttons[7].gameObject.SetActive(true);
        else
            buttons[7].gameObject.SetActive(false);
    }
}
