using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2;

public class s_skillButton : s_button
{
    public s_move move;
    public rpg_item item;

    public enum BTN_TYPE {
        SKILL,
        ITEM
    }
    public BTN_TYPE BUTTON_TYPE;

    protected override void OnButtonClicked()
    {
        switch (BUTTON_TYPE)
        {
            case BTN_TYPE.SKILL:
                rpg_globals.gl.currentMove = move;
                break;

            case BTN_TYPE.ITEM:
                rpg_globals.gl.currentMove = item.action;
                break;
        }
        base.OnButtonClicked();
    }
}
