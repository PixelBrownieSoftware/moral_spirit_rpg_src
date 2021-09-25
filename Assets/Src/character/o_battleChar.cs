using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Elements: Fire, Ice, Wind, Electric, Psychic, Light, Dark, Normal, Strike, ??? (nothing is weak, immune or strong aginst this type)
/// Act types: Comfort, Playful, Threat, Flirt, Intellect. 
/// </summary>
public enum ELEMENT
{
    UNKNOWN = -1,
    NORMAL,
    FORCE,
    FIRE,
    ICE,
    WIND,
    ELECTRIC,
    PSYCHIC,
    LIGHT,
    DARK,
    POISON,
    EARTH,
    PEIRCE
}
public enum ACTION_TYPE
{
    NONE,
    COMFORT,
    PLAYFUL,
    THREAT,
    FLIRT,
    RESERVED,
    PSYCHIC,
    LIGHT,
    DARK
}
public enum STATUS_EFFECT
{
    NONE,
    POISON,     //Gets damaged until cured
    BLIND,      //Often misses
    SILENCE,    //Cannot cast magic or act
    ANXIETY,    //Cannot act
    SLEEP,      //Cannot do anything, is cured by being attacked, regenerates SP and HP, defence is largely compromised
    NAUSEA,     //Cannot consume food items
    CONFUSED,   //Does random attacks, including party members
    CHARM,      //Supports whoever charmed them and attacks party members
    CRYING,     //Cannot do "Threat" actions
    BLEEDING,   //HP meter goes down quicker
    RAGE,       //Defence radically decreaeses and Attack radically increases, can only do "Threat" act moves
    PARALYZED,  //Stops affected person from attacking unless immune, absorb electriciity, this mostly fails when one is resistant
    FROZEN,     //Ditto ^ except for ice
    BURN,       //Like poison, but doesn't work for characters that immune/absorb fire, mostly fails on resistant ones, if resistant burn, it does little damage
    DOWN,       //Cannot attack for a bit
    UNCONSIOUS  //Pretty much defeated
}
public enum MOVE_TYPE
{
    PHYSICAL,
    SPECIAL,
    STATUS,
    TALK,
    MISC,
    GUARD
}
public enum STATUS_MOVE_TYPE
{
    NONE,
    HEAL,
    CURE_STATUS,
    HEAL_STAMINA,
    BUFF,
    DEBUFF,
    REVIVE,
    CUSTOM
}

[System.Serializable]
public class charAI
{
    public enum CONDITIONS
    {
        ALWAYS,
        ELEMENT_TARG,
        PARTICULAR_TARG,
        TARGET_PARTY_NUM,
        USER_PARTY_NUM,
        USER_PARTY_HP_HIGHER,
        USER_PARTY_HP_LOWER,
        TARGET_PARTY_HP_LOWER,
        TARGET_PARTY_HP_HIGHER,
        TARGET_PARTY_HP_LOWEST,
        TARGET_PARTY_HP_HIGHEST,
        TARGET_PARTY_LEVEL,
        ON_TURN,
        SELF_HP_LOWER,
        SELF_HP_HIGHER,
        SELF_SP_HIGHER,
        SELF_SP_LOWER,
        USER_PARTY_SP_HIGHER,
        USER_PARTY_SP_LOWER,
        USER_PARTY_HP_LOWEST,
        USER_PARTY_HP_HIGHEST
    }
    public enum TURN_COUNTER
    {
        NONE,
        TURN_COUNTER_EQU,
        ROUND_COUNTER_EQU,
        ROUND_TURN_COUNTER_EQU,
    }
    public TURN_COUNTER turnCounters;
    public CONDITIONS conditions;
    public ELEMENT element;
    public float healthPercentage;
    public string name;
    public s_move moveName;
    public bool onParty;
    public int number1;
    public int number2;
    public bool isImportant = false;    //This would be done first if the condition is met
}

[System.Serializable]
public class o_battleCharSaveData {
    public string dataSrc;

    public string name;
    public int level;
    public float exp;

    public int hitPoints;
    public int maxHitPoints;
    //public int skillPoints;
    public int maxSkillPoints;

    public int attack;
    public int defence;
    public int intelligence;
    public int guts;
    public int speed;
    public int luck;


    public bool inBattle = true;
    public List<string> currentMoves = new List<string>();
    public List<string> extraMoves = new List<string>();
    public STATUS_EFFECT currentStatus = STATUS_EFFECT.NONE;
}

