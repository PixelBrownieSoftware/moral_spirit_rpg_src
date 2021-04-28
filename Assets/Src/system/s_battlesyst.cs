﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using MagnumFoundation2.System.Core;

[Serializable]
public class s_battleAction
{
    public enum MOVE_TYPE
    {
        MOVE,
        ITEM,
        GUARD,
        PASS
    };
    public MOVE_TYPE type;
    public o_battleChar user;
    public o_battleChar target;
    public s_move move;
    public rpg_item item;

    public s_battleAction(o_battleChar user, o_battleChar target, s_move move)
    {
        this.user = user;
        this.target = target;
        this.move = move;
        item = null;
        type = MOVE_TYPE.MOVE;
    }

    public s_battleAction(o_battleChar user, o_battleChar target, rpg_item item)
    {
        this.user = user;
        this.target = target;
        move = null;
        this.item = item;
        type = MOVE_TYPE.ITEM;
    }
}

[Serializable]
public class s_pressTurn {
    public bool isHalf;
    public GameObject obj;
}

public enum MENUSTATE
{
    NONE,
    MENU,
    FIGHT,
    SKILLS,
    ACT,
    SPARE,
    SPARE_CHOICE,
    ITEM,
    ANALYZE
};

public enum TARGET_TYPE
{
    PLAYER,
    OPPONENT,
    ALL
}
public enum PRESS_TURN_TYPE
{
    NORMAL,
    PASS,
    WEAKNESS,
    IMMUNE,
    REFLECT,
    ABSORB,
    MISS
}; 

public enum BATTLE_ENGINE_STATE
{
    IDLE,
    DECISION,
    TARGET,
    MOVE_START,
    MOVE_PROCESS,
    NEGOTIATE_MENU,
    END,
    NONE
}

/// <summary>
/// To run the game's battle engine
/// </summary>
public class s_battlesyst : s_singleton<s_battlesyst>
{
    public bool isActive;
    public SpriteRenderer battleFx;
    public SpriteRenderer[] battleFxGroup;
    public Texture2D pressTurnIcon;

    public Rect testRect;

    public s_inventory inventory;

    public GameObject animationObject;
    public GameObject battleSystGUI;

    public List<o_battleChar> allCharacters;
    public List<o_battleChar> players;
    public List<o_battleChar> opposition;

    List<o_battleChar> sparable;

    public AudioClip moveCursorSelect;
    public AudioClip upTurn;
    public AudioClip selectOption;
    public AudioClip playerHurt;
    public AudioClip healSound;

    public s_dmg[] damageObject;

    public GameObject damageObj;
    public GameObject enemyHealth;

    //Move, User, Target
    public s_battleAction currentMove;
    public Queue<o_battleChar> playerCharacterTurnQueue = new Queue<o_battleChar>();
    public Queue<o_battleChar> oppositionCharacterTurnQueue = new Queue<o_battleChar>();
    public o_battleChar guestPartyMember;   //They have their turn right after the party members, like earthbound/mother 3, they can't take damage

    public HashSet<o_battleChar> active_characters = new HashSet<o_battleChar>();
    public o_battleChar currentCharacter;
    public int Menuchoice = 0;
    public string currentItem;
    int menuchoice_leng = 0;
    //If the chosen move is on the team
    bool onTeam;
    bool menuBoxOpen = false;

    public float playerMoralePoints;
    public float enemyMoralePoints;

    #region buttonVisuals
    public Image fightButton;
    public Image guardButton;
    public Image skillsButton;
    public Image itemsButton;
    public Image spareButton;
    public Image runButton;
    public Image passButton;
    public Image analyzeButton;
    public Texture battleButtons;
    #endregion

    public Text pressTurnText;

    public s_battleEvents[] OnBattleEvents;
    public int roundNum;
    public int countRoundNum; //For counting rounds

    int fleeTurns = 0;  //For the time to flee

    public int pressTurn;
    public int halfTurn;
    public int netTurn
    {
        get
        {
            return pressTurn + halfTurn;
        }
    }
    public BATTLE_ENGINE_STATE CurrentBattleEngineState = BATTLE_ENGINE_STATE.IDLE;

    #region menuVisuals
    public Sprite weakDmgTargImage;
    public Sprite normalDmgTargImage;
    public Sprite resDmgTargImage;

    public Image menuBox;
    public Animator TurnTextGui;

    //Draw the exp bar for each character when the battle ends
    bool drawExp = false;
    public Texture expBarTexture;
    public Texture guiBar;
    public int[] levelUpFlags;

    public Slider[] EXPList;
    public Text[] EXPText;
    public GameObject EXPResults;
    
    //Repeating buttons menu
    public GameObject listOfbuttons;
    public menu_button[] menuButtons;
    public GameObject[] menuButtonObjects;

    public menu_button displayMoveName;
    public Text earningsBattle;

    [System.Serializable]
    public class menu_button {
        public Image img;
        public Text txt;
        public Image selector;
    }

    public Sprite strikeImage;
    public Sprite forceImage;
    public Sprite peirceImage;
    public Sprite fireImage;
    public Sprite iceImage;
    public Sprite windImage;
    public Sprite electricImage;
    public Sprite earthImage;
    public Sprite psychicImage;
    public Sprite darkImage;
    public Sprite lightImage;

    public Slider enemyHP;
    public Slider enemySP;

    public GameObject[] allTargetSelectorsObj;
    [System.Serializable]
    public class selector {
        public GameObject Selector;
        public Slider enemyHP;
        public Slider enemySP;
        public Image weaknessTarg;
    }
    public selector singleTargSelector;
    public selector[] allTargetSelectors;
    #endregion

    public s_analyzeStats[] analyzeGUIs;
    public bool isPlayerTurn;
    bool[] animationsDone;

    public Text actionDisp;
    public Text enemyHPSP;

    public List<s_battleguiHP> guis = new List<s_battleguiHP>();
    public List<GameObject> animPrefabs = new List<GameObject>();
    public Image[] PT_GUI;
    public enemy_group enemyGroup;
    public u_encounter encObj;
    Camera cam;

    public Image menuSelector;
    public Image fadeFX;

    List<string> battleOptions;

    TARGET_TYPE targType;

    public MENUSTATE menu_state = MENUSTATE.NONE;

    public const float GUIsepDist = 112;
    public PRESS_TURN_TYPE turnPressFlag = PRESS_TURN_TYPE.NORMAL;
    public s_move normalAttack;
    public bool isCutscene = false;

    public void HitWeakness()
    {
        if (halfTurn >= netTurn)
        {
            halfTurn--;
        }
        else
        {
            halfTurn++;
            pressTurn--;
        }
    }
    public void NextTurn()
    {
        if (halfTurn > 0)
            halfTurn--;
        else
            pressTurn--;
    }
    public void PassTurn()
    {
        if (halfTurn > 0)
        {
            halfTurn--;
        }
        else
        {
            halfTurn++;
            pressTurn--;
        }
    }

    public void Awake()
    {
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        isActive = false;
        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
        currentMove = null;
        inventory = GetComponent<s_inventory>();
        ClearButtons();
        displayMoveName.txt.color = Color.clear;
        displayMoveName.img.color = Color.clear;

        EXPResults.SetActive(false);

        //allTargetSelectors = new selector[allTargetSelectorsObj.Length];
        for(int i =0; i < allTargetSelectorsObj.Length;i++)
        {
            GameObject g = allTargetSelectorsObj[i];
            /*
                allTargetSelectors[i] = new selector();
                selector sel = allTargetSelectors[i];
                sel.Selector = g;
                sel.enemySP = g.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Slider>();
                sel.enemyHP = g.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Slider>();
            */
            g.SetActive(false);
        }

        {
            int i = 0;
            menuButtons = new menu_button[menuButtonObjects.Length];
            foreach (GameObject g in menuButtonObjects)
            {
                menuButtons[i] = new menu_button();
                menuButtons[i].img = g.transform.GetChild(1).GetComponent<Image>();
                menuButtons[i].selector = g.transform.GetChild(0).GetComponent<Image>();
                menuButtons[i].txt = g.transform.GetChild(2).GetComponent<Text>();
                menuButtons[i].txt.text = "";
                menuButtons[i].img.sprite = null;
                menuButtons[i].selector.color = Color.clear;
                i++;
            }
            
            listOfbuttons.SetActive(false);
        }
        
    }
    public bool CheckForCostRequirements(s_move move, o_battleChar battleCharacter)
    {
        if (!battleCharacter.skillMoves.Contains(move))
            return false;
        if (move.cost == 0)
            return true;
        switch (move.moveType)
        {
            case MOVE_TYPE.PHYSICAL:
                if (move.cost < battleCharacter.hitPoints)
                    return true;
                break;

            case MOVE_TYPE.TALK:
            case MOVE_TYPE.SPECIAL:
            case MOVE_TYPE.STATUS:
                if (move.cost <= battleCharacter.skillPoints)
                    return true;
                break;
        }
        return false;
    }

    public void EnemySelectAttack(ref o_battleChar character)
    {
        if (character != null)
        {
            o_battleChar target = null;
            s_move move = null;
            List<o_battleChar> candidates = new List<o_battleChar>();

            foreach (o_battleChar p in players)
            {
                if (p.hitPoints > 0)
                    candidates.Add(p);
            }

            List<s_move> allmove = new List<s_move>();
            foreach (s_move m in allmove)
            {
                if (!CheckForCostRequirements(m, character))
                    continue;
                allmove.Add(m);
            }

            if (character.characterAI != null)
                foreach (charAI chai in character.characterAI)
                {

                    bool breakOutOfLoop = false;

                    if (target != null)
                        break;

                    o_battleChar Targ = null;
                    move = chai.moveName;
                    switch (chai.conditions)
                    {
                        case charAI.CONDITIONS.ALWAYS:
                            {
                                Targ = candidates[UnityEngine.Random.Range(0, candidates.Count - 1)];
                                if (Targ != null)
                                {
                                    target = Targ;
                                    breakOutOfLoop = true;
                                }
                            }
                            break;
                        case charAI.CONDITIONS.USER_PARTY_HP_LOWER:
                            {
                                Targ = opposition.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                                if (Targ != null)
                                {
                                    target = Targ;
                                    breakOutOfLoop = true;
                                }
                            }
                            break;
                        case charAI.CONDITIONS.USER_PARTY_HP_HIGHER:
                            {
                                Targ = opposition.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                                if (Targ != null)
                                {
                                    target = Targ;
                                    breakOutOfLoop = true;
                                }
                            }
                            break;
                        case charAI.CONDITIONS.TARGET_PARTY_HP_HIGHER:
                            {
                                Targ = candidates.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                                if (Targ != null)
                                {
                                    target = Targ;
                                    breakOutOfLoop = true;
                                }
                            }
                            break;
                        case charAI.CONDITIONS.TARGET_PARTY_HP_LOWER:
                            {
                                Targ = candidates.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                                if (Targ != null)
                                {
                                    target = Targ;
                                    breakOutOfLoop = true;
                                }
                            }
                            break;
                        case charAI.CONDITIONS.ON_TURN:
                            break;
                    }
                    if (breakOutOfLoop)
                        break;
                }
            else {
                target = candidates[UnityEngine.Random.Range(0, candidates.Count - 1)];
                if (allmove.Count == 0) {
                    Guard();
                }
                move = allmove[UnityEngine.Random.Range(0, allmove.Count - 1)];
            }
            currentMove = new s_battleAction (
                    character,
                    target,
                    move);
        }
        }

    public void ClearButtons()
    {
        fightButton.gameObject.SetActive(false);
        itemsButton.gameObject.SetActive(false);
        skillsButton.gameObject.SetActive(false);
        guardButton.gameObject.SetActive(false);
        spareButton.gameObject.SetActive(false);
        runButton.gameObject.SetActive(false);
        passButton.gameObject.SetActive(false);
    }

    public void WinBattle()
    {
    }

    public void SpawnDamageObject(int dmg, Vector2 characterPos, s_dmg.HIT_FX_TYPE flag) {
        for (int i = 0; i < damageObject.Length; i++) {
            if (!damageObject[i].isDone)
                continue;
            damageObject[i].transform.position = characterPos;

            damageObject[i].PlayAnim(dmg, flag);
            break;
        }
    }

