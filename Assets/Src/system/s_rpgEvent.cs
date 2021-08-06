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
    public static bool _inBattle = false;
    delegate void disableBattle();
    disableBattle db;

    public new void Awake()
    {
        DontDestroyOnLoad(gameObject);
        textBox.gameObject.SetActive(false);
        base.Awake();
        rpgEv = this;
        evEnd = EndEvent;
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

    public void EndEvent() {
        if(!_inBattle)
            s_menuhandler.GetInstance().SwitchMenu("OpenMenu");
    }
    IEnumerator GotoBattle(enemy_group group)
    {
        StartCoroutine(s_BGM.GetInstance().FadeOutMusic(2f));
        yield return new WaitForSeconds(0.6f);
        s_camera.cam.ZoomCamera(10, 125);
        MagnumFoundation2.System.Core.s_soundmanager.GetInstance().PlaySound("encounter");
        yield return StartCoroutine(s_triggerhandler.trigSingleton.Fade(Color.black));
        yield return new WaitForSeconds(0.6f);

        rpg_globals.gl.SwitchToBattle(group);
    }

    public override IEnumerator EventPlay(ev_details current_ev)
    {
        switch ((EVENT_TYPES)current_ev.eventType)
        {
            default:
                s_menuhandler.GetInstance().SwitchMenu("EMPTY");
                yield return StartCoroutine(base.EventPlay(current_ev));
                break;


            case EVENT_TYPES.CUSTOM_FUNCTION:

                switch (current_ev.funcName) {
                    case "START_BATTLE":
                        _inBattle = true;
                        bs = GetComponent<s_battlesyst>();
                        pointer = -1;
                        doingEvents = false;
                        player.rendererObj.color = Color.clear;
                        /*
                        s_camera.cam.ZoomCamera(10, 250);
                        MagnumFoundation2.System.Core.s_soundmanager.GetInstance().PlaySound("encounter");
                        yield return StartCoroutine(Fade(Color.black));
                        */

                        yield return StartCoroutine(GotoBattle((enemy_group)current_ev.scrObj));
                        break;

                    case "ADD_PARTY_MEMBER":
                        BattleCharacterData bcd = current_ev.scrObj as BattleCharacterData;
                        if (rpg_globals.gl.partyMembers.Find(x => x.name == bcd.name) == null)
                            rpg_globals.gl.AddMemeber(bcd, current_ev.int0);
                        break;

                    case "INCREASE_ES_LIMIT":
                        rpg_globals.gl.extraSkillAmount += current_ev.int0;
                        break;

                    case "BIG_TEXT":
                        if (isSkipping)
                        {
                            bigTxt.color = Color.clear;
                            yield return new WaitForSeconds(Time.deltaTime);
                        }
                        else
                        {
                            bigTxt.text = current_ev.string0;
                            print(current_ev.string0);
                            bigTxt.color = Color.clear;
                            float a = 0;
                            while (bigTxt.color != Color.white)
                            {
                                bigTxt.color = Color.Lerp(bigTxt.color, Color.white, a);
                                a += Time.deltaTime;
                                yield return new WaitForSeconds(Time.deltaTime * 1.75f);
                            }
                            a = 0;
                            float t = 0;
                            continueTxt.text = "Press " + rpg_globals.GetKeyPref("select").ToString() + " to continue.";
                            while (!Input.GetKeyDown(rpg_globals.GetKeyPref("select")))
                            {
                                t = Mathf.Sin(a);
                                a += Time.deltaTime * 2.5f;
                                continueTxt.color = Color.Lerp(Color.white, Color.clear, t);
                                yield return new WaitForSeconds(Time.deltaTime);
                            }
                            continueTxt.color = Color.clear;
                            a = 0;
                            while (bigTxt.color != Color.clear)
                            {
                                bigTxt.color = Color.Lerp(bigTxt.color, Color.clear, a);
                                a += Time.deltaTime;
                                yield return new WaitForSeconds(Time.deltaTime);
                            }
                        }
                        break;
                }

                break;
        }
    }

}
