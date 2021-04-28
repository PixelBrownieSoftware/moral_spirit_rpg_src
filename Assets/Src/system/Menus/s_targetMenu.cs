using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_targetMenu : s_menucontroller
{
    public override void OnOpen()
    {
        base.OnOpen();
        List<o_battleCharData> bcs = rpg_globals.gl.partyMembers;
        int ind = 0;
        foreach (s_targCharButton button in buttons)
        {
            button.txt.text = "";
            button.gameObject.SetActive(false);
        }
        foreach (o_battleCharData bcD in bcs)
        {
            GetButton<s_targCharButton>(ind).gameObject.SetActive(true);
            GetButton<s_targCharButton>(ind).txt.text = bcD.name + " - " + bcD.hitPoints;
            GetButton<s_targCharButton>(ind).battleChar = bcD;
            ind++;
        }
    }
}