    public IEnumerator DisplayMoveName(string moveName) {
        displayMoveName.txt.text = moveName;
        float t = 0;
        float spd = 4.75f;
        while (displayMoveName.img.color != Color.white) {
            displayMoveName.img.color = Color.Lerp(Color.clear, Color.white,t);
            displayMoveName.txt.color = Color.Lerp(Color.clear, Color.white,t);
            t += Time.deltaTime* spd;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        t = 0;
        yield return new WaitForSeconds(0.25f);
        while (displayMoveName.img.color != Color.clear)
        {
            displayMoveName.img.color = Color.Lerp(Color.white, Color.clear, t);
            displayMoveName.txt.color = Color.Lerp(Color.white, Color.clear, t);
            t += Time.deltaTime * spd;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator DamageCalculationAllEffect()
    {
        animationsDone = new bool[opposition.Count];
        for (int i = 0; i < opposition.Count; i++) {
            o_battleChar t = opposition[i];
            StartCoroutine(DamageCalculationEffect(t));
            yield return new WaitForSeconds(0.3f);
        }
    }
    public IEnumerator DamageCalculationEffect(o_battleChar Targ)
    {
        s_battleAction move = currentMove;
        Vector2 characterPos = new Vector2(0, 0);
        bool isPlayer = players.Contains(move.target);
        Vector2 plGUIPos = Vector2.zero;
        s_move mov = null;

        switch (move.type) {
            case s_battleAction.MOVE_TYPE.MOVE:
                mov = move.move;
                break;

            case s_battleAction.MOVE_TYPE.ITEM:
                mov = move.item.action;
                break;
        }

        float tm = 0.008f;
        int damage = Targ.CalculateMove(ref move);

        float buffTimer = 0.25f;
        switch (mov.moveType) {
            case MOVE_TYPE.SPECIAL:
            case MOVE_TYPE.PHYSICAL:
            case MOVE_TYPE.TALK:
                if (Targ.elementTypeCharts[(int)mov.element] > 1.99f || Targ.actionTypeCharts[(int)mov.action_type] > 1.99f)
                {
                    s_soundmanager.sound.PlaySound("critical");
                }

                if (!mov.onTeam)
                {
                    float acc1 = ((move.user.getNetSpeed / 8) * 6.25f);
                    float acc2 = ((Targ.getNetSpeed / 8) * 6.25f);

                    float accuracy_percentage = (float)move.move.accuracy * (acc1/acc2);
                   // print("User: " + move.user + " Target: " + Targ + " accuracy = " + accuracy_percentage);
                    float accuracy = UnityEngine.Random.Range(0,100);

                    if (mov.moveType != MOVE_TYPE.TALK)
                    {
                        //If the accuracy is higher than the chance the move misses
                        //TALK moves never miss
                        if (accuracy > accuracy_percentage)
                        {
                            turnPressFlag = PRESS_TURN_TYPE.MISS;
                        }
                    }
                    if (turnPressFlag != PRESS_TURN_TYPE.MISS) {
                        if (Targ.elementTypeCharts[(int)mov.element] > 1.99f ||
                                                Targ.actionTypeCharts[(int)mov.action_type] > 1.99f ||
                                                Targ.currentStatus == STATUS_EFFECT.FROZEN)
                        {
                            if (Targ.isGuarding)
                            {
                                if (Targ.currentStatus != STATUS_EFFECT.FROZEN)
                                {
                                    turnPressFlag = PRESS_TURN_TYPE.NORMAL;
                                    Targ.isGuarding = false;
                                }
                                else
                                {
                                    turnPressFlag = PRESS_TURN_TYPE.WEAKNESS;
                                }
                            }
                            else
                            {
                                turnPressFlag = PRESS_TURN_TYPE.WEAKNESS;
                            }
                        }
                        else if ((Targ.elementTypeCharts[(int)mov.element] < 0 &&
                          Targ.elementTypeCharts[(int)mov.element] >= -1) ||
                          (Targ.actionTypeCharts[(int)mov.action_type] < 0 &&
                          Targ.actionTypeCharts[(int)mov.action_type] >= -1))
                        {
                            turnPressFlag = PRESS_TURN_TYPE.REFLECT;
                        }
                        else if (Targ.elementTypeCharts[(int)mov.element] < -1 ||
                        Targ.actionTypeCharts[(int)mov.action_type] < -1)
                        {
                            //ABSORB
                            turnPressFlag = PRESS_TURN_TYPE.ABSORB;
                        }
                        else if (Targ.elementTypeCharts[(int)mov.element] == 0 ||
                            Targ.actionTypeCharts[(int)mov.action_type] == 0)
                        {
                            turnPressFlag = PRESS_TURN_TYPE.IMMUNE;
                        }
                    }
                }

                if (turnPressFlag == PRESS_TURN_TYPE.REFLECT) {
                    damage = Targ.CalculateMove(ref move) * - 1;
                    Targ = move.user;
                }
                switch (turnPressFlag)
                {
                    case PRESS_TURN_TYPE.ABSORB:
                        s_soundmanager.sound.PlaySound("heal");
                        Targ.hitPoints += damage * -1;
                        Targ.hitPoints = Mathf.Clamp(Targ.hitPoints, -Targ.maxHitPoints, Targ.maxHitPoints);
                        break;

                    case PRESS_TURN_TYPE.IMMUNE:
                        if (isPlayer)
                        {
                            plGUIPos = guis[players.IndexOf(move.target)].transform.position;
                            characterPos = Targ.transform.position;
                        }
                        else
                            characterPos = Targ.transform.position;
                        //TODO: PUT IMMUNE GRAPHIC
                        SpawnDamageObject(0, characterPos, s_dmg.HIT_FX_TYPE.BLOCK);
                        break;

                    case PRESS_TURN_TYPE.MISS:
                        SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.MISS);
                        break;

                    default:
                        if (mov.moveType != MOVE_TYPE.TALK)
                        {
                            Targ.hitPoints -= damage;
                            Targ.hitPoints = Mathf.Clamp(Targ.hitPoints, -Targ.maxHitPoints, Targ.maxHitPoints);
                            s_soundmanager.sound.PlaySound("hit2");
                        }
                        else
                        {
                            Targ.skillPoints -= damage;
                            Targ.skillPoints = Mathf.Clamp(Targ.skillPoints, 0, Targ.maxSkillPoints);
                            s_soundmanager.sound.PlaySound("hit2");
                        }
                        if (Targ.elementTypeCharts[(int)mov.element] > 0)
                        {
                            float r = UnityEngine.Random.Range(0f, 1f);
                            //print(r);

                            if (mov.statusEffectChances.statusEffectChance != 0)
                            {
                                if (r <= (mov.statusEffectChances.statusEffectChance * (float)(move.user.getNetLuck/Targ.getNetLuck)))
                                {
                                    Targ.SetStatusEffect(mov.statusEffectChances.status_effect);
                                }
                            }
                        }
                        plGUIPos = new Vector2(1, 1);
                        if (isPlayer)
                        {
                            plGUIPos = guis[players.IndexOf(move.target)].transform.position;
                            characterPos = Targ.transform.position ;
                        }
                        else
                            characterPos = Targ.transform.position ; 
                        switch (mov.moveType) {
                            case MOVE_TYPE.TALK:
                                SpawnDamageObject(damage, characterPos + new Vector2(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20)), s_dmg.HIT_FX_TYPE.DMG_SP);
                                break;
                            default:
                                SpawnDamageObject(damage, characterPos + new Vector2(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20)), s_dmg.HIT_FX_TYPE.NONE);
                                break;
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            if (isPlayer)
                            {
                                //guis[players.IndexOf(Targ)].transform.position = plGUIPos + new Vector2(15, 0);
                                Targ.transform.position = characterPos + new Vector2(15, 0);
                                yield return new WaitForSeconds(tm);
                                //guis[players.IndexOf(Targ)].transform.position = plGUIPos;
                                Targ.transform.position = characterPos;
                                yield return new WaitForSeconds(tm);
                                //guis[players.IndexOf(Targ)].transform.position = plGUIPos + new Vector2(-15, 0);
                                Targ.transform.position = characterPos + new Vector2(-15, 0);
                                yield return new WaitForSeconds(tm);
                               // guis[players.IndexOf(Targ)].transform.position = plGUIPos;
                                Targ.transform.position = characterPos;
                                yield return new WaitForSeconds(tm);
                            }
                            else
                            {
                                Targ.transform.position = characterPos + new Vector2(15, 0);
                                yield return new WaitForSeconds(tm);
                                Targ.transform.position = characterPos;
                                yield return new WaitForSeconds(tm);
                                Targ.transform.position = characterPos + new Vector2(-15, 0);
                                yield return new WaitForSeconds(tm);
                                Targ.transform.position = characterPos;
                                yield return new WaitForSeconds(tm);
                            }
                        }
                        switch (mov.statusMoveType)
                        {
                            case STATUS_MOVE_TYPE.BUFF:
                                if (mov.str_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("attk_up");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                                    yield return new WaitForSeconds(buffTimer);
                                }

                                if (mov.dex_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("int_up");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                                    yield return new WaitForSeconds(buffTimer);
                                }

                                if (mov.vit_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("def_up");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                                    yield return new WaitForSeconds(buffTimer);
                                }

                                if (mov.agi_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("agi_up");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                                    yield return new WaitForSeconds(buffTimer);
                                }

                                if (mov.gut_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("gut_up");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                                    yield return new WaitForSeconds(buffTimer);
                                }
                                if (!mov.buffUser)
                                {
                                    Targ.attackBuff += mov.str_inc;
                                    Targ.intelligenceBuff += mov.dex_inc;
                                    Targ.defenceBuff += mov.vit_inc;
                                    Targ.speedBuff += mov.agi_inc;
                                    Targ.gutsBuff += mov.gut_inc;
                                }
                                else
                                {
                                    currentCharacter.attackBuff += mov.str_inc;
                                    currentCharacter.intelligenceBuff += mov.dex_inc;
                                    currentCharacter.defenceBuff += mov.vit_inc;
                                    currentCharacter.speedBuff += mov.agi_inc;
                                    currentCharacter.gutsBuff += mov.gut_inc;
                                }
                                break;

                            case STATUS_MOVE_TYPE.DEBUFF:
                                if (mov.str_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("attk_down");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                                    yield return new WaitForSeconds(buffTimer);
                                }

                                if (mov.dex_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("int_down");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                                    yield return new WaitForSeconds(buffTimer);
                                }

                                if (mov.vit_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("def_down");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                                    yield return new WaitForSeconds(buffTimer);
                                }

                                if (mov.agi_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("agi_down");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                                    yield return new WaitForSeconds(buffTimer);
                                }

                                if (mov.gut_inc > 0)
                                {
                                    s_soundmanager.sound.PlaySound("gut_down");
                                    SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                                    yield return new WaitForSeconds(buffTimer);
                                }

                                Targ.attackBuff -= mov.str_inc;
                                Targ.intelligenceBuff -= mov.dex_inc;
                                Targ.defenceBuff -= mov.vit_inc;
                                Targ.speedBuff -= mov.agi_inc;
                                Targ.gutsBuff -= mov.gut_inc;
                                break;
                        }
                        break;
                }
                yield return new WaitForSeconds(0.05f);

                break;

            case MOVE_TYPE.STATUS:

                switch (mov.statusMoveType) {
                    case STATUS_MOVE_TYPE.HEAL:
                        s_soundmanager.sound.PlaySound("heal");
                        if (Targ.hitPoints < 0)
                        {
                            Targ.rend.color = Color.white;
                        }
                        Targ.hitPoints += damage;
                        Targ.hitPoints = Mathf.Clamp(Targ.hitPoints, -Targ.maxHitPoints, Targ.maxHitPoints);
                        SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.HEAL);
                        break;

                    case STATUS_MOVE_TYPE.HEAL_STAMINA:
                        s_soundmanager.sound.PlaySound("heal");
                        Targ.skillPoints += damage;
                        Targ.skillPoints = Mathf.Clamp(Targ.skillPoints, 0, Targ.maxSkillPoints);
                        SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.HEAL);
                        break;

                    case STATUS_MOVE_TYPE.CURE_STATUS:
                        //Targ.currentStatus = mov.statusEffectChances.;
                        break;
                    case STATUS_MOVE_TYPE.BUFF:
                        if (mov.str_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("attk_up");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                            yield return new WaitForSeconds(buffTimer);
                        }

                        if (mov.dex_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("int_up");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                            yield return new WaitForSeconds(buffTimer);
                        }

                        if (mov.vit_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("def_up");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                            yield return new WaitForSeconds(buffTimer);
                        }

                        if (mov.agi_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("agi_up");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                            yield return new WaitForSeconds(buffTimer);
                        }

                        if (mov.gut_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("gut_up");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                            yield return new WaitForSeconds(buffTimer);
                        }
                        if (!mov.buffUser)
                        {
                            Targ.attackBuff += mov.str_inc;
                            Targ.intelligenceBuff += mov.dex_inc;
                            Targ.defenceBuff += mov.vit_inc;
                            Targ.speedBuff += mov.agi_inc;
                            Targ.gutsBuff += mov.gut_inc;
                        }
                        else
                        {
                            currentCharacter.attackBuff += mov.str_inc;
                            currentCharacter.intelligenceBuff += mov.dex_inc;
                            currentCharacter.defenceBuff += mov.vit_inc;
                            currentCharacter.speedBuff += mov.agi_inc;
                            currentCharacter.gutsBuff += mov.gut_inc;
                        }
                        break;

                    case STATUS_MOVE_TYPE.DEBUFF:

                        if (mov.str_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("attk_down");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                            yield return new WaitForSeconds(buffTimer);
                        }

                        if (mov.dex_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("int_down");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                            yield return new WaitForSeconds(buffTimer);
                        }

                        if (mov.vit_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("def_down");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                            yield return new WaitForSeconds(buffTimer);
                        }

                        if (mov.agi_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("agi_down");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                            yield return new WaitForSeconds(buffTimer);
                        }

                        if (mov.gut_inc > 0)
                        {
                            s_soundmanager.sound.PlaySound("gut_down");
                            SpawnDamageObject(damage, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                            yield return new WaitForSeconds(buffTimer);
                        }

                        Targ.attackBuff -= mov.str_inc;
                        Targ.intelligenceBuff -= mov.dex_inc;
                        Targ.defenceBuff -= mov.vit_inc;
                        Targ.speedBuff -= mov.agi_inc;
                        Targ.gutsBuff -= mov.gut_inc;
                        break;

                }
                break;
        }
        if (Targ.hitPoints < 0) {
            Targ.rend.color = Color.black;
        }
    }

    public IEnumerator PlayAniamtion(s_battleAction move)
    {
        ///spawn object "move animation"
        ///select the animPrefab that has the name of the animation
        ///then depending on the animation enum excecute the animation
        switch (move.type) {
            case s_battleAction.MOVE_TYPE.MOVE:
                StartCoroutine(DisplayMoveName(move.move.name));
                break;

            case s_battleAction.MOVE_TYPE.ITEM:
                StartCoroutine(DisplayMoveName(move.item.name));
                break;
        }
        yield return new WaitForSeconds(0.65f);
        battleFx.color = Color.white;
        //Play hurt animation
        Vector2 characterPos = new Vector2(0, 0);

        bool isPlayer = players.Contains(move.target);
        int times = 1;
        s_move.s_moveAnim[] anim = null;
        s_move mov = null;


        switch (move.type) {
            case s_battleAction.MOVE_TYPE.MOVE:
                anim = move.move.animation;
                if (move.move.isMultiHit)
                    times = UnityEngine.Random.Range(1, 6);  //For now we'll hard-code a RNG in there but later on it's going to be affected by aglity
                mov = move.move;
                break;

            case s_battleAction.MOVE_TYPE.ITEM:
                anim = move.item.action.animation;
                if (move.item.action.isMultiHit)
                    times = UnityEngine.Random.Range(1, 6);  //For now we'll hard-code a RNG in there but later on it's going to be affected by aglity
                mov = move.item.action;
                break;

        }
        List<o_battleChar> characterList = new List<o_battleChar>();

        for (int i = 0; i < times; i++)
        {
            switch (mov.target)
            {
                case TARGET_MOVE_TYPE.SINGLE:

                    if (anim != null)
                    {
                        if (anim.Length > 0)
                        {
                            foreach (s_move.s_moveAnim a in anim)
                            {
                                switch (a.type)
                                {
                                    case s_move.s_moveAnim.ANIM_TYPE.ANIMATION:
                                        battleFx.sprite = a.image;
                                        switch (a.pos)
                                        {
                                            case s_move.s_moveAnim.MOVEPOSTION.ON_TARGET:
                                                battleFx.transform.position = move.target.transform.position;
                                                break;

                                            case s_move.s_moveAnim.MOVEPOSTION.FIXED:
                                                battleFx.transform.position = a.position;
                                                break;
                                        }
                                        battleFx.color = Color.white;
                                        battleFx.GetComponent<Animator>().enabled = true;
                                        // battleFx.GetComponent<Animator>().runtimeAnimatorController = a.anim;
                                        battleFx.GetComponent<Animator>().Play(a.name);
                                        yield return new WaitForSeconds(a.duration);
                                        break;

                                    case s_move.s_moveAnim.ANIM_TYPE.IMAGE:
                                        battleFx.sprite = a.image;
                                        switch (a.pos)
                                        {
                                            case s_move.s_moveAnim.MOVEPOSTION.ON_TARGET:
                                                battleFx.transform.position = move.target.transform.position;
                                                break;

                                            case s_move.s_moveAnim.MOVEPOSTION.FIXED:
                                                battleFx.transform.position = a.position;
                                                break;
                                        }
                                        battleFx.color = Color.white;
                                        yield return new WaitForSeconds(a.duration);
                                        break;

                                    case s_move.s_moveAnim.ANIM_TYPE.INFLICT_STATUS:

                                        battleFx.color = Color.clear;
                                        break;

                                    case s_move.s_moveAnim.ANIM_TYPE.CALCUATION:

                                        battleFx.color = Color.clear;
                                        yield return StartCoroutine(DamageCalculationEffect(move.target));
                                        break;
                                }
                                yield return new WaitForSeconds(a.duration);
                            }
                        } else
                        {
                            yield return StartCoroutine(DamageCalculationEffect(move.target));
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(DamageCalculationEffect(move.target));
                    }
                    break;

                case TARGET_MOVE_TYPE.RANDOM:

                    if (mov.onTeam)
                    {
                        if (players.Contains(currentCharacter))
                        {
                            characterList = players.FindAll(x => x.hitPoints > 0);
                        }
                        else
                        {
                            characterList = opposition.FindAll(x => x.hitPoints > 0);
                        }
                    }
                    else
                    {
                        if (players.Contains(currentCharacter))
                        {
                            characterList = opposition.FindAll(x => x.hitPoints > 0); ;
                        }
                        else
                        {
                            characterList = players.FindAll(x => x.hitPoints > 0); ;
                        }
                    }
                    move.target = characterList[UnityEngine.Random.Range(0, characterList.Count)];
                    if (characterList.Count == 0) {
                        times = 0;
                        break;
                    } else
                    {
                        if (anim != null)
                        {
                            if (anim.Length > 0)
                            {
                                foreach (s_move.s_moveAnim a in anim)
                                {
                                    switch (a.type)
                                    {
                                        case s_move.s_moveAnim.ANIM_TYPE.ANIMATION:
                                            battleFx.sprite = a.image;
                                            switch (a.pos)
                                            {
                                                case s_move.s_moveAnim.MOVEPOSTION.ON_TARGET:
                                                    battleFx.transform.position = move.target.transform.position;
                                                    break;

                                                case s_move.s_moveAnim.MOVEPOSTION.FIXED:
                                                    battleFx.transform.position = a.position;
                                                    break;
                                            }
                                            battleFx.color = Color.white;
                                            // battleFx.GetComponent<Animator>().runtimeAnimatorController = a.anim;
                                            battleFx.GetComponent<Animator>().Play(a.name);
                                            yield return new WaitForSeconds(a.duration);
                                            break;

                                        case s_move.s_moveAnim.ANIM_TYPE.IMAGE:
                                            battleFx.sprite = a.image;
                                            switch (a.pos)
                                            {
                                                case s_move.s_moveAnim.MOVEPOSTION.ON_TARGET:
                                                    battleFx.transform.position = move.target.transform.position;
                                                    break;

                                                case s_move.s_moveAnim.MOVEPOSTION.FIXED:
                                                    battleFx.transform.position = a.position;
                                                    break;
                                            }
                                            battleFx.color = Color.white;
                                            yield return new WaitForSeconds(a.duration);
                                            break;

                                        case s_move.s_moveAnim.ANIM_TYPE.INFLICT_STATUS:

                                            battleFx.color = Color.clear;
                                            break;

                                        case s_move.s_moveAnim.ANIM_TYPE.CALCUATION:

                                            battleFx.color = Color.clear;
                                            yield return StartCoroutine(DamageCalculationEffect(move.target));
                                            break;
                                    }
                                    yield return new WaitForSeconds(a.duration);
                                }
                            }
                            else
                            {
                                yield return StartCoroutine(DamageCalculationEffect(move.target));
                            }
                        }
                        else
                        {
                            yield return StartCoroutine(DamageCalculationEffect(move.target));
                        }
                    }
                    break;

                case TARGET_MOVE_TYPE.ALL:
                    
                    if (mov.onTeam)
                    {
                        if (players.Contains(currentCharacter))
                        {
                            characterList = players;
                        } else
                        {
                            characterList = opposition;
                        }
                    } else {
                        if (players.Contains(currentCharacter))
                        {
                            characterList = opposition;
                        }
                        else
                        {
                            characterList = players;
                        }
                    }
                    for (int i2 = 0; i2 < characterList.Count; i2++)
                    {
                        o_battleChar t = characterList[i2];
                        if (anim != null)
                        {
                            if (anim.Length > 0)
                            {
                                foreach (s_move.s_moveAnim a in anim)
                                {
                                    switch (a.type)
                                    {
                                        case s_move.s_moveAnim.ANIM_TYPE.ANIMATION:
                                            battleFx.sprite = a.image;
                                            switch (a.pos)
                                            {
                                                case s_move.s_moveAnim.MOVEPOSTION.ON_TARGET:
                                                    battleFxGroup[i2].transform.position = t.transform.position;
                                                    break;

                                                case s_move.s_moveAnim.MOVEPOSTION.FIXED:
                                                    battleFxGroup[i2].transform.position = a.position;
                                                    break;
                                            }
                                            battleFxGroup[i2].color = Color.white;
                                            // battleFx.GetComponent<Animator>().runtimeAnimatorController = a.anim;
                                            battleFxGroup[i2].GetComponent<Animator>().Play(a.name);
                                            yield return new WaitForSeconds(a.duration);
                                            break;
                                        case s_move.s_moveAnim.ANIM_TYPE.IMAGE:
                                            battleFxGroup[i2].sprite = a.image;
                                            switch (a.pos)
                                            {
                                                case s_move.s_moveAnim.MOVEPOSTION.ON_TARGET:
                                                    battleFxGroup[i2].transform.position = t.transform.position;
                                                    break;

                                                case s_move.s_moveAnim.MOVEPOSTION.FIXED:
                                                    battleFxGroup[i2].transform.position = a.position;
                                                    break;
                                            }
                                            battleFxGroup[i2].color = Color.white;
                                            yield return new WaitForSeconds(a.duration);
                                            break;

                                        case s_move.s_moveAnim.ANIM_TYPE.INFLICT_STATUS:

                                            battleFxGroup[i2].color = Color.clear;
                                            break;

                                        case s_move.s_moveAnim.ANIM_TYPE.CALCUATION:

                                            battleFxGroup[i2].color = Color.clear;
                                            StartCoroutine(DamageCalculationEffect(t));
                                            //print("Yes");
                                            break;
                                    }
                                    yield return new WaitForSeconds(a.duration);
                                }
                            } else
                            {
                                yield return StartCoroutine(DamageCalculationEffect(t));
                            }
                        }
                        else
                        {
                            yield return StartCoroutine(DamageCalculationEffect(t));
                            yield return new WaitForSeconds(0.3f);
                        }
                    }
                    break;
            }
        }

        //If the user has a status effect
        switch (move.user.currentStatus)
        {
            case STATUS_EFFECT.POISON:
                move.user.hitPoints -= move.user.maxHitPoints / 6;
                for (int i = 0; i < 2; i++)
                {
                    if (!isPlayer)
                        characterPos = guis[players.IndexOf(move.user)].transform.position;
                    else
                        characterPos = move.user.transform.position;

                    if (!isPlayer)
                    {
                        print("OK POSION");
                        guis[players.IndexOf(move.user)].transform.position = characterPos + new Vector2(15, 0);
                        yield return new WaitForSeconds(0.02f);
                        guis[players.IndexOf(move.user)].transform.position = characterPos;
                        yield return new WaitForSeconds(0.02f);
                        guis[players.IndexOf(move.user)].transform.position = characterPos + new Vector2(-15, 0);
                        yield return new WaitForSeconds(0.02f);
                        guis[players.IndexOf(move.user)].transform.position = characterPos;
                        yield return new WaitForSeconds(0.02f);
                    }
                    else
                    {
                        move.user.transform.position = characterPos + new Vector2(15, 0);
                        yield return new WaitForSeconds(0.02f);
                        move.user.transform.position = characterPos;
                        yield return new WaitForSeconds(0.02f);
                        move.user.transform.position = characterPos + new Vector2(-15, 0);
                        yield return new WaitForSeconds(0.02f);
                        move.user.transform.position = characterPos;
                        yield return new WaitForSeconds(0.02f);
                    }
                }
                break;
        }


        if (move.target != null)
        {
            if (isPlayer)
                characterPos = guis[players.IndexOf(move.target)].transform.position;
            else
                characterPos = move.target.transform.position;
        }


        //actionDisp.text += move.user.name + move.move.actionDescription + "\n";

        yield return new WaitForSeconds(0.5f);

        currentMove = null;
        EndAction(); 
        yield return null;
    }

    public void EndAction()
    {
        Menuchoice = 0;
        StartCoroutine(PressTurnIconDissapear());
    }

    public Vector3 screenTOVIEW(Vector3 transformpos)
    {
        return Camera.main.WorldToScreenPoint(transformpos);
    }

    public IEnumerator DoDefeatAnim()
    {
        yield return new WaitForSeconds(2.5f);
    }
    
    public void StartBattleRoutine() {
        StartCoroutine(StartBattle());
    }
    public IEnumerator StartBattle()
    {
        foreach (Image img in PT_GUI)
        {
            img.GetComponent<Animator>().Play("PTIconGone");
        }
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
        yield return new WaitForSeconds(0.3f);
        {
            int i = 0;
            foreach (o_battleChar img in players)
            {
                yield return StartCoroutine(TurnIconFX(TURN_ICON_FX.APPEAR, i));
                i++;
            }
        }
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(PollBattleEvent());
        menu_state = MENUSTATE.MENU;
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.IDLE;
    }

    public IEnumerator CheckForStatusEffect()
    {
        if (currentCharacter == null)
            yield return null;
        else
        {
            switch (currentCharacter.currentStatus)
            {
                case STATUS_EFFECT.POISON:

                    break;

            }
            yield return new WaitForSeconds(1.2f);
        }
    }

    public IEnumerator GameOverThingy()
    {
        //actionDisp.text = "Evan: (This isn't the end of it... there must be a way... there must be...)"+ "\n";

        yield return StartCoroutine(rpg_globals.gl.Fade(true));

        Destroy(rpg_globals.gl.gameObject);
        Destroy(gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

    public IEnumerator DefeatedEnemies()
    {
        actionDisp.text += "Defeated all enemies" + "\n";
        yield return StartCoroutine(ResultsShow(opposition.ToArray(), 1));
        yield return StartCoroutine(ConcludeBattle());
    }

    public IEnumerator ConcludeBattle() {
        //Fade
        EXPResults.SetActive(false);
        oppositionCharacterTurnQueue.Clear();
        playerCharacterTurnQueue.Clear();
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(rpg_globals.gl.Fade(true));

        foreach (o_battleChar c in players) {
            rpg_globals.gl.SetStats(c);
        }

        if (enemyGroup.endEvent == null)
            EndBattle();
        else
            EndBattle(enemyGroup.endEvent);

        if(encObj != null)
            if (encObj.destroyEnemies)
            {
                encObj.DestroyAllEnemies();
            }

        yield return StartCoroutine(rpg_globals.gl.Fade(false));
    }
    /*
    public IEnumerator BattleEventPlay(s_battleEvents ev) {

        foreach (s_bEvent beElement in ev.events)
        {
            switch (beElement.battleAction)
            {
                case s_bEvent.B_ACTION_TYPE.DIALOGUE:

                    actionDisp.text = beElement.string0 + "\n";
                    yield return new WaitForSeconds(1.5f);
                    break;
                case s_bEvent.B_ACTION_TYPE.MOVE:

                    actionDisp.text = beElement.string0 + "\n";
                    //yield return StartCoroutine(PlayAniamtion());
                    break;

                case s_bEvent.B_ACTION_TYPE.CHECK_FLAG:

                    break;

                case s_bEvent.B_ACTION_TYPE.END_BATTLE:
                    if (beElement.name == "")
                        EndBattle();
                    else
                        EndBattle(beElement.eventScript);
                    break;
            }
        }
    }
    */

    public IEnumerator PollBattleEvent() {
        yield return CheckForStatusEffect();
        bool conditionFufilled = false;
        if (OnBattleEvents != null)
            foreach (s_battleEvents be in OnBattleEvents)
            {
                if (be.enabled)
                {
                    switch (be.battleCheckCond)
                    {
                        case s_battleEvents.B_CHECK_COND.ON_START:

                            switch (be.battleCond)
                            {
                                /*
                                case s_battleEvents.B_COND.HEALTH:
                                    if (opposition.Find(x => x.name == be.name).hitPoints == be.int0)
                                    {
                                        conditionFufilled = true;
                                    }
                                    break;
                                    */

                                case s_battleEvents.B_COND.TURNS_ELAPSED:
                                    if (roundNum == be.int0)
                                    {
                                        conditionFufilled = true;
                                        roundNum = 0;
                                    }
                                    break;

                            }
                            if (conditionFufilled)
                            {
                                s_rpgEvent.rpgEv.JumpToEvent(be.eventScript);
                                isCutscene = true;
                                be.enabled = false;

                                yield return null;
                            }
                            break;

                        case s_battleEvents.B_CHECK_COND.PER_TURN:

                            if (be.enabled)
                            {
                                switch (be.battleCond)
                                {
                                    /*
                                    case s_battleEvents.B_COND.HEALTH:
                                        if (opposition.Find(x => x.name == be.name).hitPoints == be.int0)
                                        {
                                            conditionFufilled = true;
                                        }
                                        break;
                                        */

                                    case s_battleEvents.B_COND.TURNS_ELAPSED:
                                        if (roundNum == be.int0)
                                        {
                                            s_rpgEvent.rpgEv.JumpToEvent(be.eventScript);//yield return StartCoroutine();
                                            isCutscene = true;
                                            be.enabled = false;
                                            roundNum = 0;
                                            goto l1a;
                                        }
                                        break;

                                }
                            }
                            else
                                continue;
                            break;

                    }

                }
                else
                    continue;
                    
            }
        l1a:
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.IDLE;
    }

    public IEnumerator PressTurnIconDissapear() {

        currentMove = null;
        battleFx.color = Color.clear;
        //Color pre = pressTurnIcons[netTurn - 1].color;
        float t = 0;
        switch (turnPressFlag)
        {
            case PRESS_TURN_TYPE.NORMAL:
                /*
                while (t < 1)
                {
                    t += Time.deltaTime;
                    pressTurnIcons[netTurn - 1].color = Color.Lerp(pre, Color.clear, t);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                */
                NextTurn();
                yield return StartCoroutine(TurnIconFX(TURN_ICON_FX.FADE, netTurn));
                break;

            case PRESS_TURN_TYPE.WEAKNESS:
                if (pressTurn > 0)
                {
                    HitWeakness();
                    yield return StartCoroutine(TurnIconFX(TURN_ICON_FX.HIT, netTurn - halfTurn));
                }
                else
                {
                    HitWeakness();
                    yield return StartCoroutine(TurnIconFX(TURN_ICON_FX.FADE, netTurn));
                }
                break;

            case PRESS_TURN_TYPE.PASS:
                /*
                while (t < 1)
                {
                    t += Time.deltaTime;
                    pressTurnIcons[netTurn - 1].color = Color.Lerp(pre, Color.magenta, t);
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                */
                if (halfTurn == 0)
                {
                    PassTurn();
                    yield return StartCoroutine(TurnIconFX(TURN_ICON_FX.HIT, netTurn - halfTurn));
                }
                else
                {
                    PassTurn();
                    yield return StartCoroutine(TurnIconFX(TURN_ICON_FX.FADE, netTurn));
                }
                break;

            case PRESS_TURN_TYPE.MISS:
            case PRESS_TURN_TYPE.IMMUNE:
                NextTurn();
                StartCoroutine(TurnIconFX(TURN_ICON_FX.FADE, netTurn));
                NextTurn();
                StartCoroutine(TurnIconFX(TURN_ICON_FX.FADE, netTurn));
                yield return new WaitForSeconds(0.35f);
                break;

            case PRESS_TURN_TYPE.REFLECT:
            case PRESS_TURN_TYPE.ABSORB:
                for (int i = netTurn; i > 0; i--)
                {
                    StartCoroutine(TurnIconFX(TURN_ICON_FX.FADE, i - 1));
                    yield return new WaitForSeconds(0.15f);
                }
                yield return new WaitForSeconds(0.2f);
                halfTurn = 0;
                pressTurn = 0;
                break;
        }
        /*
        if (conditionFufilled != true)
            middleOfAction = false;
        */
        yield return new WaitForSeconds(0.2f);
        turnPressFlag = PRESS_TURN_TYPE.NORMAL;
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.END;
    }

    public void EndBattle()
    {
        currentMove = null;
        playerCharacterTurnQueue.Clear();
        oppositionCharacterTurnQueue.Clear();
        rpg_globals.gl.SwitchToOverworld();
    }
    public void EndBattle(MagnumFoundation2.Objects.ev_script even)
    {
        currentMove = null;
        playerCharacterTurnQueue.Clear();
        oppositionCharacterTurnQueue.Clear();
        rpg_globals.gl.SwitchToOverworld();
        MagnumFoundation2.System.s_triggerhandler.trigSingleton.JumpToEvent(even);
    }

    public void DrawPressTurn() {
        for (int i = netTurn; i > 0; i--) {
            //Image ptI = pressTurnIcons[i];
            Rect rectan = new Rect(300 +(20 * i), 20, 30, 30);
            if (i % 2 == 0)
            {
                rectan = new Rect(300 + (15 * i), 40, 30, 30);
            }

            if (netTurn < i)
                GUI.DrawTexture(rectan, pressTurnIcon);
            else
            {
                if (halfTurn + 1 > i)
                {
                    GUI.color = Color.red;
                    GUI.DrawTexture(rectan, pressTurnIcon);
                }
                else
                {
                    GUI.color = Color.white;
                    GUI.DrawTexture(rectan, pressTurnIcon);
                }
            }
        } 
    
    }

    public void RunTurn()
    {
        StartCoroutine(ConcludeBattle());
        /*
         * turnPressFlag = PRESS_TURN_TYPE.NORMAL;
        if (fleeTurns > 0)
            fleeTurns++;
        if (fleeTurns == 5) {
        }
        else
        {
            CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_PROCESS;
            EndAction();
        }
        */
    }
    public void Guard() {
        turnPressFlag = PRESS_TURN_TYPE.NORMAL;
        currentCharacter.isGuarding = true;
        currentCharacter.skillPoints += Mathf.RoundToInt(30 * currentCharacter.guts / 25);
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_PROCESS;
        EndAction();
    }
    
    public void HighlightMenuButton(ref Image img) {
        menuSelector.rectTransform.position = img.rectTransform.position;
    }
    
    public o_battleChar GetTargetCharacter(List<o_battleChar> candidates, charAI chai)
    {
        float res = 0;
        int rand = 0;
        o_battleChar Targ = null;
        switch (chai.conditions)
        {
            case charAI.CONDITIONS.ALWAYS:
                rand = UnityEngine.Random.Range(0, candidates.Count);
                print("candiate num " + rand);
                Targ = candidates[rand];
                break;
            case charAI.CONDITIONS.USER_PARTY_HP_LOWER:
                Targ = candidates.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                break;
            case charAI.CONDITIONS.USER_PARTY_HP_HIGHER:
                Targ = candidates.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                break;
            case charAI.CONDITIONS.TARGET_PARTY_HP_HIGHER:
                Targ = candidates.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                break;
            case charAI.CONDITIONS.TARGET_PARTY_HP_LOWER:
                Targ = candidates.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                break;
            case charAI.CONDITIONS.TARGET_PARTY_HP_HIGHEST:
                Targ = candidates[0];
                res = 0;
                foreach (o_battleChar bc in candidates)
                {
                    if (bc.hitPoints > res)
                    {
                        res = bc.hitPoints;
                        Targ = bc;
                    }
                }
                break;
            case charAI.CONDITIONS.TARGET_PARTY_HP_LOWEST:
                Targ = candidates[0];
                res = float.MaxValue;
                foreach (o_battleChar bc in candidates)
                {
                    if (bc.hitPoints < res)
                    {
                        res = bc.hitPoints;
                        Targ = bc;
                    }
                }
                break;
            case charAI.CONDITIONS.ELEMENT_TARG:
                foreach (o_battleChar bc in candidates)
                {
                    if (bc.elementTypeCharts[(int)chai.moveName.element] > 1.99f)
                    {
                        Targ = bc;
                    }
                }
                break;
            case charAI.CONDITIONS.ON_TURN:
                break;
        }
        if (Targ == null)
        {
            switch (chai.conditions) {
                default:
                    rand = UnityEngine.Random.Range(0, candidates.Count - 1);
                    print(rand);
                    Targ = candidates[rand];
                    break;

                case charAI.CONDITIONS.USER_PARTY_HP_LOWER:
                case charAI.CONDITIONS.TARGET_PARTY_HP_HIGHER:
                case charAI.CONDITIONS.TARGET_PARTY_HP_LOWER:
                case charAI.CONDITIONS.USER_PARTY_HP_HIGHER:
                    return null;
            }
        }
        return Targ;
    }

    void Update()
    {
        if (isActive)
        {
            //DrawPressTurn();
            //pressTurnText.text = "Turns: " + pressTurn + " Half turns: " + halfTurn + " Total turns: " + netTurn;
            DisplayPlayerStats();
            if (!isCutscene)
            {
                switch (CurrentBattleEngineState)
                {
                    case BATTLE_ENGINE_STATE.IDLE:

                        if (isPlayerTurn)
                        {
                            currentCharacter = playerCharacterTurnQueue.Peek();

                            if (currentCharacter != null)
                            {
                                if (currentCharacter.hitPoints <= 0 || currentCharacter.skillPoints == -currentCharacter.maxSkillPoints)
                                {
                                    playerCharacterTurnQueue.Dequeue();
                                    playerCharacterTurnQueue.Enqueue(currentCharacter);
                                    currentCharacter = playerCharacterTurnQueue.Peek();
                                    print(currentCharacter);
                                }
                                else
                                {
                                    s_menuhandler.GetInstance().SwitchMenu("BattleMainMenu");

                                    //
                                    CurrentBattleEngineState = BATTLE_ENGINE_STATE.DECISION;
                                    menu_state = MENUSTATE.MENU;

                                }
                            }
                        }
                        else
                        {
                            currentCharacter = oppositionCharacterTurnQueue.Peek();
                            CurrentBattleEngineState = BATTLE_ENGINE_STATE.DECISION;
                        }
                        break;

                    case BATTLE_ENGINE_STATE.DECISION:
                        switch (currentCharacter.currentStatus)
                        {
                            default:
                                if (isPlayerTurn)
                                {
                                    /*
                                    if (currentCharacter.skillPoints < 0)
                                    {
                                        currentCharacter.skillPoints += UnityEngine.Random.Range(2, 6);
                                        ClearButtons();
                                        Menuchoice = 0;
                                        actionDisp.text = currentCharacter.name + " started wasting time.";
                                        currentCharacter.charge = 0;
                                        currentCharacter = null;
                                        menu_state = MENUSTATE.MENU;
                                        return;
                                    }
                                    */

                                    if (Input.GetKeyDown(s_globals.GetKeyPref("left")))
                                    {
                                        Menuchoice--;
                                        //s_soundmanager.sound.PlaySound(ref moveCursorSelect, false);
                                    }

                                    if (Input.GetKeyDown(s_globals.GetKeyPref("right")))
                                    {
                                        Menuchoice++;
                                        //s_soundmanager.sound.PlaySound(ref moveCursorSelect, false);
                                    }
                                    if (Input.GetKeyDown(s_globals.GetKeyPref("up")))
                                    {
                                        Menuchoice -= 4;
                                        //s_soundmanager.sound.PlaySound(ref moveCursorSelect, false);
                                    }

                                    if (Input.GetKeyDown(s_globals.GetKeyPref("down")))
                                    {
                                        Menuchoice += 4;
                                        //s_soundmanager.sound.PlaySound(ref moveCursorSelect, false);
                                    }

                                    switch (menu_state)
                                    {
                                        case MENUSTATE.MENU:
                                            //currentCharacter.itemUsed = null;

                                            if (battleOptions == null)
                                            {
                                                battleOptions = new List<string>();
                                                battleOptions.Add("attack");
                                                if (currentCharacter.skillMoves.Count > 0)
                                                {
                                                    battleOptions.Add("skills");
                                                }
                                                if (rpg_globals.gl.inventory.Count > 0)
                                                {
                                                    battleOptions.Add("items");
                                                }
                                                battleOptions.Add("guard");
                                                battleOptions.Add("analyze");
                                                battleOptions.Add("pass");

                                                bool spareButtonOn = false;

                                                if (enemyGroup.allRelations)
                                                {
                                                    if (opposition.FindAll(x => x.skillPoints <= 0) != null)
                                                    {
                                                        if (opposition.FindAll(x => x.skillPoints <= 0).Count ==
                                                            opposition.FindAll(x => x.hitPoints > 0).Count)
                                                        {
                                                            spareButtonOn = true;
                                                        }
                                                        else
                                                        {
                                                            spareButtonOn = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        spareButtonOn = false;
                                                    }

                                                }
                                                /*
                                                else
                                                {
                                                    if (enemyGroup.relationships.Length > 0)
                                                        foreach (enemy_group.s_relationship eg in enemyGroup.relationships)
                                                        {
                                                            if (opposition[eg.targetID].skillPoints == 0 &&
                                                                opposition[eg.characterID].skillPoints > 0)
                                                            {
                                                                spareButtonOn = false;
                                                            }
                                                        }
                                                    else
                                                    {
                                                        if (opposition.FindAll(x => x.skillPoints <= 0) != null)
                                                            if (opposition.FindAll(x => x.skillPoints <= 0).Count > 0)
                                                                spareButtonOn = true;
                                                    }
                                                }
                                                */
                                                if (spareButtonOn)
                                                {
                                                    battleOptions.Add("spare");
                                                }
                                                if (enemyGroup.isFleeable)
                                                    battleOptions.Add("run");

                                                StartCoroutine(ShowButtons(battleOptions, true));

                                                menuchoice_leng = battleOptions.Count - 1;
                                            }
                                            else
                                            {

                                                Menuchoice = Mathf.Clamp(Menuchoice, 0, menuchoice_leng);
                                                switch (battleOptions[Menuchoice])
                                                {
                                                    case "attack":
                                                        HighlightMenuButton(ref fightButton);
                                                        break;

                                                    case "guard":
                                                        HighlightMenuButton(ref guardButton);
                                                        break;

                                                    case "analyze":
                                                        HighlightMenuButton(ref analyzeButton);
                                                        break;

                                                    case "skills":
                                                        HighlightMenuButton(ref skillsButton);
                                                        break;

                                                    case "items":
                                                        HighlightMenuButton(ref itemsButton);
                                                        break;

                                                    case "spare":
                                                        HighlightMenuButton(ref spareButton);
                                                        break;

                                                    case "pass":
                                                        HighlightMenuButton(ref passButton);
                                                        break;

                                                    case "run":
                                                        HighlightMenuButton(ref runButton);
                                                        break;

                                                }
                                                if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                                                {
                                                    switch (battleOptions[Menuchoice])
                                                    {
                                                        case "attack":
                                                            onTeam = false;
                                                            currentCharacter.itemUsed = null;
                                                            currentMove = new s_battleAction(currentCharacter, null, normalAttack);
                                                            CurrentBattleEngineState = BATTLE_ENGINE_STATE.TARGET;
                                                            break;

                                                        case "guard":

                                                            print(Mathf.RoundToInt(30 * currentCharacter.guts / 25));
                                                            Guard();
                                                            break;

                                                        case "skills":

                                                            currentCharacter.move_typ = MOVE_TYPE.SPECIAL;
                                                            StartCoroutine(SetMenuBox(currentCharacter.skillMoves, true));
                                                            //ClearButtons();
                                                            menu_state = MENUSTATE.SKILLS;
                                                            break;

                                                        case "items":

                                                            Menuchoice = 0;
                                                            StartCoroutine(SetMenuBox(rpg_globals.gl.inventory, true));
                                                            menu_state = MENUSTATE.ITEM;
                                                            break;

                                                        case "spare":
                                                            CurrentBattleEngineState = BATTLE_ENGINE_STATE.NEGOTIATE_MENU;
                                                            break;

                                                        case "analyze":
                                                            menu_state = MENUSTATE.ANALYZE;
                                                            break;

                                                        case "pass":
                                                            turnPressFlag = PRESS_TURN_TYPE.PASS;
                                                            //EndPlayerTurn();
                                                            CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_PROCESS;
                                                            EndAction();
                                                            break;

                                                        case "run":
                                                            StartCoroutine(ConcludeBattle());
                                                            //Will loose money and enemy will not dissapear
                                                            //RunTurn();
                                                            break;

                                                    }
                                                    StartCoroutine(ShowButtons(battleOptions, false));
                                                    battleOptions = null;
                                                    //s_soundmanager.sound.PlaySound(ref selectOption, false);
                                                }
                                            }
                                            break;

                                        case MENUSTATE.ANALYZE:
                                            for (int i = 0; i < analyzeGUIs.Length; i++)
                                            {
                                                s_analyzeStats st = analyzeGUIs[i];
                                                if (st.battleChar.name != "")
                                                    st.SetText();
                                            }
                                            if (Input.GetKeyDown(s_globals.GetKeyPref("back")))
                                            {
                                                for (int i = 0; i < analyzeGUIs.Length; i++)
                                                {
                                                    s_analyzeStats st = analyzeGUIs[i];
                                                    st.HideText();
                                                }
                                                menu_state = MENUSTATE.MENU;
                                            }
                                            break;

                                        case MENUSTATE.ITEM:
                                            Menuchoice = Mathf.Clamp(Menuchoice, 0, rpg_globals.gl.inventory.Count);
                                            if (Input.GetKeyDown(s_globals.GetKeyPref("back")))
                                            {
                                                Menuchoice = 0;
                                                menu_state = MENUSTATE.MENU;
                                                StartCoroutine(SetMenuBox(rpg_globals.gl.inventory, false));
                                            }
                                            if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                                            {
                                                rpg_item itemToUse = rpg_globals.gl.inventoryData.Find(x => x.name == currentItem);
                                                bool canUseItem = false;
                                                switch (itemToUse.action.moveType)
                                                {
                                                    case MOVE_TYPE.STATUS:
                                                        switch (itemToUse.action.statusMoveType)
                                                        {
                                                            case STATUS_MOVE_TYPE.CURE_STATUS:
                                                                if (players.FindAll(x => x.currentStatus == itemToUse.action.statusEffectChances.status_effect).Count > 0)
                                                                {
                                                                    print(itemToUse.action.statusEffectChances.status_effect);
                                                                    onTeam = true;
                                                                    canUseItem = true;
                                                                }
                                                                break;

                                                            case STATUS_MOVE_TYPE.HEAL:
                                                                if (players.FindAll(x => x.hitPoints < x.maxHitPoints) != null)
                                                                {
                                                                    onTeam = true;
                                                                    canUseItem = true;
                                                                }
                                                                break;
                                                        }
                                                        break;
                                                }
                                                if (canUseItem)
                                                {
                                                    currentMove = new s_battleAction(currentCharacter, null, itemToUse);
                                                    Menuchoice = 0;
                                                    CurrentBattleEngineState = BATTLE_ENGINE_STATE.TARGET;
                                                    StartCoroutine(SetMenuBox(currentCharacter.skillMoves, false));
                                                }
                                                //currentCharacter.itemUsed = rpg_globals.gl.inventory[currentItem];
                                                //s_soundmanager.sound.PlaySound(selectOption);
                                            }
                                            break;

                                        case MENUSTATE.SKILLS:
                                            Menuchoice = Mathf.Clamp(Menuchoice, 0, currentCharacter.skillMoves.Count - 1);
                                            SetCursorMenu(currentCharacter.skillMoves);
                                            if (Input.GetKeyDown(s_globals.GetKeyPref("back")))
                                            {
                                                Menuchoice = 0;
                                                menu_state = MENUSTATE.MENU;
                                                StartCoroutine(SetMenuBox(currentCharacter.skillMoves, false));
                                                menuBoxOpen = true;
                                            }
                                            if (CheckForCostRequirements(currentCharacter.skillMoves[Menuchoice], currentCharacter))
                                            {
                                                if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                                                {
                                                    currentCharacter.CurrentMoveNum = Menuchoice;
                                                    currentCharacter.currentMove = currentCharacter.skillMoves[Menuchoice];
                                                    currentMove = new s_battleAction(currentCharacter, null, currentCharacter.skillMoves[Menuchoice]);
                                                    Menuchoice = 0;
                                                    CurrentBattleEngineState = BATTLE_ENGINE_STATE.TARGET;
                                                    onTeam = currentCharacter.currentMove.onTeam;
                                                    StartCoroutine(SetMenuBox(currentCharacter.skillMoves, false));
                                                    //s_soundmanager.sound.PlaySound(ref selectOption, false);
                                                }
                                            }
                                            break;

                                        case MENUSTATE.SPARE:
                                            sparable = new List<o_battleChar>();
                                            sparable = opposition.FindAll(x => x.skillPoints <= 0);

                                            Menuchoice = Mathf.Clamp(Menuchoice, 0, sparable.Count - 1);
                                            currentCharacter.targetCharacter = sparable[Menuchoice];
                                            if (Input.GetKeyDown(s_globals.GetKeyPref("back")))
                                            {
                                                Menuchoice = 0;
                                                menu_state = MENUSTATE.MENU;
                                            }
                                            if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                                            {
                                                ClearButtons();
                                                CurrentBattleEngineState = BATTLE_ENGINE_STATE.NEGOTIATE_MENU;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    o_battleChar target = null;
                                    s_move move = null;
                                    List<o_battleChar> candidates = new List<o_battleChar>();
                                    List<o_battleChar> partyCandidates = new List<o_battleChar>();

                                    foreach (o_battleChar p in players)
                                    {
                                        if (p.hitPoints > 0)
                                            candidates.Add(p);
                                    }
                                    foreach (o_battleChar p in opposition)
                                    {
                                        if (p.hitPoints > 0)
                                            partyCandidates.Add(p);
                                    }

                                    List<s_move> allmove = new List<s_move>();
                                    if (currentCharacter.skillMoves != null)
                                        allmove = currentCharacter.skillMoves;

                                    if (currentCharacter.characterAI != null && currentCharacter.characterAI.Length > 0)
                                    {
                                        //Check through all of these and see which one is important, and check if the conditions are met - it will pick the highest priority one
                                        charAI important = currentCharacter.characterAI.ToList().Find(
                                            x => x.isImportant &&
                                            CheckForCostRequirements(x.moveName, currentCharacter));
                                        //Then check if a target can be found
                                        if (important != null)
                                        {
                                            if (important.moveName.onTeam)
                                                target = GetTargetCharacter(partyCandidates, important);
                                            else
                                                target = GetTargetCharacter(candidates, important);
                                            if (target == null)
                                                important = null;
                                            else
                                                move = important.moveName;
                                            //print(move.name);
                                            print(currentCharacter.name);
                                            //print(target.name);
                                        }
                                        //After the function check if the important move is null
                                        if (important == null)
                                        {
                                            List<charAI> nonImportant = new List<charAI>();
                                            nonImportant = currentCharacter.characterAI.ToList().FindAll(
                                                x => !x.isImportant &&
                                                CheckForCostRequirements(x.moveName, currentCharacter));

                                            if (nonImportant == null)
                                            {
                                                target = candidates[UnityEngine.Random.Range(0, candidates.Count - 1)];
                                                move = normalAttack;
                                            }
                                            else
                                            {
                                                if (nonImportant.Count > 0)
                                                {
                                                    charAI moveAI = nonImportant[UnityEngine.Random.Range(0, nonImportant.Count - 1)];

                                                    if (moveAI.moveName.onTeam)
                                                        target = GetTargetCharacter(partyCandidates, moveAI);
                                                    else
                                                        target = GetTargetCharacter(candidates, moveAI);
                                                    if (target == null)
                                                    {
                                                        if (moveAI.moveName.onTeam)
                                                            target = partyCandidates[UnityEngine.Random.Range(0, partyCandidates.Count - 1)];
                                                        else
                                                            target = candidates[UnityEngine.Random.Range(0, candidates.Count - 1)];
                                                    }
                                                    move = moveAI.moveName;
                                                }
                                                else
                                                {
                                                    target = candidates[UnityEngine.Random.Range(0, candidates.Count - 1)];
                                                    move = normalAttack;
                                                }
                                            }
                                            //print(move.name);
                                            //print(currentCharacter.name);
                                            //print(target.name);
                                        }

                                    }
                                    else
                                    {
                                        //print("I've got nothing, man!");
                                        target = candidates[UnityEngine.Random.Range(0, candidates.Count - 1)];
                                        move = normalAttack;
                                    }
                                    currentMove = new s_battleAction(currentCharacter, target, move);

                                    CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_START;
                                }
                                break;

                            case STATUS_EFFECT.SLEEP:
                                EndAction();
                                CurrentBattleEngineState = BATTLE_ENGINE_STATE.END;
                                break;

                            case STATUS_EFFECT.PARALYZED:
                                if (currentCharacter.statusDur % 2 == 0)
                                {
                                    EndAction();
                                    CurrentBattleEngineState = BATTLE_ENGINE_STATE.END;
                                }
                                break;

                        }
                        break;

                    case BATTLE_ENGINE_STATE.TARGET:

                        switch (currentMove.type)
                        {
                            case s_battleAction.MOVE_TYPE.ITEM:

                                if (currentMove.item.action.target == TARGET_MOVE_TYPE.SINGLE)
                                {

                                    if (Input.GetKeyDown(s_globals.GetKeyPref("left")))
                                    {
                                        Menuchoice--;
                                        //s_soundmanager.sound.PlaySound(ref moveCursorSelect, false);
                                    }

                                    if (Input.GetKeyDown(s_globals.GetKeyPref("right")))
                                    {
                                        Menuchoice++;
                                        //s_soundmanager.sound.PlaySound(ref moveCursorSelect, false);
                                    }
                                    enemyHealth.SetActive(true);
                                    if (onTeam)
                                    {
                                        Menuchoice = Mathf.Clamp(Menuchoice, 0, players.Count - 1);
                                        currentMove.target = players[Menuchoice];
                                    }
                                    else
                                    {
                                        List<o_battleChar> bcs = opposition.FindAll(x => x.hitPoints > 0);
                                        Menuchoice = Mathf.Clamp(Menuchoice, 0, bcs.Count - 1);
                                        currentMove.target = bcs[Menuchoice];
                                    }

                                    if (currentMove.target != null)
                                    {
                                        enemyHealth.transform.position = screenTOVIEW(currentMove.target.transform.position);
                                    }
                                }
                                else
                                {
                                    Menuchoice = 0;
                                }
                                break;

                            case s_battleAction.MOVE_TYPE.MOVE:
                                if (currentMove.move.target == TARGET_MOVE_TYPE.SINGLE)
                                {
                                    List<o_battleChar> bcs = opposition.FindAll(x => x.hitPoints > 0);

                                    if (!currentMove.move.onSelf)
                                    {

                                        if (Input.GetKeyDown(s_globals.GetKeyPref("left")))
                                        {
                                            Menuchoice--;
                                            //s_soundmanager.sound.PlaySound(ref moveCursorSelect, false);
                                        }

                                        if (Input.GetKeyDown(s_globals.GetKeyPref("right")))
                                        {
                                            Menuchoice++;
                                            //s_soundmanager.sound.PlaySound(ref moveCursorSelect, false);
                                        }
                                        if (onTeam)
                                        {
                                            Menuchoice = Mathf.Clamp(Menuchoice, 0, players.Count - 1);
                                            currentMove.target = players[Menuchoice];
                                        }
                                        else
                                        {
                                            Menuchoice = Mathf.Clamp(Menuchoice, 0, bcs.Count - 1);
                                            currentMove.target = bcs[Menuchoice];
                                        }
                                    }
                                    else
                                    {
                                        currentMove.target = currentCharacter;
                                    }
                                    enemyHealth.SetActive(true);


                                    if (currentMove.target != null)
                                    {
                                        enemyHealth.transform.position = screenTOVIEW(currentMove.target.transform.position);
                                    }
                                }
                                else
                                {
                                    Menuchoice = 0;
                                }
                                break;
                        }

                        if (currentMove.move != null)
                        {
                            o_battleChar tc = null;
                            float HP = 0;
                            float SP = 0;
                            switch (currentMove.move.target)
                            {
                                case TARGET_MOVE_TYPE.SINGLE:

                                    if (currentMove.target != null)
                                    {
                                        tc = currentMove.target;

                                        singleTargSelector.Selector.SetActive(true);
                                        HP = ((float)tc.hitPoints / (float)tc.maxHitPoints) * 100;
                                        SP = ((float)tc.skillPoints / (float)tc.maxSkillPoints) * 100;
                                        HP = Mathf.Round(HP);
                                        SP = Mathf.Round(SP);

                                        singleTargSelector.enemyHP.value = HP;
                                        singleTargSelector.enemySP.value = SP;
                                        //print(tc.elementTypeCharts[(int)currentMove.move.element]);
                                        switch (currentMove.move.moveType)
                                        {
                                            case MOVE_TYPE.TALK:
                                                if (tc.actionTypeCharts[(int)currentMove.move.action_type] >= 2)
                                                    singleTargSelector.weaknessTarg.sprite = weakDmgTargImage;
                                                else if (tc.actionTypeCharts[(int)currentMove.move.action_type] <= 0)
                                                    singleTargSelector.weaknessTarg.sprite = resDmgTargImage;
                                                else if (tc.actionTypeCharts[(int)currentMove.move.action_type] > 0 && tc.actionTypeCharts[(int)currentMove.move.action_type] < 2)
                                                    singleTargSelector.weaknessTarg.sprite = normalDmgTargImage;
                                                break;
                                            case MOVE_TYPE.SPECIAL:
                                            case MOVE_TYPE.PHYSICAL:
                                                if (tc.elementTypeCharts[(int)currentMove.move.element] >= 2)
                                                    singleTargSelector.weaknessTarg.sprite = weakDmgTargImage;
                                                else if (tc.elementTypeCharts[(int)currentMove.move.element] <= 0)
                                                    singleTargSelector.weaknessTarg.sprite = resDmgTargImage;
                                                else if (tc.elementTypeCharts[(int)currentMove.move.element] > 0 && tc.elementTypeCharts[(int)currentMove.move.element] < 2)
                                                    singleTargSelector.weaknessTarg.sprite = normalDmgTargImage;
                                                break;
                                        }

                                    }
                                    break;

                                case TARGET_MOVE_TYPE.RANDOM:
                                case TARGET_MOVE_TYPE.ALL:
                                    for (int i = 0; i < opposition.Count; i++)
                                    {
                                        tc = opposition[i];
                                        allTargetSelectors[i].Selector.SetActive(true);
                                        allTargetSelectors[i].Selector.transform.position = screenTOVIEW(tc.transform.position);
                                        HP = ((float)tc.hitPoints / (float)tc.maxHitPoints) * 100;
                                        SP = ((float)tc.skillPoints / (float)tc.maxSkillPoints) * 100;
                                        HP = Mathf.Round(HP);
                                        SP = Mathf.Round(SP);

                                        if (tc.elementTypeCharts[(int)currentMove.move.element] >= 2)
                                            allTargetSelectors[i].weaknessTarg.sprite = weakDmgTargImage;
                                        else if (tc.elementTypeCharts[(int)currentMove.move.element] <= 0)
                                            allTargetSelectors[i].weaknessTarg.sprite = resDmgTargImage;
                                        else if (tc.elementTypeCharts[(int)currentMove.move.element] > 0 && tc.elementTypeCharts[(int)currentMove.move.element] < 2)
                                            allTargetSelectors[i].weaknessTarg.sprite = normalDmgTargImage;

                                        allTargetSelectors[i].enemyHP.value = HP;
                                        allTargetSelectors[i].enemySP.value = SP;
                                    }
                                    break;

                            }
                            //enemyHPSP.text = bc.name + " " + "HP: " + HP + "%" + " SP:" + SP + "%";
                            ///Will put other things like weaknesses and strengths
                            ///If resistance is 0, they are immune, 
                            ///If above 0 but less than 1, resistant
                            ///If 1 or above and less than 2, normal damage /x1 damage
                            ///If 2 or above, double damage /x2+ damage
                            ///If less than 0 but more than -2, reflect
                            ///If -2 or less, absorb
                        }
                        else
                        {
                            o_battleChar tc = null;
                            float HP = 0;
                            float SP = 0;
                            switch (currentMove.item.action.target)
                            {
                                case TARGET_MOVE_TYPE.SINGLE:

                                    if (currentMove.target != null)
                                    {
                                        tc = currentMove.target;

                                        HP = ((float)tc.hitPoints / (float)tc.maxHitPoints) * 100;
                                        SP = ((float)tc.skillPoints / (float)tc.maxSkillPoints) * 100;
                                        HP = Mathf.Round(HP);
                                        SP = Mathf.Round(SP);

                                        enemyHP.value = HP;
                                        enemySP.value = SP;
                                    }
                                    break;

                                case TARGET_MOVE_TYPE.RANDOM:
                                case TARGET_MOVE_TYPE.ALL:
                                    for (int i = 0; i < opposition.Count; i++)
                                    {
                                        tc = opposition[i];
                                        allTargetSelectors[i].Selector.SetActive(true);
                                        allTargetSelectors[i].Selector.transform.position = screenTOVIEW(tc.transform.position);
                                        HP = ((float)tc.hitPoints / (float)tc.maxHitPoints) * 100;
                                        SP = ((float)tc.skillPoints / (float)tc.maxSkillPoints) * 100;
                                        HP = Mathf.Round(HP);
                                        SP = Mathf.Round(SP);

                                        allTargetSelectors[i].enemyHP.value = HP;
                                        allTargetSelectors[i].enemySP.value = SP;
                                    }
                                    break;

                            }
                        }
                        if (Input.GetKeyDown(s_globals.GetKeyPref("select")))
                        {
                            bool ableToUse = false;
                            switch (currentMove.type)
                            {
                                default:
                                    ableToUse = true;
                                    break;

                                case s_battleAction.MOVE_TYPE.ITEM:
                                    rpg_globals.gl.inventory[currentMove.item.name]--;
                                    switch (currentMove.item.action.moveType)
                                    {
                                        case MOVE_TYPE.STATUS:

                                            switch (currentMove.item.action.statusMoveType)
                                            {
                                                case STATUS_MOVE_TYPE.CURE_STATUS:
                                                    if (currentMove.target.currentStatus == currentMove.item.action.statusEffectChances.status_effect)
                                                    {
                                                        ableToUse = true;
                                                    }
                                                    break;

                                                case STATUS_MOVE_TYPE.HEAL:
                                                    if (currentMove.target.hitPoints < currentMove.target.maxHitPoints)
                                                    {
                                                        ableToUse = true;
                                                    }
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                            }
                            if (ableToUse)
                            {
                                DeactivateOptions();
                                menu_state = MENUSTATE.MENU;
                                enemyHealth.SetActive(false);
                                CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_START;
                                Menuchoice = 0;
                            }
                        }
                        if (Input.GetKeyDown(s_globals.GetKeyPref("back")))
                        {
                            Menuchoice = currentCharacter.CurrentMoveNum;

                            switch (currentMove.type)
                            {
                                case s_battleAction.MOVE_TYPE.ITEM:

                                    ActivateMenuBox();
                                    menu_state = MENUSTATE.ITEM;
                                    break;

                                case s_battleAction.MOVE_TYPE.MOVE:

                                    switch (currentMove.move.moveType)
                                    {
                                        case MOVE_TYPE.TALK:
                                            StartCoroutine(SetMenuBox(currentCharacter.skillMoves, true));
                                            menu_state = MENUSTATE.SKILLS;
                                            break;

                                        case MOVE_TYPE.PHYSICAL:
                                            if (currentMove.move.name == normalAttack.name)
                                            {
                                                menu_state = MENUSTATE.MENU;
                                            }
                                            else
                                            {
                                                menu_state = MENUSTATE.SKILLS;
                                            }
                                            break;

                                        case MOVE_TYPE.STATUS:
                                        case MOVE_TYPE.SPECIAL:
                                            StartCoroutine(SetMenuBox(currentCharacter.skillMoves, true));
                                            menu_state = MENUSTATE.SKILLS;
                                            break;
                                    }
                                    break;
                            }

                            DeactivateOptions();
                            enemyHealth.SetActive(false);
                            CurrentBattleEngineState = BATTLE_ENGINE_STATE.DECISION;
                        }
                        break;

                    case BATTLE_ENGINE_STATE.MOVE_START:

                        switch (currentMove.type)
                        {
                            case s_battleAction.MOVE_TYPE.MOVE:

                                switch (currentMove.move.moveType)
                                {
                                    case MOVE_TYPE.PHYSICAL:
                                        currentMove.user.hitPoints -= currentMove.move.cost;
                                        break;

                                    case MOVE_TYPE.STATUS:
                                    case MOVE_TYPE.TALK:
                                    case MOVE_TYPE.SPECIAL:
                                        currentMove.user.skillPoints -= currentMove.move.cost;
                                        break;
                                }
                                break;

                            case s_battleAction.MOVE_TYPE.GUARD:

                                break;
                        }
                        CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_PROCESS;
                        StartCoroutine(PlayAniamtion(currentMove));
                        break;

                    case BATTLE_ENGINE_STATE.NEGOTIATE_MENU:

                        int act = 0;
                        sparable = new List<o_battleChar>();
                        sparable = opposition.FindAll(x => x.skillPoints <= 0);
                        bool relationship = false;

                        /*
                        //If they have relationships with any character that does not have depleted SP, give a cold look
                        foreach (o_battleChar bc in sparable) {
                            if (bc.relationships == null)
                                continue;
                            foreach (o_battleChar bcRel in bc.relationships) {
                                if (bcRel.skillPoints > 0) {
                                    relationship = true;
                                    NoNegotationRelationship(bcRel);
                                }
                            }
                        }
                        */

                        if (!relationship)
                        {
                            StartCoroutine(SpareEnemy(sparable.ToArray(), act));
                            battleOptions = null;
                            //ClearButtons();
                            //s_soundmanager.sound.PlaySound(ref selectOption, false);
                        }
                        break;

                    case BATTLE_ENGINE_STATE.END:
                        if (opposition.FindAll(x => x.hitPoints <= 0).Count == opposition.Count)
                        {
                            CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
                            oppositionCharacterTurnQueue.Clear();
                            playerCharacterTurnQueue.Clear();
                            StartCoroutine(DefeatedEnemies());
                        }
                        else if (players.FindAll(x => x.hitPoints <= 0).Count == players.Count)
                        {
                            CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
                            currentMove = null;
                            playerCharacterTurnQueue.Clear();
                            StartCoroutine(GameOverThingy());
                        }
                        else if (netTurn <= 0)
                        {
                            if (isPlayerTurn)
                            {
                                print("End enemy round");
                                playerCharacterTurnQueue.Clear();
                                CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
                                StartCoroutine(TurnText(false));
                            }
                            else
                            {
                                //Reset the turn order from Leader to last member

                                print("End enemy round");
                                oppositionCharacterTurnQueue.Clear();
                                CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
                                StartCoroutine(TurnText(true));
                            }
                        }
                        else
                        {
                            if (isPlayerTurn)
                            {
                                print("End player turn");
                                playerCharacterTurnQueue.Dequeue();
                                playerCharacterTurnQueue.Enqueue(currentCharacter);
                            }
                            else
                            {
                                print("End enemy turn");
                                oppositionCharacterTurnQueue.Dequeue();
                                oppositionCharacterTurnQueue.Enqueue(currentCharacter);
                            }
                            CurrentBattleEngineState = BATTLE_ENGINE_STATE.IDLE;
                        }
                        break;
                }
            }
            else {
                if (!s_rpgEvent.rpgEv.doingEvents)
                    isCutscene = false;
            }
        }
    }

    public IEnumerator TurnText(bool pl)
    {
        int pressTurnCount = 0;
        if (pl)
        {
            TurnTextGui.Play("enemyTurnEnd");
            yield return new WaitForSeconds(0.35f);

            isPlayerTurn = true;
            foreach (o_battleChar e in players)
            {
                playerCharacterTurnQueue.Enqueue(e);
                if (e.hitPoints <= 0)
                {
                    continue;
                }
                StartCoroutine(TurnIconFX(TURN_ICON_FX.APPEAR, pressTurnCount));
                pressTurnCount++;
            }
            yield return new WaitForSeconds(0.85f);
            pressTurn = pressTurnCount;
            CurrentBattleEngineState = BATTLE_ENGINE_STATE.IDLE;
        }
        else {
            TurnTextGui.enabled = true;
            TurnTextGui.Play("playerTurnEnd");
            yield return new WaitForSeconds(0.35f);
            isPlayerTurn = false;
            foreach (o_battleChar e in opposition)
            {
                oppositionCharacterTurnQueue.Enqueue(e);
                if (e.hitPoints <= 0)
                {
                    continue;
                }
                StartCoroutine(TurnIconFX(TURN_ICON_FX.APPEAR, pressTurnCount));
                pressTurnCount++;
            }
            yield return new WaitForSeconds(0.85f);
            pressTurn = pressTurnCount;
            roundNum++;
            menu_state = MENUSTATE.NONE;
            CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
            StartCoroutine(PollBattleEvent());
        }
    }

    void DisplayPlayerStats()
    {
        for (int i = 0; i < guis.Count; i++)
        {
            if (players.Count - 1 < i)
            {
                guis[i].gameObject.SetActive(false);
            }
            else
            {
                guis[i].gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < players.Count; i++)
        {
            o_battleChar b = players[i];

            switch (b.currentStatus) {
                case STATUS_EFFECT.POISON:

                    guis[i].img.color = Color.magenta;
                    break;

                default:
                    guis[i].img.color = Color.white;
                    break;

            }

            guis[i].txt.text = b.name;
            guis[i].HPTxt.text = "HP: " + b.hitPoints;
            guis[i].SPTxt.text = "SP: " + b.skillPoints;

            if (b.hitPoints > b.hitPointMeter)
                b.hitPointMeter += Time.deltaTime * b.meterSpeed;
            else if (b.hitPoints < b.hitPointMeter)
                b.hitPointMeter -= Time.deltaTime * b.meterSpeed;

            float HP = ((float)b.hitPoints / (float)b.maxHitPoints) * 100;
            float SP = ((float)b.skillPoints / (float)b.maxSkillPoints) * 100;
            HP = Mathf.Round(HP);
            SP = Mathf.Round(SP);

            guis[i].hpBar.value = HP;
            guis[i].spBar.value = SP;

            if (b.hitPoints <= 0)
            {
                if (b == currentCharacter)
                {
                    b.isGuarding = true;
                    Menuchoice = 0;
                    menu_state = MENUSTATE.MENU;
                    currentCharacter = null;
                }
                guis[i].img.color = Color.red;
            }
            else
                guis[i].img.color = Color.white;

            float xpos = guis[i].transform.position.x;
            //guis[i].transform.position = new Vector3(xpos, 108);

            /*
            if (players.Contains(b))
            {
                if (b == currentCharacter)
                    guis[i].transform.position = new Vector3(xpos, 148);
            }
            */
        }

    }

    public void ChangeButton(Image img, bool buttonON)
    {
        if (buttonON)
            img.color = Color.white;
        else
            img.color = Color.clear;
    }
    public void ChangeButton(Image img, Color colour)
    {
        img.color = colour;
    }
    public void ChangeAllButtons(Color colour)
    {
        fightButton.color = colour;
        guardButton.color = colour;
        skillsButton.color = colour;
        itemsButton.color = colour;
        spareButton.color = colour;
        runButton.color = colour;
    }

    public void SetMenuBox(Dictionary<string,int> listOfThings)
    {
        int i = 0;
        foreach (KeyValuePair<string, int> it in listOfThings) {

            string nameOfThing = it.Key;

            if (Menuchoice == i) {
                currentItem = nameOfThing;
            }
            menuButtons[i].txt.text = nameOfThing + " - " + it.Value;

            if (Menuchoice == i)
                menuButtons[i].selector.color = new Color(1, 1, 1, 0.5f);
            else
                menuButtons[i].selector.color = Color.clear;

            i++;
        }
    }

    public IEnumerator SetMenuBox(Dictionary<string, int> listOfThings, bool show)
    {
        Color startCol = Color.clear;
        Color endCol = Color.white;
        if (!show)
        {
            startCol = Color.white;
            endCol = Color.clear;
        }
        Color col = startCol;
        float t = 0;
        int i = 0;
        foreach (KeyValuePair<string, int> it in listOfThings)
        {
            string nameOfThing = it.Key;
            if (menuButtons.Length < i)
                continue;
            menuButtons[i].img.color = col;
            menuButtons[i].img.sprite = strikeImage;
            menuButtons[i].txt.text = nameOfThing + " - " + it.Value;

            if (Menuchoice == i)
                menuButtons[i].selector.color = new Color(1, 1, 1, 0.5f);
            else
                menuButtons[i].selector.color = Color.clear;

            if (Menuchoice == i)
                menuButtons[i].selector.color = new Color(1, 1, 1, 0.5f);
            else
                menuButtons[i].selector.color = Color.clear;
            i++;
        }
        menuBox.color = col;
        listOfbuttons.SetActive(true);
        while (col != endCol)
        {
            col = Color.Lerp(startCol, endCol, t);
            menuBox.color = col;
            t += Time.deltaTime * 7.5f;
            for (int i2 = 0; i2 < menuButtons.Length; i2++)
            {
                menuButtons[i2].img.color = col;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (!show)
        {
            listOfbuttons.SetActive(false);
            foreach (menu_button btn in menuButtons)
            {
                btn.img.sprite = null;
                btn.txt.text = "";
            }
        }
    }
    public IEnumerator SetMenuBox(List<s_move> listOfThings, bool show)
    {
        Color startCol = Color.clear;
        Color endCol = Color.white;
        if (!show)
        {
            startCol = Color.white;
            endCol = Color.clear;
        }
        Color col = startCol;
        float t = 0;
        for (int i = 0; i < listOfThings.Count; i++)
        {
            s_move it = listOfThings[i];
            string nameOfThing = it.name;
            if (menuButtons.Length < i)
                continue;
            menuButtons[i].img.color = col;
            switch (it.element)
            {
                case ELEMENT.NORMAL:
                    menuButtons[i].img.sprite = strikeImage;
                    break;

                case ELEMENT.FORCE:
                    menuButtons[i].img.sprite = forceImage;
                    break;

                case ELEMENT.PEIRCE:
                    menuButtons[i].img.sprite = peirceImage;
                    break;

                case ELEMENT.FIRE:
                    menuButtons[i].img.sprite = fireImage;
                    break;

                case ELEMENT.ICE:
                    menuButtons[i].img.sprite = iceImage;
                    break;

                case ELEMENT.ELECTRIC:
                    menuButtons[i].img.sprite = electricImage;
                    break;

                case ELEMENT.EARTH:
                    menuButtons[i].img.sprite = earthImage;
                    break;

                case ELEMENT.WIND:
                    menuButtons[i].img.sprite = windImage;
                    break;

                case ELEMENT.LIGHT:
                    menuButtons[i].img.sprite = lightImage;
                    break;

                case ELEMENT.DARK:
                    menuButtons[i].img.sprite = darkImage;
                    break;

                case ELEMENT.PSYCHIC:
                    menuButtons[i].img.sprite = psychicImage;
                    break;
            }
            string l = it.name;
            if (it.cost > 0)
            {
                if (it.moveType == MOVE_TYPE.PHYSICAL)
                {
                    l += " - " + it.cost + "HP  ";
                }
                else if (it.moveType == MOVE_TYPE.TALK)
                {
                    l += " - " + it.cost + "SP  ";
                }
                else if (it.moveType == MOVE_TYPE.SPECIAL)
                {
                    l += " - " + it.cost + "SP  ";
                }
                else if (it.moveType == MOVE_TYPE.STATUS)
                {
                    l += " - " + it.cost + "SP  ";
                }
            }

            if (!CheckForCostRequirements(it, currentCharacter))
            {
                menuButtons[i].txt.text = "<color=red> " + l + " </color>";
            }
            else
            {
                menuButtons[i].txt.text = l;
            }
            if (Menuchoice == i)
                menuButtons[i].selector.color = new Color(1, 1, 1, 0.5f);
            else
                menuButtons[i].selector.color = Color.clear;

        }
        menuBox.color = col;
        listOfbuttons.SetActive(true);
        while (col != endCol)
        {
            col = Color.Lerp(startCol, endCol, t);
            menuBox.color = col;
            t += Time.deltaTime * 7.5f;
            for (int i = 0; i < menuButtons.Length; i++)
            {
                menuButtons[i].img.color = col;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (!show)
        {
            listOfbuttons.SetActive(false);
            foreach (menu_button btn in menuButtons)
            {
                btn.img.sprite = null;
                btn.txt.text = "";
            }
        }
    }
    public void SetCursorMenu(List<s_move> listOfThings)
    {
        for (int i = 0; i < listOfThings.Count; i++)
        {
            if (Menuchoice == i)
                menuButtons[i].selector.color = new Color(1, 1, 1, 0.5f);
            else
                menuButtons[i].selector.color = Color.clear;
        }
    }

    public IEnumerator ShowButtons(List<string> buttons, bool show)
    {
        Color startCol = Color.clear;
        Color endCol = Color.white;
        if (!show)
        {
            startCol = Color.white;
            endCol = Color.clear;
        }
        Color col = startCol;
        float t = 0;
        while (col != endCol)
        {
            col = Color.Lerp(startCol, endCol, t);
            t += Time.deltaTime * 7.5f;
            menuSelector.color = col;
            foreach (string b in buttons)
            {
                switch (b)
                {
                    case "attack":
                        fightButton.gameObject.SetActive(true);
                        fightButton.color = col;
                        break;//f

                    case "skills":
                        skillsButton.gameObject.SetActive(true);
                        skillsButton.color = col;
                        break;//f

                    case "run":
                        runButton.color = col;
                        runButton.gameObject.SetActive(true);
                        break;

                    case "spare":
                        spareButton.color = col;
                        spareButton.gameObject.SetActive(true);
                        break;

                    case "pass":
                        passButton.color = col;
                        passButton.gameObject.SetActive(true);
                        break;
                    case "items":
                        itemsButton.color = col;
                        itemsButton.gameObject.SetActive(true);
                        break;

                    case "analyze":
                        analyzeButton.color = col;
                        analyzeButton.gameObject.SetActive(true);
                        break;

                    case "guard":
                        guardButton.color = col;
                        guardButton.gameObject.SetActive(true);
                        break;
                }
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (!show)
            ClearButtons();
    }

    public void ActivateMenuBox()
    {
        menuBox.color = Color.white;
        listOfbuttons.SetActive(true);
    }

    public void DeactivateOptions()
    {
        for (int i = 0; i < allTargetSelectors.Length; i++)
        {
            allTargetSelectors[i].Selector.SetActive(false);
        }
    }
    
    enum TURN_ICON_FX
    {
        APPEAR,
        FADE,
        HIT
    }
    IEnumerator TurnIconFX(TURN_ICON_FX fx, int i)
    {
        if (i > PT_GUI.Length - 1)
            yield return null;

        Color turnColour = Color.white;
        if(isPlayerTurn)
            turnColour = Color.red;
        else
            turnColour = Color.yellow;
        PT_GUI[i].color = turnColour;

        switch (fx)
        {
            case TURN_ICON_FX.APPEAR:
                PT_GUI[i].GetComponent<Animator>().Play("PTiconAppear");
                yield return new WaitForSeconds(0.7f);
                break;

            case TURN_ICON_FX.HIT:
                PT_GUI[i].GetComponent<Animator>().Play("PTIconBlink");
                yield return new WaitForSeconds(0.1f);
                break;

            case TURN_ICON_FX.FADE:
                PT_GUI[i].GetComponent<Animator>().Play("PTiconDissapear");
                yield return new WaitForSeconds(0.7f);
                break;
        }
    }
    
    public IEnumerator NoNegotationRelationship(o_battleChar bc)
    {
        actionDisp.text = bc.name + " gave you a cold look." + "\n";
        yield return new WaitForSeconds(0.6f);
        Menuchoice = 0;
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.DECISION;
    }

    public IEnumerator ResultsShow(o_battleChar[] targ, float expMult) {

        for (int i = 0; i < players.Count; i++)
        {
            EXPList[i].value = players[i].exp;
            EXPText[i].text = players[i].name + " Level: " + players[i].level;
        }
        EXPResults.SetActive(true);
        for (int i = 0; i < players.Count; i++)
        {
            o_battleChar bc = players[i];
            int exp = 0;
            foreach (o_battleChar chTarg in targ)
            {
                exp += bc.CalculateExp(chTarg, expMult);
            }
            actionDisp.text += bc.name + " gained " + exp + " experience points." + "\n";
            yield return new WaitForSeconds(0.05f);
            for (int i2 = 0; i2 < exp; i2++)
            {
                bc.exp += 1;
                if (bc.exp >= 100)
                {
                    BattleCharacterData ch = bc.data;
                    bc.level++;
                    if (ch.level % ch.attackG == 0)
                    {
                        bc.attack++;
                    }
                    if (ch.level % ch.defenceG == 0)
                    {
                        bc.defence++;
                    }
                    if (ch.level % ch.intelligenceG == 0)
                    {
                        bc.intelligence++;
                    }
                    if (ch.level % ch.speedG == 0)
                    {
                        bc.speed++;
                    }

                    foreach (o_battleChar.move_learn mov in ch.moveDatabase)
                    {
                        if (mov.level <= bc.level)
                        {
                            if (bc.skillMoves.Find(x => x.name == mov.move.name) == null) {
                                bc.skillMoves.Add(mov.move);
                            }
                        }
                    }

                    bc.exp = 0;
                    exp = 0;
                    foreach (o_battleChar chTarg in targ)
                    {
                        exp += bc.CalculateExp(chTarg, expMult);
                    }
                }
                EXPText[i].text = players[i].name + " Level: " + players[i].level;
                EXPList[i].value = bc.exp;
                yield return new WaitForSeconds(Time.deltaTime * 3.85f);
            }
        }
    }
    
    public IEnumerator SpareEnemy(o_battleChar[] targ, int mode) {
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_PROCESS;
        drawExp = true;

        yield return StartCoroutine(ResultsShow(targ, 0.75f));

        float multiplier = 0.15f * targ.Length;
        
        foreach (o_battleChar ch in players) {
            ch.skillPoints += (int)((float)ch.maxSkillPoints * multiplier);
        }

        float moneyTot = 0;

        Dictionary<string, int> items = new Dictionary<string, int>();

        //TODO GIVE PLAYER DROPS
        foreach (o_battleChar chTarg in targ)
        {
            if (chTarg.spareDrops != null) {

                foreach (rpg_item it in chTarg.spareDrops)
                {
                    if (!items.ContainsKey(it.name))
                    {
                        items.Add(it.name, 1);
                    }
                    else {
                        items[it.name]++;
                    }
                    rpg_globals.gl.AddItem(it, 1);
                }
            }
            moneyTot += chTarg.moneySpare;
        }
        foreach (KeyValuePair<string, int> it in items) {

            earningsBattle.text += "\n" + it.Key + " x " + it.Value;
        }
        actionDisp.text += "£" + moneyTot + " gained." + "\n";
        yield return new WaitForSeconds(1.4f);

        foreach (o_battleChar chTarg in targ)
        {
            opposition.Remove(chTarg);
        }
        ClearButtons();
        Menuchoice = 0;
        //yield return StartCoroutine(ResultsShow(targ, 0.35f));
        yield return StartCoroutine(ConcludeBattle());

    }

    public void EndPlayerTurn()
    {
        ClearButtons();
        Menuchoice = 0;
        menu_state = MENUSTATE.MENU;
        playerCharacterTurnQueue.Dequeue();
        playerCharacterTurnQueue.Enqueue(currentCharacter);
        //EndAction();
    }

    public void SpawnDamageObject(Vector2 position, int damage)
    {
        /*
        s_object du = hitObj;
        du.GetComponent<Animator>().Play("damageAnim");
        du.transform.GetChild(1).GetComponent<Text>().text = "";
        du.transform.GetChild(1).GetComponent<Text>().text = "" + damage;
        du.transform.position = position;
        */
    }

    private void OnGUI()
    {
       // DrawPressTurn();
        for (int i = 0; i < players.Count; i++)
        {
            o_battleChar b = players[i];
            if (drawExp)
            {
                /*
                float seperator = 170;
                GUI.DrawTexture(new Rect(20 + (seperator * i), 20, 160, 215), guiBar);
                float barSize = 100;
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(20 + (seperator * i), 20, barSize, 40), expBarTexture);
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(20 + (seperator * i), 20, barSize * (float)((float)b.exp / (float)b.expToNextLevel), 40), expBarTexture);
                
                GUI.Label(new Rect(20 + (seperator * i), 70, 160, 40), "Str: " + b.attack);
                GUI.Label(new Rect(20 + (seperator * i), 110, 160, 40), "Vit: " + b.defence);
                GUI.Label(new Rect(20 + (seperator * i), 150, 160, 40), "Spd: " + b.speed);
                GUI.Label(new Rect(20 + (seperator * i), 190, 160, 40), "Gut: " + b.guts);
                */
            }
        }
    }
}

/*
public void UpdatePlayerMeterPos()
{
    //int num = 512;
    switch (players.Count)
    {
        case 1:

            guis[0].transform.localPosition = new Vector2(0, 108);
            break;

        case 2:

            guis[0].transform.localPosition = new Vector2(-85, 108);
            guis[1].transform.localPosition = new Vector2(145, 108);
            break;

        case 3:

            guis[0].transform.localPosition = new Vector2(-193, 108);
            guis[1].transform.localPosition = new Vector2(37, 108);
            guis[2].transform.localPosition = new Vector2(267, 108);
            break;

        case 4:

            guis[0].transform.localPosition = new Vector3(-342, 108);
            guis[1].transform.localPosition = new Vector3(-112, 108);
            guis[2].transform.localPosition = new Vector3(118, 108);
            guis[3].transform.localPosition = new Vector3(348, 108);
            break;
    }

}
*/
/*
public s_anim FindAnim(string aName)
{
    s_animhandler an = animationObject.GetComponent<s_animhandler>();
    foreach (s_anim a in an.animations)
    {
        if (a.name == aName)
            return a;
    }
    return null;
}
*/
/*
public IEnumerator DamageCalculationEffect()
{
    s_battleAction move = currentMove;
    Vector2 characterPos = new Vector2(0, 0);
    bool isPlayer = players.Contains(move.target);

    float tm = 0.013f;

    if (move.move.moveType != MOVE_TYPE.MISC)
    {
        int damage = move.target.DoDamage(ref move);
        if (!move.move.onTeam)
        {

            if (isPlayer)
                SpawnDamageObject(characterPos, damage);
            else
                SpawnDamageObject(screenTOVIEW(characterPos), damage);

            s_soundmanager.sound.PlaySound("hit2");
            if (move.target.elementTypeCharts[(int)move.move.element] > 1 || move.target.actionTypeCharts[(int)move.move.action_type] > 1)
            {
                s_soundmanager.sound.PlaySound("hitweakpt");
            }
            if (move.target.elementTypeCharts[(int)move.move.element] > 0)
            {
                float r = UnityEngine.Random.Range(0f, 1f);
                print(r);
                if (move.move.statusEffectChances.statusEffectChance != 0)
                {
                    if (r <= move.move.statusEffectChances.statusEffectChance)
                    {
                        move.target.SetStatusEffect(move.move.statusEffectChances.status_effect);
                    }
                }
            }
            Vector2 plGUIPos = new Vector2(1,1);
            if (isPlayer)
            {
                plGUIPos = guis[players.IndexOf(move.target)].transform.position;
                characterPos = move.target.transform.position;
            }
                //characterPos = guis[players.IndexOf(move.target)].transform.position;
            else
                characterPos = move.target.transform.position;
            SpawnDamageObject(damage, characterPos);
            for (int i = 0; i < 2; i++)
            {
                if (isPlayer)
                {
                    guis[players.IndexOf(move.target)].transform.position = plGUIPos + new Vector2(15, 0);
                    move.target.transform.position = characterPos + new Vector2(15, 0);
                    yield return new WaitForSeconds(tm);
                    guis[players.IndexOf(move.target)].transform.position = plGUIPos;
                    move.target.transform.position = characterPos;
                    yield return new WaitForSeconds(tm);
                    guis[players.IndexOf(move.target)].transform.position = plGUIPos + new Vector2(-15, 0);
                    move.target.transform.position = characterPos + new Vector2(-15, 0);
                    yield return new WaitForSeconds(tm);
                    guis[players.IndexOf(move.target)].transform.position = plGUIPos;
                    move.target.transform.position = characterPos;
                    yield return new WaitForSeconds(tm);
                }
                else
                {
                    move.target.transform.position = characterPos + new Vector2(15, 0);
                    yield return new WaitForSeconds(tm);
                    move.target.transform.position = characterPos;
                    yield return new WaitForSeconds(tm);
                    move.target.transform.position = characterPos + new Vector2(-15, 0);
                    yield return new WaitForSeconds(tm);
                    move.target.transform.position = characterPos;
                    yield return new WaitForSeconds(tm);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            //s_soundmanager.sound.PlaySound(ref healSound, false);
        }

        if (!move.move.onTeam)
        {
            if (move.target.elementTypeCharts[(int)move.move.element] > 1 ||
                move.target.actionTypeCharts[(int)move.move.action_type] > 1 ||
                move.target.currentStatus == STATUS_EFFECT.FROZEN)
            {
                turnPressFlag = PRESS_TURN_TYPE.WEAKNESS;
                actionDisp.text += "CRRRRRITICAL!" + "\n";
                actionDisp.text += move.target.name + " took " + damage + " damage" + "\n";
            }
            else
            {
                actionDisp.text += move.target.name + " took " + damage + " damage" + "\n";
            }
        }
        else
        {
            actionDisp.text += move.target.name + " recovered " + damage + " hit points." + "\n";
        }
    }
}
*/
/*
public void SetMenuBox(List<s_move> listOfThings) 
{
    for (int i = 0; i < listOfThings.Count; i++)
    {
        s_move it = listOfThings[i];
        string nameOfThing = it.name;
        if (menuButtons.Length < i)
            continue;

        switch (it.element) {
            case ELEMENT.NORMAL:
                menuButtons[i].img.sprite = strikeImage; 
                break;

            case ELEMENT.FORCE:
                menuButtons[i].img.sprite = forceImage;
                break;

            case ELEMENT.PEIRCE:
                menuButtons[i].img.sprite = peirceImage;
                break;

            case ELEMENT.FIRE:
                menuButtons[i].img.sprite = fireImage;
                break;

            case ELEMENT.ICE:
                menuButtons[i].img.sprite = iceImage;
                break;

            case ELEMENT.ELECTRIC:
                menuButtons[i].img.sprite = electricImage;
                break;

            case ELEMENT.EARTH:
                menuButtons[i].img.sprite = earthImage;
                break;

            case ELEMENT.WIND:
                menuButtons[i].img.sprite = windImage;
                break;

            case ELEMENT.LIGHT:
                menuButtons[i].img.sprite = lightImage;
                break;

            case ELEMENT.DARK:
                menuButtons[i].img.sprite = darkImage;
                break;

            case ELEMENT.PSYCHIC:
                menuButtons[i].img.sprite = psychicImage;
                break;
        }
        string l = it.name;
        if (it.cost > 0) {
            if (it.moveType == MOVE_TYPE.PHYSICAL)
            {
                l += " - " + it.cost + "HP  ";
            }
            else if (it.moveType == MOVE_TYPE.TALK)
            {
                l += " - " + it.cost + "SP  ";
            }
            else if (it.moveType == MOVE_TYPE.SPECIAL)
            {
                l += " - " + it.cost + "SP  ";
            }
            else if (it.moveType == MOVE_TYPE.STATUS)
            {
                l += " - " + it.cost + "SP  ";
            }
        }

        if (!CheckForCostRequirements(it, currentCharacter))
        {
            menuButtons[i].txt.text = "<color=red> " + l + " </color>";
        }
        else {
            menuButtons[i].txt.text = l;
        }
        if (Menuchoice == i)
            menuButtons[i].selector.color = new Color(1,1,1, 0.5f);
        else
            menuButtons[i].selector.color = Color.clear;

    }
}
*/
/*
public void DrawBattleButtons()
{
    for (int i = 0; i < battleOptions.Count; i++) {

        Rect pos = new Rect(30 + (60 * i), 90, 50,50);

        if (Menuchoice == i)
            GUI.color = Color.green;
        else
            GUI.color = Color.white;

        float wid = 0.125f;

        switch (battleOptions[i]) {

            case "attack":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(0,0, wid, 1f));
                break;

            case "skills":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(wid * 1, 0, wid, 1f));
                break;

            case "guard":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(wid * 4, 0, wid, 1f));
                break;

            case "pass":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(wid * 7, 0, wid, 1f));
                break;

            case "spare":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(wid * 2, 0, wid, 1f));
                break;

            case "items":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(wid * 6, 0, wid, 1f));
                break;

            case "run":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(wid * 5, 0, wid, 1f));
                break;

            case "action":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(wid * 4, 0, wid, 1f));
                break;

            case "item":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(wid * 5, 0, wid, 1f));
                break;

            case "money":
                GUI.DrawTextureWithTexCoords(pos, battleButtons, new Rect(wid * 6, 0, wid, 1f));
                break;

        }
    }
}
*/
/*
void DisplayOpponentStats()
{
for (int i = 0; i < opposition.Count; i++)
{
    o_battleChar b = opposition[i];

    if (b.charge > 0.99f)
    {
        active_characters.Add(b);
        EnemySelectAttack(ref b);
    }
   // Pos += GUIsepDist * 2;
}
}

void IncrementCharges()
{
foreach (o_battleChar bc in allCharacters)
{
    if (!middleOfAction)
    {
        if (bc.hitPoints > 0)
        {
            bc.charge = Mathf.Clamp(bc.charge, 0, 1f);
            bc.charge += Time.deltaTime / 200 * bc.speed; //;
        }
        else
            bc.charge = 0;
        bc.hitPoints = Mathf.Clamp(bc.hitPoints, 0, bc.maxHitPoints);
        bc.MeterSpeed();
    }
}
}

*/
/*
public System.Collections.IEnumerator PlayAniamtion(s_move.s_moveAnim[] anim, s_battleAction move, int damage, bool isCritical)
{
    ///spawn object "move animation"
    ///select the animPrefab that has the name of the animation
    ///then depending on the animation enum excecute the animation

    battleFx.color = Color.white;

    bool isPlayer = players.Contains(move.target);

    if (anim != null)
        foreach (s_move.s_moveAnim a in anim)
        {
            switch (a.pos)
            {
                case s_move.s_moveAnim.MOVEPOSTION.ON_TARGET:

                    battleFx.transform.position = screenTOVIEW(move.target.transform.position);
                    //In battle players cannot be seen, so the animation should play on their meters
                    if (isPlayer)
                    {
                        battleFx.transform.position = guis[players.IndexOf(move.target)].transform.position;
                    }
                    break;
            }
            //battleFx.transform.position = move.Item3.transform.position;
            int ind = 0;
            s_anim currentAnim = FindAnim(a.name);
            while (ind != currentAnim.keyframes.Length)
            {
                battleFx.sprite = currentAnim.keyframes[ind].spr;
                yield return new WaitForSeconds(currentAnim.keyframes[ind].duration);
                ind++;
            }
            battleFx.color = Color.clear;
        }

    //Play hurt animation
    Vector2 characterPos;
    if (isPlayer)
        characterPos = guis[players.IndexOf(move.target)].transform.position;
    else
        characterPos = move.target.transform.position;

    if (!move.move.onTeam)
    {
        //if (isPlayer)
            //s_soundmanager.sound.PlaySound(ref playerHurt, false);

        if (isPlayer)
            SpawnDamageObject(characterPos, damage);
        else
            SpawnDamageObject(screenTOVIEW(characterPos), damage);

        for (int i = 0; i < 2; i++)
        {
            if (isPlayer)
            {
                guis[players.IndexOf(move.target)].transform.position = characterPos + new Vector2(15, 0);
                yield return new WaitForSeconds(0.02f);
                guis[players.IndexOf(move.target)].transform.position = characterPos;
                yield return new WaitForSeconds(0.02f);
                guis[players.IndexOf(move.target)].transform.position = characterPos + new Vector2(-15, 0);
                yield return new WaitForSeconds(0.02f);
                guis[players.IndexOf(move.target)].transform.position = characterPos;
                yield return new WaitForSeconds(0.02f);
            }
            else
            {
                move.target.transform.position = characterPos + new Vector2(15, 0);
                yield return new WaitForSeconds(0.02f);
                move.target.transform.position = characterPos;
                yield return new WaitForSeconds(0.02f);
                move.target.transform.position = characterPos + new Vector2(-15, 0);
                yield return new WaitForSeconds(0.02f);
                move.target.transform.position = characterPos;
                yield return new WaitForSeconds(0.02f);
            }
        }
        yield return new WaitForSeconds(0.5f);
    }
    else
    {
        //s_soundmanager.sound.PlaySound(ref healSound, false);
    }

    if (!move.move.onTeam)
    {
        if (move.target.elementTypeCharts[(int)move.move.element] > 1)
        {
            actionDisp.text += "CRRRRRITICAL!" + "\n";
            actionDisp.text += move.target.name + " took " + damage + " damage" + "\n";
        }
        else
        {
            actionDisp.text += move.target.name + " took " + damage + " damage" + "\n";
        }
    }
    else
    {
        actionDisp.text += move.target.name + " recovered " + damage + " hit points." + "\n";
    }

    if (move.target.hitPoints <= 0)
    {
        if (opposition.Contains(move.target))
        {
            opposition.Remove(move.target);
            foreach (o_battleChar bc in players)
            {
                actionDisp.text += bc.name + " gained " + bc.CalculateExp(move.target, players.Count) + " experience points." + "\n";
            }
        }
    }
    else if (move.target.skillPoints <= -move.target.maxSkillPoints)
    {

        if (opposition.Contains(move.target))
        {
            SpareEnemy(ref move.target);
            opposition.Remove(move.target);
        }
    }
    EndAction();
    yield return null;
}
*/
/*
if (opposition.Find(x => x.skillPoints <= 0))
{
    battleButtons[7].SetActive(true);
    if (Input.GetKeyDown(KeyCode.Z))
    {
        ClearButtons();
        menu_state = MENUSTATE.SPARE;
        s_soundmanager.sound.PlaySound(ref selectOption, false);
    }
}
else
    battleButtons[7].SetActive(false);

if (currentCharacter.skillMoves.Count > 0)
    battleButtons[3].SetActive(true);
else
    battleButtons[3].SetActive(false);

if (currentCharacter.actMoves != null)
    battleButtons[4].SetActive(true);
else
    battleButtons[4].SetActive(false);
*/
/*
switch (Menuchoice)
{
    case 0:
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Menuchoice = 0;
        }
        break;
    case 1:
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Menuchoice = 0;
        }
        break;
    case 2:
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Menuchoice = 0;
        }
        break;
}

if (currentCharacter.actMoves.Count > 0)
{
    if (Input.GetKeyDown(KeyCode.A))
    {
        currentCharacter.move_typ = MOVE_TYPE.TALK;
        ClearButtons();
        menu_state = MENUSTATE.ACT;
        s_soundmanager.sound.PlaySound(ref selectOption, false);
    }
}
*/
/*
 
                            foreach (charAI chai in currentCharacter.characterAI)
                            {

                                bool breakOutOfLoop = false;

                                if (target != null)
                                    break;

                                move = chai.moveName;

                                if (!CheckForCostRequirements(move, currentCharacter))
                                    continue;

                                o_battleChar Targ = null;
                                switch (chai.conditions)
                                {
                                    case charAI.CONDITIONS.ALWAYS:
                                        if (chai.onParty)
                                            Targ = partyCandidates[UnityEngine.Random.Range(0, candidates.Count)];
                                        else
                                            Targ = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                                        break;
                                    case charAI.CONDITIONS.USER_PARTY_HP_LOWER:
                                        if (chai.onParty)
                                            Targ = partyCandidates.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                                        else
                                            Targ = candidates.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                                        break;
                                    case charAI.CONDITIONS.USER_PARTY_HP_HIGHER:
                                        if (chai.onParty)
                                            Targ = partyCandidates.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                                        else
                                            Targ = candidates.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                                        break;
                                    case charAI.CONDITIONS.TARGET_PARTY_HP_HIGHER:
                                        if (chai.onParty)
                                            Targ = partyCandidates.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                                        else
                                            Targ = candidates.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                                        break;
                                    case charAI.CONDITIONS.TARGET_PARTY_HP_LOWER:
                                        if (chai.onParty)
                                            Targ = partyCandidates.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                                        else
                                            Targ = candidates.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                                        break;
                                    case charAI.CONDITIONS.TARGET_PARTY_HP_HIGHEST:
                                        if (chai.onParty)
                                        {
                                            Targ = partyCandidates[0];
                                            float res = 0;
                                            foreach (o_battleChar bc in partyCandidates)
                                            {
                                                if (bc.hitPoints > res)
                                                {
                                                    res = bc.hitPoints;
                                                    Targ = bc;
                                                }
                                            }
                                        }
                                        else {
                                            Targ = candidates[0];
                                            float res = 0;
                                            foreach (o_battleChar bc in candidates)
                                            {
                                                if (bc.hitPoints > res)
                                                {
                                                    res = bc.hitPoints;
                                                    Targ = bc;
                                                }
                                            }
                                        }
                                        break;
                                    case charAI.CONDITIONS.TARGET_PARTY_HP_LOWEST:
                                        if (chai.onParty)
                                        {
                                            Targ = partyCandidates[0];
                                            float res = float.MaxValue;
                                            foreach (o_battleChar bc in partyCandidates)
                                            {
                                                if (bc.hitPoints < res)
                                                {
                                                    res = bc.hitPoints;
                                                    Targ = bc;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Targ = candidates[0];
                                            float res = float.MaxValue;
                                            foreach (o_battleChar bc in candidates)
                                            {
                                                if (bc.hitPoints < res) {
                                                    res = bc.hitPoints;
                                                    Targ = bc;
                                                }
                                            }
                                        }
                                        break;
                                    case charAI.CONDITIONS.ON_TURN:
                                        break;
                                }
                                if (Targ != null)
                                {
                                    target = Targ;
                                    breakOutOfLoop = true;
                                }
                                if (breakOutOfLoop)
                                    break;
                            }
     
     
     */
