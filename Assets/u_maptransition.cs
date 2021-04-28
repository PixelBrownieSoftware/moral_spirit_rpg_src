using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.System;
using MagnumFoundation2.System.Core;
using MagnumFoundation2.Objects;

public class u_maptransition : s_object
{
    s_menuGui gui;
    public bool isOn = false;

    private new void Start()
    {
        base.Start();
        gui = GameObject.Find("MapMenu").GetComponent<s_menuGui>();
    }

    private new void Update()
    {
        if (Input.GetKeyDown(s_globals.GetKeyPref("select"))) {
            if (!isOn)
            {

                int ind = 0;
                if (s_globals.GetGlobalFlag("hills_unlocked") == 1)
                {
                    gui.ChangeButton(ref ind, "Barrignton Hills");
                }
                if (s_globals.GetGlobalFlag("lounge_unlocked") == 1)
                {
                    gui.ChangeButton(ref ind, "Lounge");
                }
                if (s_globals.GetGlobalFlag("corporate_unlocked") == 1)
                {
                    gui.ChangeButton(ref ind, "Diamond men HQ");
                }
                if (s_globals.GetGlobalFlag("uni_unlocked") == 1)
                {
                    gui.ChangeButton(ref ind, "Frestone University");
                }
            }
            else {

            }
        }
    }
}
