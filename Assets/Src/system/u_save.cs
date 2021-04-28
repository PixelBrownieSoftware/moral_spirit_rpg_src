using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.System;
using MagnumFoundation2.System.Core;
using MagnumFoundation2.Objects;

public class u_save : s_object
{
    public bool isActive = false;
    public BoxCollider2D trigger;
    public ev_script saveScript;

    private new void Update()
    {
        base.Update();

        c_player p = IfTouchingGetCol<c_player>(trigger);
        if (p != null)
        {
            if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
            {
                rpg_globals.gl.HealParty();
                s_soundmanager.GetInstance().PlaySound("healSound");
                s_rpgEvent.rpgEv.JumpToEvent(saveScript);
            }
        }
    }
}