[System.Serializable]
public class o_battleCharData
{
    public BattleCharacterData dataSrc;
    public charAI[] characterAI;

    public string[] characterAnims;

    public bool isPartyChar;

    public string name;
    public int level;
    public float exp;     //The expereince that the character has
    public int baseExpYeild;    //The expereince that the character gives multiplied by its level

    public int hitPoints;
    public int maxHitPoints;
    //public int skillPoints;
    public int maxSkillPoints;

    public int attack;
    public int defence;
    public int intelligence;
    public int guts;
    public int speed;
    
    public bool inBattle = true;

    public List<o_battleChar.move_learn> moveDatabase = new List<o_battleChar.move_learn>();
    public List<s_move> currentMoves = new List<s_move>();
    public List<s_move> extra_skills = new List<s_move>();
    public STATUS_EFFECT currentStatus = STATUS_EFFECT.NONE;
    public float money;
    public List<Tuple<rpg_item, float>> itemDrop;
}

[System.Serializable]
public class o_battleChar : MonoBehaviour
{
    [System.Serializable]
    public class move_learn
    {
        public s_move move;
        public int level;
        public move_learn(int level, s_move move)
        {
            this.level = level;
            this.move = move;
        }
    }


    //Can't be spared unless this one is spared
    public List<o_battleChar> relationships;

    public int level;
    public float exp;     //The expereince that the character has
    public float baseExpYeild;    //The expereince that the character gives multiplied by its level

    public float hitPointMeter;     //For that Earthbound/Mother-esque health meter that rolls down

    public int hitPoints;
    public int maxHitPoints;
    //public int skillPoints;
    public float couragePoints;     //Out of 100, used for talking and non-magic attacks
    public int maxSkillPoints;

    public int attack;
    public int defence;
    public int intelligence;
    public int guts;
    public int speed;

    public float attackBuff;
    public float defenceBuff;
    public float intelligenceBuff;
    public float gutsBuff;
    public float speedBuff;

    public float meterSpeed;
    const float meterSpeedDefault = 6.5f;

    public int guardPoints = 0;
    
    public List<move_learn> moveDatabase = new List<move_learn>();
    public charAI[] characterAI;
    public int turnNumber = 0;
    public int roundNumber = 0;

    public s_move currentMove;
    public rpg_item itemUsed;
    public o_battleChar targetCharacter;

    public rpg_item[] defeatDrops;
    public s_move[] spareDrops;
    public float moneySpare;

    public List<s_move> skillMoves;
    public List<s_move> extra_skills;
    public int CurrentMoveNum;

    public MOVE_TYPE move_typ;
    public STATUS_EFFECT currentStatus = STATUS_EFFECT.NONE;
    int statusLast = 0;
    int statusCooldown = 0;
    public int statusDur { get { return statusLast; } }

