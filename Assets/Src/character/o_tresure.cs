using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.System;
using MagnumFoundation2.Objects;
using MagnumFoundation2.System.Core;

public class o_tresure : s_object
{
    public s_move item;
    public int amount;

    public Sprite Open;
    public Sprite Closed;
    public BoxCollider2D bx;
    public ev_script scr;

    public void Awake() {
        collision = GetComponent<BoxCollider2D>();
    }

    public new void Update() {
        base.Update();
        if (switchA)
            rendererObj.sprite = Open;
        else
            rendererObj.sprite = Closed;
        if (!switchA)
        {
            c_player col = IfTouchingGetCol<c_player>(bx);
            if (col != null)
            {
                if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                {
                    rpg_globals.gl.AddItem(item.name, amount);
                    switchA = true;
                    print(name);
                    s_globals.globalSingle.AddTriggerState(new triggerState(name, gameObject.scene.name, switchA));
                    s_rpgEvent.rpgEv.JumpToEvent(scr, item.name + " x " + amount);
                }
                //Put up notification
            }
        }
    }
}
