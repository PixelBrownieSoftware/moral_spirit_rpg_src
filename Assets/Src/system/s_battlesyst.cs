using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using MagnumFoundation2.System.Core;
using MagnumFoundation2.System;

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

    List<string> enemySkillsUsed = new List<string>();

    public HashSet<o_battleChar> active_characters = new HashSet<o_battleChar>();
    public List<RPG_battleMemory> oppositionBattleMemory = new List<RPG_battleMemory>();
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
    bool[] battleEvDone;
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
    public float opponentCP, opponentCPMax;
    public float playerCP, playerCPMax;

    public Slider playerCPSlider, opponentCPSlider;
    public Text playerCPText, opponentCPText;

    public BATTLE_ENGINE_STATE CurrentBattleEngineState = BATTLE_ENGINE_STATE.IDLE;

    public Sprite battleBG;
    public SpriteRenderer bgBattle;

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
    public s_move guardMove;
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
        if (move == guardMove)
            return true;
        if (!battleCharacter.allSkills.Contains(move))
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
                if (players.Contains(battleCharacter))
                {
                    if (move.cost <= playerCP)
                        return true;
                }
                else
                {
                    if (move.cost <= opponentCP)
                        return true;
                }
                break;
        }
        return false;
    }

    public void WinBattle()
    {
    }
    
    public void SetEnemyBattleMemoryAction(string charName, ACTION_TYPE ac)
    {
        RPG_battleMemory bat = oppositionBattleMemory.Find(x => x.name == charName);
        if (bat == null)
        {
            bat = new RPG_battleMemory();
            bat.name = charName;
            oppositionBattleMemory.Add(bat);
            bat.knownTalkAffinites[(int)ac] = true;
        }
        else
        {
            bat.knownTalkAffinites[(int)ac] = true;
        }
    }

    public void SpawnDamageObject(string dmg, Vector2 characterPos, s_dmg.HIT_FX_TYPE flag) {
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
    public IEnumerator ShakeScreenCharacter(float damage, o_battleChar bc) {

        float range = 0;
        float bcHPPerc = damage / bc.maxHitPoints * 100;

        if (bcHPPerc > 45) {
            range = 25f;
        }
        else if (bcHPPerc <= 45 && bcHPPerc > 25)
        {
            range = 15f;
        }
        else if (bcHPPerc <= 25 && bcHPPerc > 15)
        {
            range = 5f;
        }
        for (int i = 0; i < 15; i++)
        {
            MagnumFoundation2.System.s_camera.cam.CameraShake(0, UnityEngine.Random.Range(-range, range));
            yield return new WaitForSeconds(0.025f);
            MagnumFoundation2.System.s_camera.cam.transform.position = new Vector3(0, 0, MagnumFoundation2.System.s_camera.cam.transform.position.z);
        }
        MagnumFoundation2.System.s_camera.cam.transform.position = new Vector3(0,0, MagnumFoundation2.System.s_camera.cam.transform.position.z);
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
        Vector2 characterPos = new Vector2(0, 0);
        bool isPlayer = true;
        Vector2 plGUIPos = Vector2.zero;
        s_move mov = null;

        switch (currentMove.type) {
            case s_battleAction.MOVE_TYPE.MOVE:
                mov = currentMove.move;
                break;

            case s_battleAction.MOVE_TYPE.ITEM:
                mov = currentMove.item.action;
                break;
        }
        if (currentMove.move.moveType != MOVE_TYPE.GUARD)
            isPlayer = players.Contains(currentMove.target);

        float tm = 0.008f;
        int damage = 0;

        if (currentMove.move.moveType != MOVE_TYPE.GUARD)
            damage = Targ.CalculateMove(ref currentMove);

        float buffTimer = 0.15f;
        switch (mov.moveType) {
            case MOVE_TYPE.SPECIAL:
            case MOVE_TYPE.PHYSICAL:
            case MOVE_TYPE.TALK:

                if (!mov.onTeam)
                {
                    if (mov.element != ELEMENT.UNKNOWN)
                    {
                        if (/*
                            Targ.elementTypeCharts[(int)mov.element] > 1.99f ||
                            */Targ.actionTypeCharts[(int)mov.action_type] > 1.99f)
                        {
                            if (Targ.guardPoints > 0)
                            {
                                turnPressFlag = PRESS_TURN_TYPE.NORMAL;
                            }
                            else
                            {
                                Targ.hitByWeaknessCount++;
                                turnPressFlag = PRESS_TURN_TYPE.WEAKNESS;
                            }
                        }
                        else if (
                            /*
                          (Targ.elementTypeCharts[(int)mov.element] < 0 &&
                          Targ.elementTypeCharts[(int)mov.element] >= -1) ||
                          */
                          (Targ.actionTypeCharts[(int)mov.action_type] < 0 &&
                          Targ.actionTypeCharts[(int)mov.action_type] >= -1))
                        {
                            Targ.hitByWeaknessCount = 0;
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
                            Targ.hitByWeaknessCount = 0;
                            turnPressFlag = PRESS_TURN_TYPE.IMMUNE;
                        }
                        if (players.Contains(currentMove.user))
                        {
                            if (mov.moveType == MOVE_TYPE.SPECIAL ||
                               mov.moveType == MOVE_TYPE.PHYSICAL)
                            {
                                rpg_globals.GetInstance().GetComponent<rpg_globals>().SetBattleMemoryElement(Targ.name, mov.element);
                            }
                            else if (mov.moveType == MOVE_TYPE.TALK)
                            {
                                rpg_globals.GetInstance().GetComponent<rpg_globals>().SetBattleMemoryAction(Targ.name, mov.action_type);
                            }
                        }
                        else
                        {
                            if (mov.moveType == MOVE_TYPE.TALK)
                                SetEnemyBattleMemoryAction(Targ.name, mov.action_type);
                        }
                    }
                }

                if (turnPressFlag == PRESS_TURN_TYPE.REFLECT) {
                    damage = Targ.CalculateMove(ref currentMove) * - 1;
                    Targ = currentMove.user;
                }
                switch (turnPressFlag)
                {
                    case PRESS_TURN_TYPE.ABSORB:
                        characterPos = Targ.transform.position;
                        s_soundmanager.sound.PlaySound("heal");
                        Targ.hitPoints += damage * -1;
                        Targ.hitPoints = Mathf.Clamp(Targ.hitPoints, -Targ.maxHitPoints, Targ.maxHitPoints);
                        break;

                    case PRESS_TURN_TYPE.IMMUNE:
                        if (isPlayer)
                        {
                            plGUIPos = guis[players.IndexOf(currentMove.target)].transform.position;
                            characterPos = Targ.transform.position;
                        }
                        else
                            characterPos = Targ.transform.position;
                        //TODO: PUT IMMUNE GRAPHIC
                        SpawnDamageObject(0 + "", characterPos, s_dmg.HIT_FX_TYPE.BLOCK);
                        break;

                    case PRESS_TURN_TYPE.MISS:
                        characterPos = (Vector2)Targ.transform.position + new Vector2(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20));
                        SpawnDamageObject("", characterPos, s_dmg.HIT_FX_TYPE.MISS);
                        s_soundmanager.sound.PlaySound("miss");
                        break;

                    default:
                        /*
                        if (mov.moveType != MOVE_TYPE.TALK)
                        {
                            switch (mov.moveType)
                            {
                                case MOVE_TYPE.PHYSICAL:
                                case MOVE_TYPE.SPECIAL:
                                    Targ.guardPoints--;
                                    break;
                            }
                            Targ.hitPoints -= damage;
                            if (Targ.hitPoints <= 0) {
                                Targ.attackBuff = 0;
                                Targ.defenceBuff = 0;
                                Targ.gutsBuff = 0;
                                Targ.speedBuff = 0;
                            }
                            Targ.hitPoints = Mathf.Clamp(Targ.hitPoints, -Targ.maxHitPoints, Targ.maxHitPoints);
                        }
                        else
                        {
                            Targ.skillPoints -= damage;
                            Targ.skillPoints = Mathf.Clamp(Targ.skillPoints, 0, Targ.maxSkillPoints);
                        }
                        */
                        if(Targ.guardPoints > 0)
                            Targ.guardPoints--;
                        Targ.hitPoints -= damage;
                        if (Targ.hitPoints <= 0)
                        {
                            Targ.attackBuff = 0;
                            Targ.defenceBuff = 0;
                            Targ.gutsBuff = 0;
                            Targ.speedBuff = 0;
                        }
                        Targ.hitPoints = Mathf.Clamp(Targ.hitPoints, -Targ.maxHitPoints, Targ.maxHitPoints);
                        if (mov.element == ELEMENT.UNKNOWN)
                        {
                            float r = UnityEngine.Random.Range(0f, 1f);
                            //print(r);

                            if (mov.statusEffectChances.statusEffectChance != 0)
                            {
                                /*
                                if (r <= (mov.statusEffectChances.statusEffectChance * (float)(currentMove.user.getNetLuck / Targ.getNetLuck)))
                                {
                                    Targ.SetStatusEffect(mov.statusEffectChances.status_effect);
                                }
                                */
                            }
                        }
                        else if (Targ.elementTypeCharts[(int)mov.element] > 0)
                        {
                            float r = UnityEngine.Random.Range(0f, 1f);
                            //print(r);

                            /*
                            if (mov.statusEffectChances.statusEffectChance != 0)
                            {
                                if (r <= (mov.statusEffectChances.statusEffectChance * (float)(currentMove.user.getNetLuck / Targ.getNetLuck)))
                                {
                                    Targ.SetStatusEffect(mov.statusEffectChances.status_effect);
                                }
                            }
                            */
                        }
                        plGUIPos = new Vector2(1, 1);
                        if (isPlayer)
                        {
                            plGUIPos = guis[players.IndexOf(currentMove.target)].transform.position;
                            characterPos = Targ.transform.position ;
                        }
                        else
                            characterPos = Targ.transform.position ; 
                        switch (mov.moveType) {
                            case MOVE_TYPE.TALK:
                                if (turnPressFlag == PRESS_TURN_TYPE.WEAKNESS)
                                    SpawnDamageObject("" + damage, characterPos + new Vector2(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20)), s_dmg.HIT_FX_TYPE.CRIT_HP);
                                else
                                    SpawnDamageObject("" + damage, characterPos + new Vector2(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20)), s_dmg.HIT_FX_TYPE.NONE);
                                break;
                            default:
                                if (turnPressFlag == PRESS_TURN_TYPE.WEAKNESS)
                                    SpawnDamageObject("" + damage, characterPos + new Vector2(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20)), s_dmg.HIT_FX_TYPE.CRIT_HP);
                                else
                                    SpawnDamageObject("" + damage, characterPos + new Vector2(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20)), s_dmg.HIT_FX_TYPE.NONE);
                                break;
                        }
                        if (players.Contains(Targ))
                        {
                            s_soundmanager.GetInstance().PlaySound("hurtsound2");
                            StartCoroutine(ShakeScreenCharacter(damage, Targ));
                        }
                        else {
                            s_soundmanager.sound.PlaySound("hit2");
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
                                yield return new WaitForSeconds(0.3f);
                                characterPos = Targ.transform.position;
                                {
                                    if (mov.buffUser)
                                    {
                                        currentCharacter.attackBuff += mov.str_inc;
                                        currentCharacter.intelligenceBuff += mov.dex_inc;
                                        currentCharacter.defenceBuff += mov.vit_inc;
                                        currentCharacter.speedBuff += mov.agi_inc;
                                        currentCharacter.gutsBuff += mov.gut_inc;
                                    }
                                    if (mov.buffTarget)
                                    {
                                        Targ.attackBuff += mov.str_inc;
                                        Targ.intelligenceBuff += mov.dex_inc;
                                        Targ.defenceBuff += mov.vit_inc;
                                        Targ.speedBuff += mov.agi_inc;
                                        Targ.gutsBuff += mov.gut_inc;
                                    }

                                    string buff_debuff_string = "";
                                    if (mov.str_inc > 0)
                                    {
                                        buff_debuff_string += "+ " + mov.str_inc + " Str" + '\n';
                                    }

                                    if (mov.dex_inc > 0)
                                    {
                                        buff_debuff_string += "+ " + mov.dex_inc + " Dex" + '\n';
                                    }

                                    if (mov.vit_inc > 0)
                                    {
                                        buff_debuff_string += "+ " + mov.vit_inc + " Vit" + '\n';
                                    }

                                    if (mov.agi_inc > 0)
                                    {
                                        buff_debuff_string += "+ " + mov.agi_inc + " Agi" + '\n';
                                    }

                                    if (mov.gut_inc > 0)
                                    {
                                        buff_debuff_string += "+ " + mov.gut_inc + " Gut" + '\n';
                                    }
                                    //s_soundmanager.sound.PlaySound("int_up");
                                    SpawnDamageObject(buff_debuff_string, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
                                    s_soundmanager.sound.PlaySound("attk_up");
                                    yield return new WaitForSeconds(buffTimer);
                                }
                                
                                break;

                            case STATUS_MOVE_TYPE.DEBUFF:
                                yield return new WaitForSeconds(0.3f);
                                characterPos = Targ.transform.position;
                                {
                                    Targ.attackBuff -= mov.str_inc;
                                    Targ.intelligenceBuff -= mov.dex_inc;
                                    Targ.defenceBuff -= mov.vit_inc;
                                    Targ.speedBuff -= mov.agi_inc;
                                    Targ.gutsBuff -= mov.gut_inc;

                                    string buff_debuff_string = "";
                                    if (mov.str_inc > 0)
                                    {
                                        buff_debuff_string += "- " + mov.str_inc + " Str" + '\n';
                                    }

                                    if (mov.dex_inc > 0)
                                    {
                                        buff_debuff_string += "- " + mov.dex_inc + " Dex" + '\n';
                                    }

                                    if (mov.vit_inc > 0)
                                    {
                                        buff_debuff_string += "- " + mov.vit_inc + " Vit" + '\n';
                                    }

                                    if (mov.agi_inc > 0)
                                    {
                                        buff_debuff_string += "- " + mov.agi_inc + " Agi" + '\n';
                                    }

                                    if (mov.gut_inc > 0)
                                    {
                                        buff_debuff_string += "- " + mov.gut_inc + " Gut" + '\n';
                                    }
                                    //s_soundmanager.sound.PlaySound("int_up");
                                    s_soundmanager.sound.PlaySound("int_down");
                                    SpawnDamageObject(buff_debuff_string, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                                    yield return new WaitForSeconds(buffTimer);
                                }    
                                break;
                        }
                        break;
                }

                if (mov.cost == 0)
                {
                    if (players.Contains(currentCharacter))
                    {
                        switch (turnPressFlag)
                        {
                            case PRESS_TURN_TYPE.NORMAL:
                                if (Targ.actionTypeCharts[(int)mov.action_type] >= 1)
                                    playerCP += 3;
                                else
                                    playerCP += 1;
                                break;

                            case PRESS_TURN_TYPE.WEAKNESS:
                                playerCP += 4;
                                break;

                            case PRESS_TURN_TYPE.ABSORB:
                                opponentCP += 2;
                                break;

                            case PRESS_TURN_TYPE.REFLECT:
                                opponentCP += 1;
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (turnPressFlag)
                        {
                            case PRESS_TURN_TYPE.NORMAL:
                                if (Targ.actionTypeCharts[(int)mov.action_type] >= 1)
                                    opponentCP += 3;
                                else
                                    opponentCP += 1;
                                break;

                            case PRESS_TURN_TYPE.WEAKNESS:
                                opponentCP += 4;
                                break;

                            case PRESS_TURN_TYPE.ABSORB:
                                playerCP += 2;
                                break;

                            case PRESS_TURN_TYPE.REFLECT:
                                playerCP += 1;
                                break;

                            default:
                                break;
                        }
                    }
                }
                playerCP = Mathf.Clamp(playerCP, 0, playerCPMax);
                opponentCP = Mathf.Clamp(opponentCP, 0, opponentCPMax);
                yield return new WaitForSeconds(0.05f);
                break;

            case MOVE_TYPE.STATUS:

                switch (mov.statusMoveType) {
                    case STATUS_MOVE_TYPE.HEAL:
                        characterPos = Targ.transform.position;
                        s_soundmanager.sound.PlaySound("healsound2");
                        if (Targ.hitPoints < 0)
                        {
                            Targ.rend.color = Color.white;
                        }
                        if(damage + Targ.hitPoints >= Targ.maxHitPoints)
                            SpawnDamageObject("Max", characterPos, s_dmg.HIT_FX_TYPE.HEAL);
                        else
                            SpawnDamageObject(""+ damage, characterPos, s_dmg.HIT_FX_TYPE.HEAL);
                        Targ.hitPoints += damage;
                        Targ.hitPoints = Mathf.Clamp(Targ.hitPoints, -Targ.maxHitPoints, Targ.maxHitPoints);

                        if (Targ.hitPoints > 0)
                        {
                            float t = 0;
                            float spd = 13.6f;
                            while (Targ.rend.color != Color.white)
                            {
                                Targ.rend.color = Color.Lerp(Color.black, Color.white, t);
                                t += Time.deltaTime * spd;
                                yield return new WaitForSeconds(Time.deltaTime);
                            }
                        }
                        break;

                    case STATUS_MOVE_TYPE.HEAL_STAMINA:
                        characterPos = Targ.transform.position;
                        s_soundmanager.sound.PlaySound("spHealSound");
                        if (players.Contains(Targ))
                        {
                            playerCP += damage;
                            playerCP = Mathf.Clamp(playerCP, 0, playerCPMax);
                        }
                        else {
                            opponentCP += damage;
                            opponentCP = Mathf.Clamp(opponentCP, 0, opponentCPMax);
                        }
                        break;

                    case STATUS_MOVE_TYPE.CURE_STATUS:
                        //Targ.currentStatus = mov.statusEffectChances.;
                        break;
                    case STATUS_MOVE_TYPE.BUFF:
                        if (mov.buffUser)
                        {
                            StartCoroutine(PlayBuffAnim(currentCharacter, mov));
                        }
                        yield return new WaitForSeconds(0.15f);
                        if (mov.buffTarget)
                        {
                            StartCoroutine(PlayBuffAnim(Targ, mov));
                        }
                        yield return new WaitForSeconds(0.25f);
                        break;

                    case STATUS_MOVE_TYPE.DEBUFF:
                        characterPos = Targ.transform.position;

                        {
                            Targ.attackBuff -= mov.str_inc;
                            Targ.intelligenceBuff -= mov.dex_inc;
                            Targ.defenceBuff -= mov.vit_inc;
                            Targ.speedBuff -= mov.agi_inc;
                            Targ.gutsBuff -= mov.gut_inc;

                            string buff_debuff_string = "";
                            if (mov.str_inc > 0)
                            {
                                buff_debuff_string += "- " + mov.str_inc + " Str" + '\n';
                            }

                            if (mov.dex_inc > 0)
                            {
                                buff_debuff_string += "- " + mov.dex_inc + " Dex" + '\n';
                            }

                            if (mov.vit_inc > 0)
                            {
                                buff_debuff_string += "- " + mov.vit_inc + " Vit" + '\n';
                            }

                            if (mov.agi_inc > 0)
                            {
                                buff_debuff_string += "- " + mov.agi_inc + " Agi" + '\n';
                            }

                            if (mov.gut_inc > 0)
                            {
                                buff_debuff_string += "- " + mov.gut_inc + " Gut" + '\n';
                            }
                            //s_soundmanager.sound.PlaySound("int_up");
                            s_soundmanager.sound.PlaySound("int_down");
                            SpawnDamageObject(buff_debuff_string, characterPos, s_dmg.HIT_FX_TYPE.STAT_DEC);
                            yield return new WaitForSeconds(buffTimer);
                        }
                        break;

                    case STATUS_MOVE_TYPE.CUSTOM:
                        characterPos = Targ.transform.position;
                        switch (mov.customFx)
                        {
                            case "exTrun":
                                for (int i = 0; i < 4; i++) {
                                    yield return StartCoroutine(TurnIconFX( TURN_ICON_FX.HIT, i + netTurn));
                                }
                                halfTurn += 4;
                                break;

                            case "divRet":
                                for (int i = 0; i < currentMove.user.actionTypeCharts.Length; i++)
                                {
                                    if (currentMove.user.actionTypeCharts[i] >= 2)
                                    {
                                        currentMove.user.actionTypeCharts[i] = 1;
                                    }
                                    else if (currentMove.user.actionTypeCharts[i] < 2 && currentMove.user.actionTypeCharts[i] >= 1)
                                    {
                                        currentMove.user.actionTypeCharts[i] = 0.5f;
                                    }
                                    else if (currentMove.user.actionTypeCharts[i] < 1 && currentMove.user.actionTypeCharts[i] > 0)
                                    {
                                        currentMove.user.actionTypeCharts[i] = 0;
                                    }
                                    else if (currentMove.user.actionTypeCharts[i] == 0)
                                    {
                                        currentMove.user.actionTypeCharts[i] = -0.5f;
                                    }
                                    else if (currentMove.user.actionTypeCharts[i] < 0 && currentMove.user.actionTypeCharts[i] > -1)
                                    {
                                        currentMove.user.actionTypeCharts[i] = -1.5f;
                                    }
                                    else if (currentMove.user.actionTypeCharts[i] <= -1 )
                                    {
                                        currentMove.user.actionTypeCharts[i] = 2;
                                    }
                                }
                                break;
                        }
                        break;

                }
                break;

            case MOVE_TYPE.GUARD:

                characterPos = currentCharacter.transform.position;
                turnPressFlag = PRESS_TURN_TYPE.NORMAL;
                currentCharacter.guardPoints++;
                damage = 2;
                if (players.Contains(currentCharacter))
                {
                    playerCP += damage;
                    playerCP = Mathf.Clamp(playerCP, 0, playerCPMax);
                }
                else
                {
                    opponentCP += damage;
                    opponentCP = Mathf.Clamp(opponentCP, 0, opponentCPMax);
                }
                //SpawnDamageObject("", characterPos, s_dmg.HIT_FX_TYPE.HEAL);
                break;
        }
        if (mov.moveType != MOVE_TYPE.GUARD)
            if (Targ.hitPoints <= 0)
            {
                yield return new WaitForSeconds(0.15f);
                Targ.attackBuff = 0;
                Targ.defenceBuff = 0;
                Targ.intelligenceBuff = 0;
                Targ.gutsBuff = 0;
                Targ.speedBuff = 0;

                if(opposition.Contains(Targ))
                    s_soundmanager.sound.PlaySound("enemy_defeat");
                else
                    s_soundmanager.sound.PlaySound("player_defeat");
                float t = 0;
                float spd = 13.6f;
                Color cl = new Color(0.5f, 0.5f, 0, 0.2f);
                while (Targ.rend.color != cl)
                {
                    Targ.rend.color = Color.Lerp(Color.white, cl, t);
                    t += Time.deltaTime * spd;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }
    }

    public IEnumerator PlayBuffAnim(o_battleChar bc, s_move mov) {
        float buffTimer = 0.15f;
        Vector2 characterPos = bc.transform.position;

        if (mov.buffUser)
        {
            currentCharacter.attackBuff += mov.str_inc;
            currentCharacter.intelligenceBuff += mov.dex_inc;
            currentCharacter.defenceBuff += mov.vit_inc;
            currentCharacter.speedBuff += mov.agi_inc;
            currentCharacter.gutsBuff += mov.gut_inc;
            string buff_debuff_string = "";
            if (mov.str_inc > 0)
            {
                buff_debuff_string += "+ " + mov.str_inc + " Str" + '\n';
            }

            if (mov.dex_inc > 0)
            {
                buff_debuff_string += "+ " + mov.dex_inc + " Dex" + '\n';
            }

            if (mov.vit_inc > 0)
            {
                buff_debuff_string += "+ " + mov.vit_inc + " Vit" + '\n';
            }

            if (mov.agi_inc > 0)
            {
                buff_debuff_string += "+ " + mov.agi_inc + " Agi" + '\n';
            }

            if (mov.gut_inc > 0)
            {
                buff_debuff_string += "+ " + mov.gut_inc + " Gut" + '\n';
            }
            //s_soundmanager.sound.PlaySound("int_up");
            SpawnDamageObject(buff_debuff_string, currentCharacter.transform.position, s_dmg.HIT_FX_TYPE.STAT_INC);
            s_soundmanager.sound.PlaySound("attk_up");
            yield return new WaitForSeconds(buffTimer);
        }
        if (mov.buffTarget)
        {
            bc.attackBuff += mov.str_inc;
            bc.intelligenceBuff += mov.dex_inc;
            bc.defenceBuff += mov.vit_inc;
            bc.speedBuff += mov.agi_inc;
            bc.gutsBuff += mov.gut_inc;

            string buff_debuff_string = "";
            if (mov.str_inc > 0)
            {
                buff_debuff_string += "+ " + mov.str_inc + " Str" + '\n';
            }

            if (mov.dex_inc > 0)
            {
                buff_debuff_string += "+ " + mov.dex_inc + " Dex" + '\n';
            }

            if (mov.vit_inc > 0)
            {
                buff_debuff_string += "+ " + mov.vit_inc + " Vit" + '\n';
            }

            if (mov.agi_inc > 0)
            {
                buff_debuff_string += "+ " + mov.agi_inc + " Agi" + '\n';
            }

            if (mov.gut_inc > 0)
            {
                buff_debuff_string += "+ " + mov.gut_inc + " Gut" + '\n';
            }
            //s_soundmanager.sound.PlaySound("int_up");
            SpawnDamageObject(buff_debuff_string, characterPos, s_dmg.HIT_FX_TYPE.STAT_INC);
            s_soundmanager.sound.PlaySound("attk_up");
            yield return new WaitForSeconds(buffTimer);
        }
    }

    public IEnumerator PlayActualAnimation(s_move mov, o_battleChar t) {

        s_move.s_moveAnim[] anim = mov.animation;
        if (anim != null)
        {
            if (anim.Length > 0)
            {
                foreach (s_move.s_moveAnim a in anim)
                {
                    switch (a.type)
                    {
                        case s_move.s_moveAnim.ANIM_TYPE.ANIMATION:
                            //battleFx.sprite = a.image;
                            Vector2 Pos = new Vector2(0, 0);
                            {
                                switch (a.pos)
                                {
                                    case s_move.s_moveAnim.MOVEPOSTION.ON_TARGET:
                                        Pos = t.transform.position;
                                        break;

                                    case s_move.s_moveAnim.MOVEPOSTION.ON_USER:
                                        Pos = currentCharacter.transform.position;
                                        break;

                                    case s_move.s_moveAnim.MOVEPOSTION.FIXED:
                                        Pos = a.position;
                                        break;
                                }
                                s_moveanim battleFX = s_objpooler.GetInstance().SpawnObject<s_moveanim>("battleFX", Pos);
                                // battleFx.GetComponent<Animator>().runtimeAnimatorController = a.anim;
                                battleFX.GetComponent<Animator>().Play(a.name);
                                yield return new WaitForSeconds(a.duration);
                            }
                            break;
                        case s_move.s_moveAnim.ANIM_TYPE.IMAGE:
                            {
                                /*
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
                                s_moveanim battleFX = s_objpooler.GetInstance().SpawnObject<s_moveanim>("battleFX", Pos);
                                battleFX.color = Color.white;
                                yield return new WaitForSeconds(a.duration);
                                */
                            }
                            break;

                        case s_move.s_moveAnim.ANIM_TYPE.CALCUATION:
                            
                            StartCoroutine(DamageCalculationEffect(t));
                            //print("Yes");
                            break;
                    }
                    yield return new WaitForSeconds(a.duration);
                }
            }
            else
            {
                StartCoroutine(DamageCalculationEffect(t));
                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            StartCoroutine(DamageCalculationEffect(t));
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator PlayAnim(s_battleAction move, int times, List<o_battleChar> characterList) {
        s_move mov = move.move;
        if (opposition.Contains(move.user)) {

            if (mov.canLearn)
            {
                enemySkillsUsed.Add(mov.name);
            }
        }

        switch (mov.target)
        {
            case TARGET_MOVE_TYPE.SINGLE:
                if (mov.moveType != MOVE_TYPE.GUARD)
                {
                    yield return StartCoroutine(PlayActualAnimation(mov, move.target));
                }
                else
                {
                    yield return StartCoroutine(DamageCalculationEffect(null));
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
                if (characterList.Count == 0)
                {
                    times = 0;
                    break;
                }
                yield return StartCoroutine(PlayActualAnimation(mov, move.target));
                break;

            case TARGET_MOVE_TYPE.ALL:

                if (mov.onTeam)
                {
                    if (players.Contains(currentCharacter))
                    {
                        characterList = players;
                    }
                    else
                    {
                        characterList = opposition;
                    }
                }
                else
                {
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
                    yield return StartCoroutine(PlayActualAnimation(mov, t));
                }
                break;
        }
    }

    public IEnumerator PlayAniamtion(s_battleAction move)
    {
        #region NOTIFICATION
        if (players.Contains(move.user))
        {
            s_soundmanager.sound.PlaySound("notif");
        }
        else
        {
            s_soundmanager.sound.PlaySound("notif_enemy");
        }

        for(int i =0; i < 2; i++) {
            float t = 0;
            float spd = 13.6f;
            while (move.user.rend.color != Color.black)
            {
                move.user.rend.color = Color.Lerp(Color.white, Color.black, t);
                t += Time.deltaTime * spd;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            t = 0;
            while (move.user.rend.color != Color.white)
            {
                move.user.rend.color = Color.Lerp(Color.black, Color.white, t);
                t += Time.deltaTime * spd;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        switch (move.type) {
            case s_battleAction.MOVE_TYPE.MOVE:
                StartCoroutine(DisplayMoveName(move.move.name));
                break;

            case s_battleAction.MOVE_TYPE.ITEM:
                StartCoroutine(DisplayMoveName(move.item.name));
                break;
        }
        #endregion
        yield return new WaitForSeconds(0.65f);
        //Play hurt animation
        Vector2 characterPos = new Vector2(0, 0);

        bool isPlayer = players.Contains(move.target);
        int times = 1;
        s_move.s_moveAnim[] anim = null;
        s_move.s_moveAnim[] preAnim = null;
        s_move mov = null;

        switch (move.type) {
            case s_battleAction.MOVE_TYPE.MOVE:
                anim = move.move.animation;
                preAnim = move.move.preAnim;
                if (move.move.isMultiHit)
                    times = UnityEngine.Random.Range(1, 4);  //For now we'll hard-code a RNG in there but later on it's going to be affected by aglity
                mov = move.move;
                break;

            case s_battleAction.MOVE_TYPE.ITEM:
                anim = move.item.action.animation;
                preAnim = move.item.action.preAnim;
                if (move.item.action.isMultiHit)
                    times = UnityEngine.Random.Range(1, 4);  //For now we'll hard-code a RNG in there but later on it's going to be affected by aglity
                mov = move.item.action;
                break;
        }

        List<o_battleChar> characterList = new List<o_battleChar>();
        if (preAnim != null) {
            if (preAnim.Length > 0)
            {
                yield return StartCoroutine(PlayAnim(move, times, characterList));
            }
        }
        
        for (int i = 0; i < times; i++)
        {
            yield return StartCoroutine(PlayAnim(move, times, characterList));
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

        yield return new WaitForSeconds(0.05f);

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
    
    public void StartBattleRoutine()
    {
        s_globals.allowPause = false;
        if (enemyGroup.battleEvents != null) {
            OnBattleEvents = new s_battleEvents[enemyGroup.battleEvents.Length];
            Array.Copy(enemyGroup.battleEvents, OnBattleEvents, enemyGroup.battleEvents.Length);
            battleEvDone = Enumerable.Repeat(true, enemyGroup.battleEvents.Length).ToArray();
        }
        s_globals.allowPause = false;
        bgBattle.sprite = FindObjectOfType<s_BGSETTER>().BattleBG;
        StartCoroutine(StartBattle());
    }
    public IEnumerator StartBattle()
    {
        playerCPMax = 0;
        opponentCPMax = 0;
        playerCP = 0;
        opponentCP = 0;
        s_globals.allowPause = false;
        s_BGM.GetInstance().PlaySong(enemyGroup.songName);
        s_menuhandler.GetInstance().SwitchMenu("EMPTY");
        foreach (Image img in PT_GUI)
        {
            img.GetComponent<Animator>().Play("PTIconGone");
        }
        oppositionBattleMemory.Clear();
        oppositionBattleMemory = new List<RPG_battleMemory>();
        enemySkillsUsed.Clear();
        enemySkillsUsed = new List<string>();
        {
            opposition.ForEach(x => x.guardPoints = 0);
            opposition.ForEach(x => x.attackBuff = 0);
            opposition.ForEach(x => x.defenceBuff = 0);
            opposition.ForEach(x => x.speedBuff = 0);
            opposition.ForEach(x => x.gutsBuff = 0);
            opposition.ForEach(x => x.intelligenceBuff = 0);
            opposition.ForEach(x => x.hitByWeaknessCount = 0);
        }
        {
            players.ForEach(x => x.guardPoints = 0);
            players.ForEach(x => x.attackBuff = 0);
            players.ForEach(x => x.defenceBuff = 0);
            players.ForEach(x => x.speedBuff = 0);
            players.ForEach(x => x.gutsBuff = 0);
            players.ForEach(x => x.intelligenceBuff = 0);
            players.ForEach(x => x.hitByWeaknessCount = 0);
        }
        foreach (var a in players)
        {
            if (a.data.determineCP)
                playerCPMax += a.data.maxSkillPointsB;
            else
                playerCPMax += 10;
        }
        foreach (var a in opposition)
        {
            if(a.data.determineCP)
                opponentCPMax += a.data.maxSkillPointsB;
            else
                opponentCPMax += 10;
        }

        CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
        yield return new WaitForSeconds(0.3f);
        halfTurn = 0;
        {
            int i = 0;
            foreach (o_battleChar img in players)
            {
                for (int i2 = img.data.turnIcons; i2 > 0; i2--) {
                    yield return StartCoroutine(TurnIconFX(TURN_ICON_FX.APPEAR, i));
                    i++;
                }
            }
            pressTurn = i;
        }
        yield return new WaitForSeconds(0.75f);
        s_globals.allowPause = false;
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
        //actionDisp.text = "Evan: (This isn't the end of it... there must be a way... there must be...)"+ "\n"
        oppositionCharacterTurnQueue.Clear();
        playerCharacterTurnQueue.Clear();
        s_BGM.GetInstance().StartCoroutine(s_BGM.GetInstance().FadeOutMusic());
        yield return StartCoroutine(rpg_globals.gl.Fade(true));

        isPlayerTurn = true;
        players.Clear();
        isActive = false;
        rpg_globals.gl.GameState = rpg_globals.RPG_STATE.OVERWORLD;
        rpg_globals.gl.ClearAllThings();
        Destroy(rpg_globals.gl.player.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

    public IEnumerator DefeatedEnemies()
    {
        yield return StartCoroutine(ResultsShow(opposition.ToArray(), 1));
        yield return StartCoroutine(ConcludeBattle());
    }

    public IEnumerator ConcludeBattle() {
        //Fade
        EXPResults.SetActive(false);
        StartCoroutine(s_BGM.GetInstance().FadeOutMusic());
        oppositionCharacterTurnQueue.Clear();
        playerCharacterTurnQueue.Clear();
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(rpg_globals.gl.Fade(true));

        foreach (o_battleChar c in players) {
            rpg_globals.gl.SetStats(c);
        }

        players.Clear();
        opposition.Clear();
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

    public IEnumerator PollBattleEvent() {
        yield return CheckForStatusEffect();
        bool conditionFufilled = false;
        if (OnBattleEvents != null)
            for ( int i =0; i < OnBattleEvents.Length; i++)
            {
                s_battleEvents be = OnBattleEvents[i];
                if (battleEvDone[i])
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
                                battleEvDone[i] = false;

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
                                        if (roundNum >= be.int0)
                                        {
                                            s_triggerhandler.GetInstance().GetComponent<s_rpgEvent>().JumpToEvent(be.eventScript);//yield return StartCoroutine();
                                            isCutscene = true;
                                            battleEvDone[i] = false;
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
        //battleFx.color = Color.clear;
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
        rpg_globals.gl.SwitchToOverworld(true);
    }
    public void EndBattle(MagnumFoundation2.Objects.ev_script even)
    {
        currentMove = null;
        playerCharacterTurnQueue.Clear();
        oppositionCharacterTurnQueue.Clear();
        rpg_globals.gl.SwitchToOverworld(false);
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
        foreach(o_battleChar bc in players)
        {
            bc.hitPoints -= Mathf.RoundToInt(bc.maxSkillPoints * 0.15f);
            bc.hitPoints = Mathf.Clamp(bc.hitPoints, 1, bc.maxSkillPoints);
        }
        StartCoroutine(DefeatedEnemies());
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
    
    public void HighlightMenuButton(ref Image img) {
        menuSelector.rectTransform.position = img.rectTransform.position;
    }
    
    public o_battleChar GetTargetCharacter(List<o_battleChar> candidates, charAI chai)
    {
        float res = 0;
        int rand = -1;
        o_battleChar Targ = null;
        switch (chai.conditions)
        {
            case charAI.CONDITIONS.ALWAYS:
                {
                    bool isSelfMove = (chai.moveName.moveType == MOVE_TYPE.STATUS
                        && chai.moveName.statusMoveType == STATUS_MOVE_TYPE.BUFF
                        && chai.moveName.onSelf
                        && chai.moveName.onTeam);
                    if (!isSelfMove)
                    {
                        if (chai.moveName.moveType == MOVE_TYPE.STATUS && chai.moveName.excludeUser)
                        {
                            candidates.Remove(currentCharacter);
                        }
                        for (int i = 0; i < candidates.Count; i++)
                        {
                            o_battleChar p = candidates[i];
                            if (p.hitPoints <= 0)
                            {
                                candidates.Remove(p);
                            }
                        }
                        
                        List<o_battleChar> resistantTargets = new List<o_battleChar>();
                        List<o_battleChar> normalTargets = new List<o_battleChar>();
                        List<o_battleChar> bestTargets = new List<o_battleChar>();
                        List<o_battleChar> unknown = new List<o_battleChar>();

                        switch (chai.moveName.moveType) {

                            case MOVE_TYPE.STATUS:
                                if (chai.moveName.statusMoveType == STATUS_MOVE_TYPE.BUFF)
                                {
                                    foreach (var a in candidates)
                                    {
                                        bool strBuffTooHigh = chai.moveName.str_inc > 0 && chai.moveName.str_inc + Targ.attackBuff > 5;
                                        bool dexBuffTooHigh = chai.moveName.dex_inc > 0 && chai.moveName.dex_inc + Targ.intelligenceBuff > 5;
                                        bool vitBuffTooHigh = chai.moveName.vit_inc > 0 && chai.moveName.vit_inc + Targ.defenceBuff > 5;
                                        bool agiBuffTooHigh = chai.moveName.agi_inc > 0 && chai.moveName.agi_inc + Targ.speedBuff > 5;
                                        bool gutBuffTooHigh = chai.moveName.gut_inc > 0 && chai.moveName.gut_inc + Targ.gutsBuff > 5;
                                        bool buffsTooHigh = strBuffTooHigh && dexBuffTooHigh && vitBuffTooHigh && agiBuffTooHigh && gutBuffTooHigh;
                                        if (buffsTooHigh)
                                        {
                                            resistantTargets.Add(a);
                                        }
                                        else
                                        {
                                            normalTargets.Add(a);
                                        }
                                    }
                                    if (normalTargets.Count > 0)
                                    {
                                        rand = UnityEngine.Random.Range(0, normalTargets.Count);
                                        Targ = normalTargets[rand];
                                    }
                                    else
                                    {
                                        rand = UnityEngine.Random.Range(0, resistantTargets.Count);
                                        Targ = resistantTargets[rand];
                                    }
                                }
                                break;

                            case MOVE_TYPE.TALK:
                                foreach (var a in candidates) {
                                    RPG_battleMemory batt = oppositionBattleMemory.Find(x => x.name == a.name);
                                    if (batt != null)
                                    {
                                        if (batt.knownTalkAffinites[(int)chai.moveName.action_type])
                                        {
                                            float affinity = a.actionTypeCharts[(int)chai.moveName.action_type];
                                            
                                            if (affinity < 1 && affinity > 0)
                                                resistantTargets.Add(a);
                                            else if (affinity >= 1 && affinity < 2)
                                                normalTargets.Add(a);
                                            else if (affinity >= 2)
                                                bestTargets.Add(a);
                                        }
                                        else
                                        {
                                            unknown.Add(a);
                                        }
                                    }
                                    //Let's try another target out if the enemy dosen't know the resistance
                                    else
                                    {
                                        unknown.Add(a);
                                    }
                                }

                                if (bestTargets.Count > 0)
                                {
                                    rand = UnityEngine.Random.Range(0, bestTargets.Count);
                                    Targ = bestTargets[rand];
                                }
                                else if (unknown.Count > 0)
                                {
                                    rand = UnityEngine.Random.Range(0, unknown.Count);
                                    Targ = unknown[rand];
                                }
                                else if (normalTargets.Count > 0) {
                                    rand = UnityEngine.Random.Range(0, normalTargets.Count);
                                    Targ = normalTargets[rand];
                                }
                                else if (resistantTargets.Count > 0)
                                {
                                    rand = UnityEngine.Random.Range(0, resistantTargets.Count);
                                    Targ = resistantTargets[rand];
                                }
                                break;

                            default:
                                rand = UnityEngine.Random.Range(0, candidates.Count);
                                Targ = candidates[rand];
                                break;
                        }

                    }
                    else
                    {
                        Targ = currentCharacter;
                    }
                }
                break;
            case charAI.CONDITIONS.USER_PARTY_HP_LOWER:
                Targ = candidates.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                break;
            case charAI.CONDITIONS.USER_PARTY_HP_HIGHER:
                Targ = candidates.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                break;
                /*
            case charAI.CONDITIONS.USER_PARTY_SP_HIGHER:
                Targ = candidates.Find(x => x.skillPoints > chai.healthPercentage * x.maxSkillPoints);
                break;
            case charAI.CONDITIONS.USER_PARTY_SP_LOWER:
                Targ = candidates.Find(x => x.skillPoints < chai.healthPercentage * x.maxSkillPoints);
                break;
                */
            case charAI.CONDITIONS.TARGET_PARTY_HP_HIGHER:
                Targ = candidates.Find(x => x.hitPoints > chai.healthPercentage * x.maxHitPoints);
                break;
            case charAI.CONDITIONS.TARGET_PARTY_HP_LOWER:
                Targ = candidates.Find(x => x.hitPoints < chai.healthPercentage * x.maxHitPoints);
                break;

            case charAI.CONDITIONS.SELF_HP_HIGHER:
                {
                    if (currentCharacter.hitPoints > chai.healthPercentage * currentCharacter.maxHitPoints)
                    {
                        Targ = currentCharacter;
                    }
                    else {
                        Targ = null;
                    }
                }
                break;
            case charAI.CONDITIONS.SELF_HP_LOWER:
                {
                    if (currentCharacter.hitPoints < chai.healthPercentage * currentCharacter.maxHitPoints)
                    {
                        Targ = currentCharacter;
                    }
                    else
                    {
                        Targ = null;
                    }
                }
                break;

            case charAI.CONDITIONS.SELF_SP_HIGHER:
                {
                    if (opponentCP > chai.healthPercentage * opponentCPMax)
                    {
                        Targ = currentCharacter;
                    }
                    else
                    {
                        Targ = null;
                    }
                }
                break;
            case charAI.CONDITIONS.SELF_SP_LOWER:
                {
                    if (opponentCP < chai.healthPercentage * opponentCPMax)
                    {
                        Targ = currentCharacter;
                    }
                    else
                    {
                        Targ = null;
                    }
                }
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
        switch (chai.turnCounters)
        {
            case charAI.TURN_COUNTER.TURN_COUNTER_EQU:
                if (currentCharacter.turnNumber == chai.number1)
                {
                    currentCharacter.turnNumber = 0;
                }
                else
                {
                    Targ = null;
                }
                break;

            case charAI.TURN_COUNTER.ROUND_COUNTER_EQU:
                if (currentCharacter.roundNumber == chai.number2)
                {
                    currentCharacter.roundNumber = 0;
                }
                else
                {
                    Targ = null;
                }
                break;

            case charAI.TURN_COUNTER.ROUND_TURN_COUNTER_EQU:
                if (currentCharacter.roundNumber == chai.number2 &&
                    currentCharacter.turnNumber == chai.number1)
                {
                    currentCharacter.roundNumber = 0;
                    currentCharacter.turnNumber = 0;
                }
                else
                {
                    Targ = null;
                }
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
                case charAI.CONDITIONS.USER_PARTY_SP_HIGHER:
                case charAI.CONDITIONS.USER_PARTY_SP_LOWER:
                case charAI.CONDITIONS.SELF_SP_LOWER:
                case charAI.CONDITIONS.SELF_SP_HIGHER:
                case charAI.CONDITIONS.SELF_HP_LOWER:
                case charAI.CONDITIONS.SELF_HP_HIGHER:
                    return null;
            }
            switch (chai.turnCounters)
            {
                case charAI.TURN_COUNTER.TURN_COUNTER_EQU:
                case charAI.TURN_COUNTER.ROUND_COUNTER_EQU:
                case charAI.TURN_COUNTER.ROUND_TURN_COUNTER_EQU:
                    return null;
            }
        }
        return Targ;
    }

    public void SelectTarget(s_move mov)
    {
        currentCharacter.CurrentMoveNum = Menuchoice;
        currentCharacter.currentMove = mov;
        currentMove = new s_battleAction(currentCharacter, null, mov);
        Menuchoice = 0;
        onTeam = mov.onTeam;
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_START;
        // StartCoroutine(SetMenuBox(currentCharacter.skillMoves, false));
    }

    public void SelectTarget(o_battleChar battleCharButton, s_move mov) {

        currentCharacter.CurrentMoveNum = Menuchoice;
        currentCharacter.currentMove = mov;
        currentMove = new s_battleAction(currentCharacter, battleCharButton, mov);
        Menuchoice = 0;
        onTeam = mov.onTeam;
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_START;
       // StartCoroutine(SetMenuBox(currentCharacter.skillMoves, false));
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
                                if (currentCharacter.hitPoints <= 0 )
                                {
                                    playerCharacterTurnQueue.Dequeue();
                                    playerCharacterTurnQueue.Enqueue(currentCharacter);
                                    currentCharacter = playerCharacterTurnQueue.Peek();
                                    //print(currentCharacter);
                                }
                                else
                                {
                                    s_menuhandler.GetInstance().SwitchMenu("BattleMainMenu");
                                    
                                    CurrentBattleEngineState = BATTLE_ENGINE_STATE.DECISION;
                                    menu_state = MENUSTATE.MENU;

                                }
                            }
                        }
                        else
                        {
                            currentCharacter = oppositionCharacterTurnQueue.Peek();
                            if (currentCharacter.hitPoints <= 0 )
                            {
                                oppositionCharacterTurnQueue.Dequeue();
                                oppositionCharacterTurnQueue.Enqueue(currentCharacter);
                                currentCharacter = oppositionCharacterTurnQueue.Peek();
                                return;
                                //print(currentCharacter);
                            }
                            CurrentBattleEngineState = BATTLE_ENGINE_STATE.DECISION;
                        }
                        break;

                    case BATTLE_ENGINE_STATE.DECISION:
                        if(!isPlayerTurn)
                        {
                            switch (currentCharacter.currentStatus)
                            {
                                default:
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
                                            //print(currentCharacter.name);
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
                                                target = null;
                                                move = guardMove;
                                            }
                                            else
                                            {
                                                charAI moveAI = nonImportant[UnityEngine.Random.Range(0, nonImportant.Count - 1)];

                                                if (nonImportant.Count > 0)
                                                {
                                                    //Conserve CP
                                                    float sheildChance = UnityEngine.Random.Range(0f, 2f);
                                                    float CPPercent = (opponentCP / opponentCPMax) * 1.75f;
                                                    float conserv = UnityEngine.Random.Range(0f, 2.5f) * CPPercent;
                                                    print(conserv + " percent: " + CPPercent);
                                                    bool isGuard = false;
                                                    if (currentCharacter.data.conservativeCP > conserv)
                                                    {
                                                        if (currentCharacter.hitByWeaknessCount > 2) {
                                                            isGuard = true;
                                                            currentCharacter.hitByWeaknessCount = 0;
                                                        } else {
                                                            List<charAI> aiNoCost = nonImportant.FindAll(x => x.moveName.cost == 0);
                                                            moveAI = aiNoCost[UnityEngine.Random.Range(0, aiNoCost.Count - 1)];
                                                        }
                                                    }
                                                    if (currentCharacter.data.sheildChance > sheildChance)
                                                    {
                                                        if (currentCharacter.hitByWeaknessCount > 2)
                                                        {
                                                            isGuard = true;
                                                            currentCharacter.hitByWeaknessCount = 0;
                                                        }
                                                    }

                                                    if (!isGuard)
                                                    {
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
                                                        target = null;
                                                        move = guardMove;
                                                    }
                                                }
                                                else
                                                {
                                                    target = null;
                                                    move = guardMove;
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        move = guardMove;
                                        target = null;
                                    }
                                    currentMove = new s_battleAction(currentCharacter, target, move);

                                    CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_START;
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
                            //float SP = 0;
                            switch (currentMove.move.target)
                            {
                                case TARGET_MOVE_TYPE.SINGLE:

                                    if (currentMove.target != null)
                                    {
                                        tc = currentMove.target;

                                        singleTargSelector.Selector.SetActive(true);
                                        HP = ((float)tc.hitPoints / (float)tc.maxHitPoints) * 100;
                                        //SP = ((float)tc.skillPoints / (float)tc.maxSkillPoints) * 100;
                                        HP = Mathf.Round(HP);

                                        singleTargSelector.enemyHP.value = HP;
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
                                        HP = Mathf.Round(HP);

                                        if (tc.elementTypeCharts[(int)currentMove.move.element] >= 2)
                                            allTargetSelectors[i].weaknessTarg.sprite = weakDmgTargImage;
                                        else if (tc.elementTypeCharts[(int)currentMove.move.element] <= 0)
                                            allTargetSelectors[i].weaknessTarg.sprite = resDmgTargImage;
                                        else if (tc.elementTypeCharts[(int)currentMove.move.element] > 0 && tc.elementTypeCharts[(int)currentMove.move.element] < 2)
                                            allTargetSelectors[i].weaknessTarg.sprite = normalDmgTargImage;

                                        allTargetSelectors[i].enemyHP.value = HP;
                                        //allTargetSelectors[i].enemySP.value = SP;
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
                                        //SP = ((float)tc.skillPoints / (float)tc.maxSkillPoints) * 100;
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
                                        //SP = ((float)tc.skillPoints / (float)tc.maxSkillPoints) * 100;
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
                                        switch (currentMove.move.statusMoveType) {
                                            default:
                                                if (players.Contains(currentMove.user)) {
                                                    playerCP -= currentMove.move.cost;
                                                }
                                                else
                                                {
                                                    opponentCP -= currentMove.move.cost;
                                                }
                                                break;

                                            case STATUS_MOVE_TYPE.HEAL_STAMINA:
                                                currentMove.user.hitPoints -= currentMove.move.cost;
                                                break;
                                        }
                                        break;
                                    case MOVE_TYPE.TALK:
                                    case MOVE_TYPE.SPECIAL:
                                        if (players.Contains(currentMove.user))
                                        {
                                            playerCP -= currentMove.move.cost;
                                        }
                                        else
                                        {
                                            opponentCP -= currentMove.move.cost;
                                        }
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

                        /*
                        int act = 0;
                        sparable = new List<o_battleChar>();
                        sparable = opposition.FindAll(x => x.skillPoints <= 0);
                        bool relationship = false;
                        
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

                        if (!relationship)
                        {
                            StartCoroutine(SpareEnemy(opposition.ToArray(), act));
                            battleOptions = null;
                            //ClearButtons();
                            //s_soundmanager.sound.PlaySound(ref selectOption, false);
                        }
                        */
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
                                players.ForEach(x => x.turnNumber = 0);
                                //print("End enemy round");
                                playerCharacterTurnQueue.Clear();
                                CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
                                StartCoroutine(TurnText(false));
                            }
                            else
                            {
                                //Reset the turn order from Leader to last member

                                opposition.ForEach(x => x.turnNumber = 0);
                                //print("End enemy round");
                                oppositionCharacterTurnQueue.Clear();
                                CurrentBattleEngineState = BATTLE_ENGINE_STATE.NONE;
                                StartCoroutine(TurnText(true));
                            }
                        }
                        else
                        {
                            if (isPlayerTurn)
                            {
                                //print("End player turn");
                                currentCharacter.turnNumber++;
                                playerCharacterTurnQueue.Dequeue();
                                playerCharacterTurnQueue.Enqueue(currentCharacter);
                            }
                            else
                            {
                                //print("End enemy turn");
                                currentCharacter.turnNumber++;
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
                e.guardPoints = 0;
                playerCharacterTurnQueue.Enqueue(e);
                if (e.hitPoints <= 0)
                {
                    continue;
                }
                e.roundNumber++;
                for (int i = e.data.turnIcons; i > 0; i--)
                {
                    StartCoroutine(TurnIconFX(TURN_ICON_FX.APPEAR, pressTurnCount));
                    pressTurnCount++;
                }
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
                e.guardPoints = 0;
                oppositionCharacterTurnQueue.Enqueue(e);
                if (e.hitPoints <= 0)
                {
                    continue;
                }
                e.roundNumber++;
                for (int i = e.data.turnIcons; i > 0; i--)
                {
                    StartCoroutine(TurnIconFX(TURN_ICON_FX.APPEAR, pressTurnCount));
                    pressTurnCount++;
                }
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
        if (isActive)
        {
            playerCPSlider.value = ((float)playerCP / (float)playerCPMax) * 100;
            opponentCPSlider.value = ((float)opponentCP / (float)opponentCPMax) * 100;
            playerCPText.text = " " + playerCP + "/"  + playerCPMax + " - Player";
            opponentCPText.text = "Enemy - " + opponentCP + "/" + opponentCPMax;

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

                switch (b.currentStatus)
                {
                    case STATUS_EFFECT.POISON:

                        guis[i].img.color = Color.magenta;
                        break;

                    default:
                        guis[i].img.color = Color.white;
                        break;

                }
                guis[i].txt.color = b.data.character_colour;
                guis[i].txt.text = b.name;
                guis[i].HPTxt.color = b.data.character_colour;
                guis[i].HPTxt.text = "RP: " + b.hitPoints;
                //guis[i].SPTxt.text = "SP: " + b.skillPoints;
                if (b.guardPoints > 0)
                {
                    guis[i].guardPTS.text = "+" + b.guardPoints;
                    guis[i].guardPTSS.text = "+" + b.guardPoints;
                    guis[i].guard.SetActive(true);
                }
                else
                    guis[i].guard.SetActive(false);

                if (b.hitPoints > b.hitPointMeter)
                    b.hitPointMeter += Time.deltaTime * b.meterSpeed;
                else if (b.hitPoints < b.hitPointMeter)
                    b.hitPointMeter -= Time.deltaTime * b.meterSpeed;

                float HP = ((float)b.hitPoints / (float)b.maxHitPoints) * 100;
                //float SP = ((float)b.skillPoints / (float)b.maxSkillPoints) * 100;
                HP = Mathf.Round(HP);
                //SP = Mathf.Round(SP);
                guis[i].hpBarImg.color = b.data.character_colour;
                guis[i].hpBar.value = HP;
                //guis[i].spBar.value = SP;

                if (b.hitPoints <= 0)
                {
                    if (b == currentCharacter)
                    {
                        //b.isGuarding = true;
                        Menuchoice = 0;
                        menu_state = MENUSTATE.MENU;
                        currentCharacter = null;
                    }
                    guis[i].img.color = Color.red;
                }
                else
                    guis[i].img.color = Color.white;

                float xpos = guis[i].transform.position.x;
            }
        }
        else {
            for (int i = 0; i < guis.Count; i++)
            {
                guis[i].gameObject.SetActive(false);
            }
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
        if (i < 0)
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
                yield return new WaitForSeconds(0.45f);
                break;

            case TURN_ICON_FX.HIT:
                if (turnPressFlag == PRESS_TURN_TYPE.WEAKNESS)
                {
                    s_soundmanager.sound.PlaySound("hitWeak");
                }
                PT_GUI[i].GetComponent<Animator>().Play("PTIconBlink");
                yield return new WaitForSeconds(0.45f);
                break;

            case TURN_ICON_FX.FADE:
                PT_GUI[i].GetComponent<Animator>().Play("PTiconDissapear");
                yield return new WaitForSeconds(0.45f);
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

    public static bool EligibleForIncrease(int lev, float growthStat)
    {
        float statInc = lev * growthStat;
        float prevStatInc = (lev - 1) * growthStat;
        if (Mathf.Floor(statInc) > Mathf.Floor(prevStatInc))
            return true;
        return false;
    }
    public IEnumerator ResultsShow(o_battleChar[] targ, float expMult) {
        earningsBattle.text = "";
        foreach (Slider expList in EXPList) {
            expList.gameObject.SetActive(false);
        }
        foreach (Text expText in EXPText) {
            expText.text = "";
        }
        for (int i = 0; i < players.Count; i++)
        {
            EXPList[i].gameObject.SetActive(true);
            EXPList[i].value = players[i].exp;
            EXPText[i].text = players[i].name + " Level: " + players[i].level;
        }
        EXPResults.SetActive(true);

        List<string> movesList = new List<string>();
        Dictionary<string, int> items = new Dictionary<string, int>();
        
        foreach (o_battleChar chTarg in targ)
        {
            if (chTarg.hitPoints <= 0)
            {
                RPG_battleMemory mem = rpg_globals.GetInstance().GetComponent<rpg_globals>().GetBattleMemory(chTarg);
                mem.encountered = true;
                List<s_move> skillsToLearn = chTarg.skillMoves.FindAll
                    (x => x.canLearn);

                if (chTarg.spareDrops != null)
                {

                    foreach (s_move it in chTarg.spareDrops)
                    {
                        if (!items.ContainsKey(it.name))
                        {
                            items.Add(it.name, 1);
                        }
                        else
                        {
                            items[it.name]++;
                        }
                        rpg_globals.gl.AddItem(it.name, 1);
                    }
                }
                for (int i = 0; i < skillsToLearn.Count; i++)
                {
                    s_move skill = skillsToLearn[i];
                    if (skill.cost == 0)
                        continue;
                    if (!enemySkillsUsed.Contains(skill.name)) {
                        continue;
                    }
                    if (!rpg_globals.gl.extraSkills.Contains(skill))
                    {
                        rpg_globals.gl.extraSkills.Add(skill);
                        movesList.Add(skill.name);
                    }
                }
            }
            //moneyTot += chTarg.moneySpare;
        }
        foreach (KeyValuePair<string, int> it in items)
        {
            earningsBattle.text += "\n" + it.Key + " x " + it.Value;
        }
        if (movesList.Count > 0)
        {
            if (movesList.Count > 1)
                earningsBattle.text += "\n" + "Gained extra skills ";
            else
                earningsBattle.text += "\n" + "Gained extra skill ";
            for (int i = 0; i < movesList.Count; i++)
            {
                string it = movesList[i];
                earningsBattle.text += it;
                if (movesList.Count > 1 && i != movesList.Count)
                    earningsBattle.text += ',';
            }
        }
        actionDisp.text = "";

        for (int i = 0; i < players.Count; i++)
        {
            o_battleChar bc = players[i];
            int exp = 0;
            foreach (o_battleChar chTarg in targ)
            {
                if (chTarg.hitPoints <= 0)
                    exp += bc.CalculateExp(chTarg, 1);
            }
            yield return new WaitForSeconds(0.05f);
            List<string> extra_skills_gained = new List<string>();
            for (int i2 = 0; i2 < exp; i2++)
            {
                bc.exp += 1;
                if (bc.exp >= 100)
                {
                    BattleCharacterData ch = bc.data;
                    bc.level++;
                    if (EligibleForIncrease(bc.level, ch.attackG))
                    {
                        bc.attack++;
                    }
                    if (EligibleForIncrease(bc.level, ch.defenceG))
                    {
                        bc.defence++;
                    }
                    if (EligibleForIncrease(bc.level, ch.intelligenceG))
                    {
                        bc.intelligence++;
                    }
                    if (EligibleForIncrease(bc.level, ch.speedG))
                    {
                        bc.speed++;
                    }
                    if (EligibleForIncrease(bc.level, ch.gutsG))
                    {
                        bc.guts++;
                    }
                    bc.maxHitPoints += UnityEngine.Random.Range(bc.data.maxHitPointsGMin, bc.data.maxHitPointsGMax);
                    bc.maxSkillPoints += UnityEngine.Random.Range(bc.data.maxSkillPointsGMin, bc.data.maxSkillPointsGMax);

                    //bc.skillPoints = bc.maxSkillPoints;
                    bc.hitPoints = bc.maxHitPoints;
                    foreach (s_move mov in bc.data.moveDatabase)
                    {
                        if (bc.skillMoves.Contains(mov))
                            continue;
                        if (mov.str_req <= bc.attack
                            && mov.vit_req <= bc.defence
                            && mov.dex_req <= bc.intelligence
                            && mov.agi_req <= bc.speed
                            && mov.gut_req <= bc.guts
                            )
                        {
                            extra_skills_gained.Add(mov.name);
                            bc.skillMoves.Add(mov);
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

            if (extra_skills_gained.Count > 0)
            {
                string movesLearned = "";
                for (int i2 = 0; i2 < extra_skills_gained.Count; i2++)
                {
                    string mov = extra_skills_gained[i2];
                    movesLearned += mov + ", ";
                }
                earningsBattle.text += "\n" + bc.name + " learned " + movesLearned + "\n";
            }
        }
        yield return new WaitForSeconds(1.5f);
    }
    
    /*
    public IEnumerator SpareEnemy(o_battleChar[] targ, int mode) {
        CurrentBattleEngineState = BATTLE_ENGINE_STATE.MOVE_PROCESS;
        drawExp = true;

        sparable = new List<o_battleChar>();
        sparable = targ.ToList().FindAll(x => x.skillPoints <= 0);


        yield return StartCoroutine(ResultsShow(targ, 1));

        float multiplier = 0.15f * targ.Length;
        
        foreach (o_battleChar ch in players) {
            ch.skillPoints += (int)((float)ch.maxSkillPoints * multiplier);
            ch.skillPoints = Mathf.Clamp(ch.skillPoints, 0, ch.maxSkillPoints);
        }

        float moneyTot = 0;

        Dictionary<string, int> items = new Dictionary<string, int>();
        List<string> movesList = new List<string>();

        //TODO GIVE PLAYER DROPS
        foreach (o_battleChar chTarg in sparable)
        {
            RPG_battleMemory mem = rpg_globals.GetInstance().GetComponent<rpg_globals>().GetBattleMemory(chTarg);
            mem.encountered = true;

            if (chTarg.spareDrops != null) {

                foreach (s_move it in chTarg.spareDrops)
                {
                    if (!items.ContainsKey(it.name))
                    {
                        items.Add(it.name, 1);
                    }
                    else {
                        items[it.name]++;
                    }
                    rpg_globals.gl.AddItem(it.name, 1);
                }
            }
        }
        foreach (KeyValuePair<string, int> it in items) {

            earningsBattle.text += "\n" + it.Key + " x " + it.Value;
        }
        yield return new WaitForSeconds(1.4f);

        foreach (o_battleChar chTarg in targ)
        {
            opposition.Remove(chTarg);
        }
        Menuchoice = 0;
        //yield return StartCoroutine(ResultsShow(targ, 0.35f));
        yield return StartCoroutine(ConcludeBattle());

    }
    */

    public void EndPlayerTurn()
    {
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
        allmove.Add(guardMove);
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

                    case charAI.CONDITIONS.SELF_HP_HIGHER:
                        {
                            if (character.hitPoints > chai.healthPercentage * character.maxHitPoints)
                            {
                                target = Targ;
                                breakOutOfLoop = true;
                            }
                        }
                        break;
                    case charAI.CONDITIONS.SELF_HP_LOWER:
                        {
                            if (character.hitPoints < chai.healthPercentage * character.maxHitPoints)
                            {
                                target = Targ;
                                breakOutOfLoop = true;
                            }
                        }
                        break;

                    case charAI.CONDITIONS.SELF_SP_HIGHER:
                        {
                            if (character.skillPoints > chai.healthPercentage * character.maxSkillPoints)
                            {
                                target = Targ;
                                breakOutOfLoop = true;
                            }
                        }
                        break;
                    case charAI.CONDITIONS.SELF_SP_LOWER:
                        {
                            if (character.skillPoints < chai.healthPercentage * character.maxSkillPoints)
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
                    case charAI.CONDITIONS.USER_PARTY_SP_HIGHER:
                        {
                            Targ = opposition.Find(x => x.skillPoints > chai.healthPercentage * x.maxSkillPoints);
                            if (Targ != null)
                            {
                                target = Targ;
                                breakOutOfLoop = true;
                            }
                        }
                        break;
                    case charAI.CONDITIONS.USER_PARTY_SP_LOWER:
                        {
                            Targ = opposition.Find(x => x.skillPoints < chai.healthPercentage * x.maxSkillPoints);
                            if (Targ != null)
                            {
                                target = Targ;
                                breakOutOfLoop = true;
                            }
                        }
                        break;

                }
                switch (chai.turnCounters)
                {
                    case charAI.TURN_COUNTER.TURN_COUNTER_EQU:
                        if (currentCharacter.turnNumber == chai.number1)
                        {
                            currentCharacter.turnNumber = 0;
                            breakOutOfLoop = true;
                        }
                        else
                        {
                            breakOutOfLoop = false;
                        }
                        break;

                    case charAI.TURN_COUNTER.ROUND_COUNTER_EQU:
                        if (currentCharacter.roundNumber == chai.number2)
                        {
                            currentCharacter.roundNumber = 0;
                            breakOutOfLoop = true;
                        }
                        else {
                            breakOutOfLoop = false;
                        }
                        break;

                    case charAI.TURN_COUNTER.ROUND_TURN_COUNTER_EQU:
                        if (currentCharacter.roundNumber == chai.number2 &&
                            currentCharacter.turnNumber == chai.number1)
                        {
                            currentCharacter.roundNumber = 0;
                            currentCharacter.turnNumber = 0;
                            breakOutOfLoop = true;
                        }
                        else
                        {
                            breakOutOfLoop = false;
                        }
                        break;
                }
                if (breakOutOfLoop)
                    break;
            }
        else {
            target = candidates[UnityEngine.Random.Range(0, candidates.Count - 1)];
            if (allmove.Count == 0 || character.skillPoints <= 0) {
                move = guardMove;
                target = null;
            }
            else
            {
                move = allmove[UnityEngine.Random.Range(0, allmove.Count - 1)];
            }
        }
        currentMove = new s_battleAction (
                character,
                target,
                move);
    }
    }
*/

/*
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
*/

/*
float acc1 = ((currentMove.user.getNetSpeed / 8) * 6.25f);
float acc2 = ((Targ.getNetSpeed / 8) * 6.25f);

float accuracy_percentage = (float)currentMove.move.accuracy * (acc1/acc2);
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
    if (mov.element != ELEMENT.UNKNOWN) {
        if (Targ.elementTypeCharts[(int)mov.element] > 1.99f || Targ.actionTypeCharts[(int)mov.action_type] > 1.99f)
        {
            if (Targ.guardPoints > 0 && mov.moveType != MOVE_TYPE.TALK)
            {
                turnPressFlag = PRESS_TURN_TYPE.NORMAL;
            }
            else
            {
                turnPressFlag = PRESS_TURN_TYPE.WEAKNESS;
            }
            if (Targ.actionTypeCharts[(int)mov.action_type] > 1.99f && Targ.skillPoints <= 0)
            {
                turnPressFlag = PRESS_TURN_TYPE.NORMAL;
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
        if (players.Contains(currentMove.user))
        {
            if (mov.moveType == MOVE_TYPE.SPECIAL ||
               mov.moveType == MOVE_TYPE.PHYSICAL)
            {
                rpg_globals.GetInstance().GetComponent<rpg_globals>().SetBattleMemoryElement(Targ.name, mov.element);
            }
            else if (mov.moveType == MOVE_TYPE.TALK)
            {
                rpg_globals.GetInstance().GetComponent<rpg_globals>().SetBattleMemoryAction(Targ.name, mov.action_type);
            }
        }
    }
}
*/

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