    public float[] elementTypeCharts = new float[11] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };   //-1 -> absorb, 0 -> immune, 1 -> normal, 2-> weak, 3 -> knockdown 
    public float[] actionTypeCharts = new float[9] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };    //-1 -> absorb, 0 -> immune, 1 -> normal, 2-> weak, 3 -> knockdown)

    public SpriteRenderer rend;
    public Sprite[] sprites;

    public Sprite front;
    public Sprite back;

    Sprite[] animations;
    int animNum = 0;
    float animSpeed;
    float animPT;

    public float getNetAttack {
        get
        {
            float total = attack + attackBuff;
            if (total <= 0)
                return 0.3f;
            return total;
        }
    }
    public float getNetDefence
    {
        get
        {
            float total = defence + defenceBuff;
            if (total <= 0)
                return 0.3f;
            return total;
        }
    }
    public float getNetIntellgence
    {
        get
        {
            float total =  intelligence + intelligenceBuff;
            if (total <= 0)
                return 0.3f;
            return total;
        }
    }
    public float getNetGuts
    {
        get
        {
            float total = guts + gutsBuff;
            if (total <= 0)
                return 1f;
            return total;
        }
    }
    public float getNetSpeed
    {
        get
        {
            float total = speed + speedBuff;
            if (total <= 0)
                return 1f;
            return total;
        }
    }
    public List<s_move> allSkills {
        get {
            List<s_move> allSkill = new List<s_move>();
            allSkill.AddRange(skillMoves);
            allSkill.AddRange(extra_skills);
            return allSkill;
        }
    }
    public bool ableToGetStatus {
        get {
            if (statusLast == 0 && statusCooldown == 0) {
                return true;
            }
            return false;
        }
    }

    public BattleCharacterData data;

    public void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        hitPointMeter = hitPoints;
    }

    public void OnTurnStuff() {
        if (statusCooldown > 0) {
            statusCooldown--;
        } else if(statusLast > 0) {
            
            statusLast--;
            if (statusLast == 0)
                currentStatus = STATUS_EFFECT.NONE;
        }
    }

    public void SetStatusEffect(STATUS_EFFECT status) {
        if (ableToGetStatus)
        {
            statusLast = 3;
            currentStatus = status;
        }
    }

    public void AddAnimations(string[] nm) 
    {
        if (nm == null)
            return;
        animations = new Sprite[nm.Length];
        for (int i =0; i < nm.Length; i++) {
            string str = nm[i];
            foreach (Sprite spr in sprites)
            {
                if (str == spr.name)
                {
                    animations[i] = spr;
                    break;
                }
            }
        }
    }

    public void Update()
    {
        if (hitPoints > 0) {
            switch (currentStatus)
            {
                default:
                    //rend.color = Color.white;
                    break;

                case STATUS_EFFECT.POISON:
                    //rend.color = Color.magenta;
                    break;
            }
        }

        attackBuff = Mathf.Clamp(attackBuff, -5,5);
        defenceBuff = Mathf.Clamp(defenceBuff, -5,5);
        speedBuff = Mathf.Clamp(speedBuff, -5,5);
        intelligenceBuff = Mathf.Clamp(intelligenceBuff, -5,5);
        gutsBuff = Mathf.Clamp(gutsBuff, -5,5);

        if (animations != null)
        {
            if (animations.Length > 0)
            {
                rend.sprite = animations[animNum];
                if (animations.Length > 1)
                {
                    animPT -= Time.deltaTime;
                    if (animPT < 0)
                    {
                        animPT = animSpeed;
                        animNum++;
                        if (animNum > animations.Length - 1)
                            animNum = 0;
                    }
                }
            }
        }
    }

    public int CalculateMove(ref s_battleAction move) {

        int movePow = 0;

        s_move mov = null;

        switch (move.type)
        {
            case s_battleAction.MOVE_TYPE.MOVE:
                mov = move.move;
                break;

            case s_battleAction.MOVE_TYPE.ITEM:
                mov = move.item.action;
                break;
        }
        if (!mov.isFixed)
        {
            switch (mov.moveType)
            {
                default:
                    if (mov.element != ELEMENT.UNKNOWN)
                    {
                        movePow = (int)(mov.power *
                            (((move.user.level + 1) / 15) +
                            (float)(GetElementDamage(move) / (float)(getNetDefence + guardPoints))) *
                            elementTypeCharts[(int)mov.element]);
                    }
                    else
                    {
                        movePow = (int)(mov.power *
                            (((move.user.level + 1) / 15) +
                            (float)(GetElementDamage(move) / (float)(getNetDefence + guardPoints))));
                    }
                    /*
                    movePow = (int)(
                         (mov.power *
                         (((move.user.level + 1) / 16) + 
                         ((float)move.user.getNetAttack / (float)(getNetDefence + guardModifier))) *
                         elementTypeCharts[(int)mov.element]));
                    */
                    break;

                //Might do physical vs magic
                //Physical takes 3/4 attack and 1/4 intelligence and relies on physical defence
                //Magic takes all intelligence and relies on a mixture of physical defence and intelligence

                case MOVE_TYPE.SPECIAL:
                    if (mov.element != ELEMENT.UNKNOWN)
                    {
                        movePow = (int)(mov.power *
                        (((move.user.level + 1) / 15) +
                        (float)(GetElementDamage(move) / (float)(getNetDefence + guardPoints))) *
                        elementTypeCharts[(int)mov.element]);
                    }
                    else
                    {
                        movePow = (int)(mov.power *
                        (((move.user.level + 1) / 15) +
                        (float)(move.user.getNetIntellgence / (float)(getNetDefence + guardPoints))));
                    }
                    break;

                case MOVE_TYPE.STATUS:
                    switch (mov.statusMoveType)
                    {
                        case STATUS_MOVE_TYPE.HEAL:
                            movePow = (int)((float)mov.power * (float)(move.user.getNetIntellgence / 4));
                            break;

                        case STATUS_MOVE_TYPE.HEAL_STAMINA:
                            movePow = mov.power + (move.user.level / 2);
                            break;
                    }
                    break;

                //Talking's effectivenes relies on the guts of the defender
                case MOVE_TYPE.TALK:
                    movePow = (int)(
                        ((mov.power * 
                        (((move.user.level + 1) ) / 21.5f) + (GetTalkMoveDamage(move) / (float)GetTalkMoveDefence(move)))) *
                        (actionTypeCharts[(int)mov.action_type])) ;
                    break;
            }
        } else {
            return mov.power;
        }
        return movePow;
    }

    public float GetElementDamage(s_battleAction move ) {

        float elementFormula = 0;
        float talkFormula = 0;

        float baseStat = 0;

        switch (move.move.moveType) {
            case MOVE_TYPE.PHYSICAL:
                baseStat = move.user.getNetAttack ;
                break;

            case MOVE_TYPE.SPECIAL:
            case MOVE_TYPE.TALK:
                baseStat = move.user.getNetIntellgence;
                break;
        }

        switch (move.move.element)
        {
            case ELEMENT.FORCE:
                elementFormula = (move.user.getNetAttack / 1.8f);
                break;

            case ELEMENT.PEIRCE:
                elementFormula = 
                    (move.user.getNetAttack / 2.5f) +
                    (move.user.getNetSpeed / 2.5f);
                break;

            case ELEMENT.FIRE:
                elementFormula = move.user.getNetGuts / 5;
                break;

            case ELEMENT.ICE:
                elementFormula = move.user.getNetAttack / 2.25f;
                break;

            case ELEMENT.WIND:
                elementFormula =
                    (move.user.getNetIntellgence / 3) +
                    (move.user.getNetSpeed / 1.5f);
                break;

            case ELEMENT.ELECTRIC:
                elementFormula =
                    (move.user.getNetSpeed / 5) +
                    (move.user.getNetIntellgence / 2);
                break;

            case ELEMENT.EARTH:
                elementFormula =
                    (move.user.getNetDefence / 3) +
                    (move.user.getNetAttack / 1.6f);
                break;

            case ELEMENT.PSYCHIC:
                elementFormula = (move.user.getNetIntellgence / 1.85f);
                break;

            case ELEMENT.LIGHT:
                elementFormula =
                    (move.user.getNetIntellgence / 3) +
                    (move.user.getNetGuts / 3);
                break;

            case ELEMENT.DARK:
                elementFormula =
                    (move.user.getNetIntellgence / 3) +
                    (move.user.getNetAttack / 3);
                break;
        }

        switch (move.move.action_type)
        {
            case ACTION_TYPE.COMFORT:
                talkFormula = (move.user.getNetGuts / 1.5f);
                break;

            case ACTION_TYPE.THREAT:
                talkFormula = (move.user.getNetAttack);
                break;

            case ACTION_TYPE.RESERVED:
                talkFormula = (move.user.getNetIntellgence / 1.7F);
                break;

            case ACTION_TYPE.PLAYFUL:
                talkFormula = 
                    (move.user.getNetSpeed / 2.5f) +
                    (move.user.getNetGuts / 2);
                break;

            case ACTION_TYPE.FLIRT:
                talkFormula = 
                    (move.user.getNetAttack / 2.1F) +
                    (move.user.getNetGuts / 1.5f);
                break;
        }
        //print(elementFormula);

        float total = 0;
        switch (move.move.moveType)
        {
            case MOVE_TYPE.SPECIAL:
            case MOVE_TYPE.PHYSICAL:
                total = baseStat + (elementFormula / 1.85f);
                break;

            case MOVE_TYPE.TALK:
                total = baseStat + (elementFormula / 2.25f) + (talkFormula);
                break;
        }
        return total;
    }

    public float GetTalkMoveDefence(s_battleAction move)
    {
        float talkFormula = 0;

        switch (move.move.action_type)
        {
            case ACTION_TYPE.THREAT:
                talkFormula = (getNetGuts / 2.2f) +(getNetDefence / 3f);
                break;
            case ACTION_TYPE.PLAYFUL:
                talkFormula = (getNetGuts / 1.6f) + (getNetSpeed / 1.6f);
                break;

            case ACTION_TYPE.RESERVED:
                talkFormula = (getNetIntellgence / 1.7F) + (getNetGuts / 1.5f);
                break;

            default:
                talkFormula = getNetGuts;
                break;
        }
        print(name + " Stat: " + talkFormula);
        return talkFormula;
    }
    public float GetTalkMoveDamage(s_battleAction move)
    {
        
        float talkFormula = 0;
        
        switch (move.move.action_type)
        {
            case ACTION_TYPE.COMFORT:
                talkFormula = move.user.getNetDefence;
                break;

            case ACTION_TYPE.THREAT:
                talkFormula = move.user.getNetAttack;
                break;

            case ACTION_TYPE.RESERVED:
                talkFormula = move.user.getNetIntellgence;
                break;

            case ACTION_TYPE.PLAYFUL:
                talkFormula = move.user.getNetSpeed;
                break;

            case ACTION_TYPE.FLIRT:
                talkFormula =
                    (move.user.getNetAttack / 2.1F) +
                    (move.user.getNetGuts / 1.5f);
                break;

            case ACTION_TYPE.PSYCHIC:
                talkFormula = (move.user.getNetIntellgence / 1.85f) +
                    (move.user.getNetGuts / 2);
                break;

            case ACTION_TYPE.DARK:
                talkFormula =
                    (move.user.getNetIntellgence / 3) +
                    (move.user.getNetAttack / 3) +
                     (move.user.getNetGuts / 1.5f);
                break;
        }
        //print(elementFormula);

        print(move.user.name + " Stat: " + talkFormula);
        return talkFormula;
    }

    public int CalculateExp(o_battleChar character)
    {
        float lvl = (character.level / level);
        print(lvl);
        float final = (lvl * character.baseExpYeild);
        print(final);

        return Mathf.RoundToInt(final);
    }
    public int CalculateExp(o_battleChar character,float multiplier)
    {
        //float lvl = (character.level / level);
        float final = ((float)character.level/ (float)level) * character.baseExpYeild;

        final *= multiplier;

        return (int)final;
    }

    public void AIMove(List<o_battleChar> userTeam, List<o_battleChar> targetTeam) {
        foreach (charAI ai in characterAI) {
            switch (ai.conditions) {
                case charAI.CONDITIONS.ALWAYS:

                    break;

            
            }
        }
    }
    
}



