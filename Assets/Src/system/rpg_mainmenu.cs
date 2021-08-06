using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.System;

public class rpg_mainmenu : s_mainmenu
{
    public override void LoadSave<T>()
    {
        base.LoadSave<T>();
        
    }

    public override void CallLoadSave()
    {
        LoadSave<RPG_save>();
    }

    public void Start() {

        _keyList = new string[7] { "left", "right", "up", "down", "back", "select", "dash" };

    }

}
