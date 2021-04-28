﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class BattleCharacterData : ScriptableObject
{
    public charAI[] characterAI;

    public Sprite[] characterAnims;

    public Sprite front;
    public Sprite back;

    public bool isPartyChar;

    public int level;
    public int exp;     //The expereince that the character has
    public int expToNextLevel; // The experience that the character needs to get to the next level
    public float baseExpYeild;    //The expereince that the character gives multiplied by its level

    public int maxHitPoints;
    public float couragePoints;     //Out of 100, used for talking and non-magic attacks
    public int maxSkillPoints;

    /// Base stats the character starts with

    public int maxSkillPointsB = 1;
    public int maxHitPointsB = 1;
    public int attackB = 1;
    public int defenceB = 1;
    public int intelligenceB = 1;
    public int gutsB = 1;
    public int speedB = 1;
    public int luckB = 1;

    /// Determines the growth of certain stats per turn

    //Health and Stamina will be determined by 
    public int maxSkillPointsGMax;
    public int maxSkillPointsGMin;

    public int maxHitPointsGMax;
    public int maxHitPointsGMin;

    public int attackG = 1;
    public int defenceG = 1;
    public int intelligenceG = 1;
    public int gutsG = 1;
    public int speedG = 1;
    public int luckG = 1;

    public bool inBattle = true;

    public List<o_battleChar.move_learn> moveDatabase = new List<o_battleChar.move_learn>();
    public List<s_move> currentMoves = new List<s_move>();
    public STATUS_EFFECT currentStatus = STATUS_EFFECT.NONE;

    public float[] elementTypeCharts = new float[13] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };   //-1 -> absorb, 0 -> immune, 1 -> normal, 2-> weak, 3 -> knockdown 
    public float[] actionTypeCharts = new float[6] { 1, 1, 1, 1, 1 ,1};    //-1 -> absorb, 0 -> immune, 1 -> normal, 2-> weak, 3 -> knockdown)
    public float money;
    public List<rpg_item> itemDrop;

    //The characters that this one has relationships with prevent this one from being negotated 
    public BattleCharacterData[] relationships;
}