/*
    switch (mov.moveType) {
        case MOVE_TYPE.PHYSICAL:
        case MOVE_TYPE.SPECIAL:
            guardPoints--;
            break;
    }

public int DoDamage(ref s_battleAction move)
{
    int movePow = 0;

    s_move mov = null;

    switch (move.type) {
        case s_battleAction.MOVE_TYPE.MOVE:
            mov = move.move;
            break;

        case s_battleAction.MOVE_TYPE.ITEM:
            mov = move.item.action;
            break;
    }

    switch (mov.moveType)
    {
        default:
            movePow = (int)((mov.power * ((float)move.user.getNetAttack / (float)getNetDefence)) * elementTypeCharts[(int)mov.element]);
            break;

            //Might do physical vs magic
            //Physical takes 3/4 attack and 1/4 intelligence and relies on physical defence
            //Magic takes all intelligence and relies on a mixture of physical defence and intelligence

        case MOVE_TYPE.SPECIAL:
            movePow = (int)((mov.power * ((float)move.user.getNetIntellgence / (float)getNetDefence) ) * elementTypeCharts[(int)mov.element]);
            break;

            //Talking's effectivenes relies on the guts of the defender
        case MOVE_TYPE.TALK:
            movePow = (int)(
                mov.power * 
                ((float)move.user.getNetIntellgence / (float)getNetGuts) * 
                elementTypeCharts[(int)mov.element] * 
                actionTypeCharts[(int)mov.action_type]);
            break;
    }

    //If it is a healing move
    if (mov.onTeam)
    {
        movePow = Mathf.RoundToInt(mov.power * intelligence);
        hitPoints += movePow;
        hitPoints = Mathf.Clamp(hitPoints, 0, maxHitPoints);
        return movePow;
    }

    if (mov.moveType == MOVE_TYPE.TALK)
    {
        skillPoints -= movePow;
    } else
    {
        hitPoints -= movePow;
        hitPoints = Mathf.Clamp(hitPoints, 0, maxHitPoints);
    }
    switch (mov.moveType) {
        case MOVE_TYPE.PHYSICAL:
        case MOVE_TYPE.SPECIAL:
            guardPoints--;
            break;
    }
    return movePow;
}
*/
