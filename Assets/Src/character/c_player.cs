using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoundation2.Objects;
using MagnumFoundation2.System.Core;

public class c_player : o_character
{
    public GameObject thought;
    public Text thoughtText;
    public bool showThought = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    new void Start()
    {
        DisableAttack();
        Initialize();
        base.Start();
    }

    private new void Update()
    {
        base.Update();
        o_trigger c = IfTouchingGetCol<o_trigger>(collision);
        u_save s = IfTouchingGetCol<u_save>(collision);
        o_tresure tr = IfTouchingGetCol<o_tresure>(collision);
        
        if (c != null)
        {
            if (c.TRIGGER_T == TRIGGER_TYPE.CONTACT_INPUT)
            {
                showThought = true;
            }
            else
            {
                showThought = false;
            }
        }
        else if (s != null)
        {
            showThought = true;
        }
        else if (tr != null)
        {
            showThought = true;
        }
        else
        {
            showThought = false;
        }

        if (showThought)
        {
            thought.SetActive(true);
            thoughtText.text = s_globals.GetKeyPref("select").ToString();
        }
        else
        {
            thought.SetActive(false);
            thoughtText.text = "";
        }

    }

    public override void PlayerControl()
    {
        if (Input.GetKey(s_globals.GetKeyPref("dash")))
        {
            terminalspd = terminalSpeedOrigin * 1.5f;
        }
        else
        {
            terminalspd = terminalSpeedOrigin;
        }
        switch (CHARACTER_STATE)
        {
            case CHARACTER_STATES.STATE_IDLE:
                if (ArrowKeyControl())
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                }
                break;

            case CHARACTER_STATES.STATE_MOVING:

                if (!ArrowKeyControl())
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                }
                break;
        }
        AnimMove();
    }

}
