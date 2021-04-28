using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoundation2.System;
using MagnumFoundation2.System.Core;

public class s_targetButton : s_button
{
    public o_battleChar battleCharButton;
    public enum TARGET_TYPE {
        BATTLE,
        OPTIONAL_BATTLE,
        DEPLOY,
        SWITCH
    }
    public TARGET_TYPE targType;

    public Slider HPSlider;
    public Slider SPSlider;

    protected override void OnHover()
    {
        base.OnHover();
        switch (targType)
        {
            case TARGET_TYPE.BATTLE:
                print("HEY");
                s_soundmanager.GetInstance().PlaySound("cursor_move");
                s_camera.cam.SetTargPos(battleCharButton.transform.position);
                break;

            case TARGET_TYPE.OPTIONAL_BATTLE:
                //s_battleEngine.engineSingleton.
                break;
        }
    }

    private void Update()
    {
        if (battleCharButton != null)
        {
            float HP = ((float)battleCharButton.hitPoints / (float)battleCharButton.maxHitPoints) * 100;
            float SP = ((float)battleCharButton.skillPoints / (float)battleCharButton.maxSkillPoints) * 100;
            HP = Mathf.Round(HP);
            SP = Mathf.Round(SP);

            HPSlider.value = HP;
            SPSlider.value = SP;
        }
    }

    protected override void OnButtonClicked()
    {
        switch (targType) {
            case TARGET_TYPE.BATTLE:
                //s_battleEngine.engineSingleton.SelectTarget(battleCharButton);
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                s_soundmanager.GetInstance().PlaySound("selectOption");
                break;

            case TARGET_TYPE.OPTIONAL_BATTLE:
                //s_battleEngine.engineSingleton.
                break;
        }
    }
}
