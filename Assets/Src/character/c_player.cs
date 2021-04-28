using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.Objects;

public class c_player : o_overworlC
{
    Animator anim;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    new void Start()
    {
        DisableAttack();
        Initialize();
        base.Start();
        anim = SpriteObj.GetComponent<Animator>();
    }
    public override void PlayerControl()
    {
        switch (CHARACTER_STATE)
        {
            case CHARACTER_STATES.STATE_IDLE:

                // SetAnimation("test", true);

                AnimDirection("idle");
                if (ArrowKeyControl())
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                }

                /*
                if (Input.GetMouseButtonDown(1))
                {
                    StartCoroutine(Attack(weapons[selected_weapon_num]));
                }
                if (Input.GetKeyDown(s_globals.i["jump"]))
                {
                    Jump(1.95f);
                }
                */

                break;

            case CHARACTER_STATES.STATE_MOVING:
                if (control)
                {
                    AnimDirection("walk");
                    //charAnimation.SetAnimation("test2", true);

                    if (!ArrowKeyControl())
                    {
                        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                    }
                    
                }

                break;

        }
    }
    public void AnimDirection(string animName)
    {
        int verticalDir = (int)direction.y;
        int horizontalDir = (int)direction.x;

        if (verticalDir == -1 && horizontalDir == 0)
            SetAnimation(animName + "_d", true);
        else if (verticalDir == 1 && horizontalDir == 0)
            SetAnimation(animName + "_u", true);
        else if (horizontalDir == -1 && verticalDir == 1 ||
            horizontalDir == -1 && verticalDir == -1 || horizontalDir == -1)
        {
            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            SetAnimation(animName + "_s", true);
        }
        else if (horizontalDir == 1 && verticalDir == 1 ||
            horizontalDir == 1 && verticalDir == -1 || horizontalDir == 1)
        {
            rendererObj.transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));
            SetAnimation(animName + "_s", true);
        }
    }

    public new void Update()
    {
        base.Update();
        o_tresure col = IfTouchingGetCol<o_tresure>(collision);
        if (col != null)
        {
            print("K");
            rpg_globals.gl.ShowPrompt();
        }
        else
        {
            rpg_globals.gl.HidePrompt();
        }
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
