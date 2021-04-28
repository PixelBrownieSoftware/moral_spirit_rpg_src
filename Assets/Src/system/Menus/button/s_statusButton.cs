using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_statusButton : s_button
{
    public s_statusMenu menu;
    public o_battleCharData character;

    protected override void OnButtonClicked()
    {
        menu.SetChar(ref character);
        base.OnButtonClicked();
    }
}
