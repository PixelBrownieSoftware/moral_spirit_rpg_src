using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_battleAnalyze : s_menucontroller
{
    public s_analyzeStats[] enemies; 
    public s_analyzeStats[] players;

    public void ResetStats() {
        foreach (s_analyzeStats st in enemies) {
            st.gameObject.SetActive(false);
        }
        foreach (s_analyzeStats st in players){
            st.gameObject.SetActive(false);
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();
        ResetStats();
        int ind = 0;
        foreach (o_battleChar bc in s_battlesyst.GetInstance().players) {
            players[ind].battleChar = bc;
            players[ind].SetText();
            ind++;
        }
        ind = 0;
        foreach (o_battleChar bc in s_battlesyst.GetInstance().opposition)
        {
            enemies[ind].battleChar = bc;
            enemies[ind].SetText();
            ind++;
        }
    }
}
