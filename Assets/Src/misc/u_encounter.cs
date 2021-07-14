using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.System;
using MagnumFoundation2.System.Core;
using MagnumFoundation2.Objects;

public class u_encounter : s_object
{
    public enemy_group group;
    public GameObject[] enemies;
    public s_object selobj;
    public bool destroyEnemies = true;
    bool _triggered = false;

    public void DestroyAllEnemies()
    {
        switchA = true;
        s_globals.globalSingle.AddTriggerState(new triggerState(name, gameObject.scene.name, switchA));
        if (destroyEnemies)
        {
            foreach (GameObject ob in enemies) {
                Destroy(ob);
            }
        }
        Destroy(gameObject);
    }

    IEnumerator GotoBattle()
    {
        s_camera.cam.ZoomCamera(10, 125);
        MagnumFoundation2.System.Core.s_soundmanager.GetInstance().PlaySound("encounter");
        yield return StartCoroutine(s_triggerhandler.trigSingleton.Fade(Color.black));
        yield return new WaitForSeconds(0.6f);

        rpg_globals.gl.SwitchToBattle(group);
    }

    private new void Update()
    {
        base.Update();
        if (!switchA)
        {
            o_character c = IfTouchingGetCol<o_character>(collision);
            if (c != null)
            {
                selobj = c.gameObject.GetComponent<s_object>();
                //print(name + c.name);
                if (selobj)
                {
                    o_character posses = selobj.GetComponent<o_character>();
                    //print(name + c.name);
                    o_character ch = c.GetComponent<o_character>();
                    if (ch)
                    {
                        if (!ch.AI && ch.control)
                        {
                            if (!_triggered)
                            {
                                _triggered = true;
                                rpg_globals.gl.enc = this;
                                StartCoroutine(GotoBattle());
                            }
                        }
                    }
                }
            }
        }
        else {
            DestroyAllEnemies();
        }
    }
}
