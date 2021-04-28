using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MagnumFoundation2.System;
using MagnumFoundation2.Objects;

/*
public enum RPG_EVENT_TYPES
{
    LABEL = -1,
    MOVEMNET = 0,
    DIALOGUE = 1,
    SET_HEALTH = 2,
    RUN_CHARACTER_SCRIPT = 3,
    ANIMATION = 4,
    SOUND = 5,
    SET_FLAG = 7,
    CHECK_FLAG = 8,
    CAMERA_MOVEMENT = 9,
    BREAK_EVENT = 10,
    JUMP_TO_LABEL = 11,
    SET_UTILITY_FLAG = 12,
    FADE = 13,
    CREATE_OBJECT = 14,
    DISPLAY_CHARACTER_HEALTH = 15,
    UTILITY_INITIALIZE = 16,
    UTILITY_CHECK = 17,
    WAIT = 18,
    CHOICE = 19,
    CHANGE_SCENE = 20,
    PUT_SHUTTERS = 21,
    DISPLAY_IMAGE = 22,
    SHOW_TEXT = 23,
    CHANGE_MAP = 24,
    DELETE_OBJECT = 26,
    SET_OBJ_COLLISION = 27,
    ADD_CHOICE_OPTION = 28,
    CLEAR_CHOICES = 29,
    PRESENT_CHOICES = 30,
    START_BATTLE = 32,
}
*/

public class s_rpgEvent : s_triggerhandler
{
    rpg_globals rg;
    s_battlesyst bs;
    public static s_rpgEvent rpgEv;
    public Text bigTxt;

    public new void Awake()
    {
        DontDestroyOnLoad(gameObject);
        textBox.gameObject.SetActive(false);
        base.Awake();
        rpgEv = this;
    }

    public override void CreateData()
    {
        base.CreateData();
        {
            customEv ev = new customEv();
            ev.name = "ADD_PARTY_MEMBER";
            ev.hasString0 = true;

            customEvAndFunction.Add(ev);
        }
        {
            customEv ev = new customEv();
            ev.name = "START_BATTLE";
            ev.hasString0 = true;

            customEvAndFunction.Add(ev);
        }
    }


    public override IEnumerator EventPlay(ev_details current_ev)
    {
        switch ((EVENT_TYPES)current_ev.eventType)
        {
            default:
                yield return StartCoroutine(base.EventPlay(current_ev));
                break;


            case EVENT_TYPES.CUSTOM_FUNCTION:

                switch (current_ev.funcName) {
                    case "START_BATTLE":
                        bs = GetComponent<s_battlesyst>();
                        pointer = -1;
                        doingEvents = false;
                        player.rendererObj.color = Color.clear;

                        rpg_globals.gl.SwitchToBattle((enemy_group)current_ev.scrObj);
                        break;

                    case "ADD_PARTY_MEMBER":
                        BattleCharacterData bcd = current_ev.scrObj as BattleCharacterData;
                        if (rpg_globals.gl.partyMembers.Find(x => x.name == bcd.name) == null)
                            rpg_globals.gl.AddMemeber(bcd, current_ev.int0);
                        break;

                    case "BIG_TEXT":
                        bigTxt.text = current_ev.string0;
                        print(current_ev.string0);
                        bigTxt.color = Color.clear;
                        float a = 0;
                        while (bigTxt.color != Color.white) {
                            bigTxt.color = Color.Lerp(bigTxt.color, Color.white, a);
                            a += Time.deltaTime;
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        a = 0;
                        yield return new WaitForSeconds(current_ev.float0);
                        while (bigTxt.color != Color.clear)
                        {
                            bigTxt.color = Color.Lerp(bigTxt.color, Color.clear, a);
                            a += Time.deltaTime;
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        break;
                }

                break;
        }
    }

}
