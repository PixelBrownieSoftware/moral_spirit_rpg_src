﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(BattleCharacterData))]
public class ed_characterdat : Editor
{
    //Character Action Stats Editor
    public enum STAT_DIST_TYPE {
        EQU,
        STR,
        MAG,
        DEF,
        SPD,
        GUT,
        STR_DEF,
        MAG_DEF,
        GUT_DEF,
        STR_SPD,
        DEF_SPD,
        MAG_SPD,
    }
    public STAT_DIST_TYPE stat_dist;

    public enum STAT_DIST_AMOUNT
    {
        VERY_LOW = -2,
        LOW = -1,
        AVERAGE = 0,
        HIGH,
        VERY_HIGH
    }
    public STAT_DIST_AMOUNT hpDist;
    public STAT_DIST_AMOUNT hpGDist;

    public STAT_DIST_AMOUNT spDist;
    public STAT_DIST_AMOUNT spGDist;

    public STAT_DIST_AMOUNT strDist;
    public STAT_DIST_AMOUNT vitDist;
    public STAT_DIST_AMOUNT dexDist;
    public STAT_DIST_AMOUNT agilDist;
    public STAT_DIST_AMOUNT gutDist;

    public STAT_DIST_AMOUNT hpDistG;
    public STAT_DIST_AMOUNT spDistG;
    public STAT_DIST_AMOUNT strDistG;
    public STAT_DIST_AMOUNT vitDistG;
    public STAT_DIST_AMOUNT dexDistG;
    public STAT_DIST_AMOUNT agilDistG;
    public STAT_DIST_AMOUNT gutDistG;

    List<charAI> characterAI;
    Sprite[] sprites;
    int listArray;
    int tab = 0;
    int charAILeng = 0;
    BattleCharacterData data = null;
    ELEMENT elementSliderSelector = ELEMENT.NORMAL;
    ACTION_TYPE actionSliderSelector = ACTION_TYPE.NONE;
    string directoryMove;
    string element;

    public int ChangeStat(ref int stat1)
    {
        int curStat = stat1;
        int prevStat = stat1;
        curStat = EditorGUILayout.IntField(curStat);
        if (curStat != prevStat)
        {
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return curStat;
    }
    public float ChangeStatFloatSlider(ref float stat1, float max, float min)
    {
        float curStat = stat1;
        float prevStat = stat1;
        curStat = EditorGUILayout.Slider(curStat, min, max);
        if (curStat != prevStat)
        {
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return curStat;
    }
    public float ChangeStatFloat(ref float stat1)
    {
        float curStat = stat1;
        float prevStat = stat1;
        curStat = EditorGUILayout.FloatField(curStat);
        if (curStat != prevStat)
        {
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return curStat;
    }

    public void GenerateMovesFromAI(charAI[] ai) {
        foreach (charAI a in ai) {
            data.moveDatabase.Add(a.moveName);
        }
    }

    public void GenerateAIFromMoves(List<s_move> moves)
    {
        List<charAI> aiList = new List<charAI>();
        foreach (s_move a in moves)
        {
            charAI cha = new charAI();
            cha.moveName = a;
            if (a.onTeam) {
                cha.onParty = true;
                if (a.statusMoveType == STATUS_MOVE_TYPE.HEAL) {
                    cha.isImportant = true;
                    cha.conditions = charAI.CONDITIONS.USER_PARTY_HP_LOWER;
                    cha.healthPercentage = 0.35f;
                }
                if (a.statusMoveType == STATUS_MOVE_TYPE.HEAL_STAMINA)
                {
                    cha.isImportant = true;
                    cha.conditions = charAI.CONDITIONS.USER_PARTY_SP_LOWER;
                    cha.healthPercentage = 0.35f;
                }
            }
            aiList.Add(cha);
        }
        data.characterAI = aiList.ToArray();
    }
    public override void OnInspectorGUI()
    {
        data = (BattleCharacterData)target;
        if (data != null)
        {
            if (GUILayout.Button("Save"))
            {
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Convert moves to AI"))
            {
                GenerateAIFromMoves(data.moveDatabase);
            }

            if (GUILayout.Button("Convert AI to moves"))
            {
                GenerateMovesFromAI(data.characterAI);
            }


            tab = GUILayout.Toolbar(tab, new string[] { "Overview", "Stats", "Moves", "Elements", "AI", "Raw data" });
            switch (tab)
            {
                #region OVERVIEW
                case 0:
                    EditorGUILayout.LabelField("Simulated stats based on level");
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Name: " + data.name);
                    data.level = (int)EditorGUILayout.Slider(data.level, 1, 100);
                    {
                        int tempHPMin = data.maxHitPointsB;
                        int tempSPMin = data.maxSkillPointsB;
                        int tempHPMax = data.maxHitPointsB;
                        int tempSPMax = data.maxSkillPointsB;
                        int tempStr = data.attackB;
                        int tempVit = data.defenceB;
                        int tempDx = data.intelligenceB;
                        int tempAg = data.speedB;
                        int tempGut = data.gutsB;
                        int tempLuc = data.luckB;

                        for (int i = 1; i < data.level; i++)
                        {
                            if (i % data.attackG == 0)
                                tempStr++;
                            if (i % data.defenceG == 0)
                                tempVit++;
                            if (i % data.intelligenceG == 0)
                                tempDx++;
                            if (i % data.speedG == 0)
                                tempAg++;
                            if (i % data.gutsG == 0)
                                tempGut++;
                            //if (i % data.luckG == 0)
                            //   tempLuc++;

                            tempHPMin += data.maxHitPointsGMin;
                            tempSPMin += data.maxSkillPointsGMin;

                            tempHPMax += data.maxHitPointsGMax;
                            tempSPMax += data.maxSkillPointsGMax;
                            //tempHP += Random.Range(data.maxHitPointsGMin, data.maxHitPointsGMax);
                            //tempSP += Random.Range(data.maxSkillPointsGMin, data.maxSkillPointsGMax);
                        }
                        EditorGUILayout.LabelField("Health (HP): " + tempHPMin + " - " + tempHPMax);
                        EditorGUILayout.LabelField("Stamina (SP): " + tempSPMin + " - " + tempSPMax);
                        EditorGUILayout.LabelField("Strength: " + tempStr);
                        EditorGUILayout.LabelField("Vitality: " + tempVit);
                        EditorGUILayout.LabelField("Dexterity: " + tempDx);
                        EditorGUILayout.LabelField("Agilty: " + tempAg);
                        EditorGUILayout.LabelField("Guts: " + tempGut);
                        EditorGUILayout.LabelField("Luck: " + tempLuc);
                    }
                    break;
                #endregion

                #region STATS
                case 1:
                    #region STAT DISTRIBUTION STUFF
                    stat_dist = (STAT_DIST_TYPE)EditorGUILayout.EnumPopup("Stat distribution", stat_dist);
                    if (GUILayout.Button("Generate Stat distribution"))
                    {
                        switch (stat_dist)
                        {

                            case STAT_DIST_TYPE.EQU:
                                data.maxHitPointsB = Random.Range(15, 25);
                                data.maxHitPointsGMax = Random.Range(4, 6);
                                data.maxHitPointsGMin = Random.Range(1, 4);

                                data.maxSkillPointsB = Random.Range(7, 15);
                                data.maxSkillPointsGMax = Random.Range(3, 5);
                                data.maxSkillPointsGMin = Random.Range(2, 3);

                                data.attackB = Random.Range(1, 4);
                                data.defenceB = Random.Range(1, 4);
                                data.intelligenceB = Random.Range(1, 4);
                                data.gutsB = Random.Range(1, 4);
                                data.speedB = Random.Range(1, 4);
                                data.luckB = Random.Range(1, 4);

                                data.attackG = Random.Range(2, 4);
                                data.defenceG = Random.Range(2, 4);
                                data.intelligenceG = Random.Range(2, 4);
                                data.gutsG = Random.Range(2, 4);
                                data.speedG = Random.Range(2, 4);
                                data.luckG = Random.Range(2, 4);
                                break;

                            case STAT_DIST_TYPE.STR:
                                data.maxHitPointsB = Random.Range(18, 26);
                                data.maxHitPointsGMax = Random.Range(4, 6);
                                data.maxHitPointsGMin = Random.Range(1, 4);

                                data.maxSkillPointsB = Random.Range(7, 15);
                                data.maxSkillPointsGMax = Random.Range(3, 5);
                                data.maxSkillPointsGMin = Random.Range(2, 3);

                                data.attackB = Random.Range(4, 6);
                                data.defenceB = Random.Range(1, 3);
                                data.intelligenceB = Random.Range(1, 3);
                                data.gutsB = Random.Range(1, 3);
                                data.speedB = Random.Range(1, 3);
                                data.luckB = Random.Range(1, 3);

                                data.attackG = Random.Range(2, 3);
                                data.defenceG = Random.Range(2, 4);
                                data.intelligenceG = Random.Range(2, 4);
                                data.gutsG = Random.Range(2, 4);
                                data.speedG = Random.Range(2, 4);
                                data.luckG = Random.Range(2, 4);
                                break;

                            case STAT_DIST_TYPE.MAG:
                                data.maxHitPointsB = Random.Range(18, 26);
                                data.maxHitPointsGMax = Random.Range(3, 6);
                                data.maxHitPointsGMin = Random.Range(1, 3);

                                data.maxSkillPointsB = Random.Range(16, 35);
                                data.maxSkillPointsGMax = Random.Range(4, 7);
                                data.maxSkillPointsGMin = Random.Range(1, 4);

                                data.attackB = Random.Range(1, 3);
                                data.defenceB = Random.Range(1, 3);
                                data.intelligenceB = Random.Range(4, 6);
                                data.gutsB = Random.Range(1, 3);
                                data.speedB = Random.Range(1, 3);
                                data.luckB = Random.Range(1, 3);

                                data.attackG = Random.Range(2, 4);
                                data.defenceG = Random.Range(2, 4);
                                data.intelligenceG = Random.Range(2, 3);
                                data.gutsG = Random.Range(2, 4);
                                data.speedG = Random.Range(2, 4);
                                data.luckG = Random.Range(2, 4);
                                break;

                            case STAT_DIST_TYPE.DEF:
                                data.maxHitPointsB = Random.Range(18, 26);
                                data.maxHitPointsGMax = Random.Range(4, 6);
                                data.maxHitPointsGMin = Random.Range(1, 4);

                                data.maxSkillPointsB = Random.Range(7, 15);
                                data.maxSkillPointsGMax = Random.Range(3, 5);
                                data.maxSkillPointsGMin = Random.Range(2, 3);

                                data.attackB = Random.Range(1, 3);
                                data.defenceB = Random.Range(4, 6);
                                data.intelligenceB = Random.Range(1, 3);
                                data.gutsB = Random.Range(2, 3);
                                data.speedB = Random.Range(1, 3);
                                data.luckB = Random.Range(1, 3);

                                data.attackG = Random.Range(1, 4);
                                data.defenceG = Random.Range(1, 3);
                                data.intelligenceG = Random.Range(1, 4);
                                data.gutsG = Random.Range(1, 4);
                                data.speedG = Random.Range(1, 4);
                                data.luckG = Random.Range(1, 4);
                                break;

                            case STAT_DIST_TYPE.MAG_DEF:
                                data.maxHitPointsB = Random.Range(14, 26);
                                data.maxHitPointsGMax = Random.Range(3, 5);
                                data.maxHitPointsGMin = Random.Range(1, 3);

                                data.maxSkillPointsB = Random.Range(21, 37);
                                data.maxSkillPointsGMax = Random.Range(4, 7);
                                data.maxSkillPointsGMin = Random.Range(3, 4);

                                data.attackB = Random.Range(1, 3);
                                data.defenceB = Random.Range(3, 5);
                                data.intelligenceB = Random.Range(3, 5);
                                data.gutsB = Random.Range(1, 3);
                                data.speedB = Random.Range(1, 3);
                                data.luckB = Random.Range(1, 3);

                                data.attackG = Random.Range(1, 4);
                                data.defenceG = Random.Range(1, 3);
                                data.intelligenceG = Random.Range(1, 3);
                                data.gutsG = Random.Range(1, 4);
                                data.speedG = Random.Range(1, 4);
                                data.luckG = Random.Range(1, 4);
                                break;

                            case STAT_DIST_TYPE.STR_DEF:
                                data.maxHitPointsB = Random.Range(14, 26);
                                data.maxHitPointsGMax = Random.Range(3, 5);
                                data.maxHitPointsGMin = Random.Range(1, 3);

                                data.maxSkillPointsB = Random.Range(21, 37);
                                data.maxSkillPointsGMax = Random.Range(4, 7);
                                data.maxSkillPointsGMin = Random.Range(3, 4);

                                data.attackB = Random.Range(3, 5);
                                data.defenceB = Random.Range(3, 5);
                                data.intelligenceB = Random.Range(1, 3);
                                data.gutsB = Random.Range(1, 3);
                                data.speedB = Random.Range(1, 3);
                                data.luckB = Random.Range(1, 3);

                                data.attackG = Random.Range(1, 3);
                                data.defenceG = Random.Range(1, 3);
                                data.intelligenceG = Random.Range(1, 4);
                                data.gutsG = Random.Range(1, 4);
                                data.speedG = Random.Range(1, 4);
                                data.luckG = Random.Range(1, 4);
                                break;

                            case STAT_DIST_TYPE.GUT_DEF:
                                data.maxHitPointsB = Random.Range(19, 26);
                                data.maxHitPointsGMax = Random.Range(4, 6);
                                data.maxHitPointsGMin = Random.Range(1, 4);

                                data.maxSkillPointsB = Random.Range(12, 18);
                                data.maxSkillPointsGMax = Random.Range(3, 5);
                                data.maxSkillPointsGMin = Random.Range(2, 3);

                                data.attackB = Random.Range(1, 3);
                                data.defenceB = Random.Range(3, 5);
                                data.intelligenceB = Random.Range(1, 3);
                                data.gutsB = Random.Range(3, 5);
                                data.speedB = Random.Range(1, 3);
                                data.luckB = Random.Range(1, 3);

                                data.attackG = Random.Range(1, 4);
                                data.defenceG = Random.Range(1, 3);
                                data.intelligenceG = Random.Range(1, 4);
                                data.gutsG = Random.Range(1, 3);
                                data.speedG = Random.Range(1, 4);
                                data.luckG = Random.Range(1, 4);
                                break;

                            case STAT_DIST_TYPE.DEF_SPD:
                                data.maxHitPointsB = Random.Range(19, 26);
                                data.maxHitPointsGMax = Random.Range(4, 6);
                                data.maxHitPointsGMin = Random.Range(1, 4);

                                data.maxSkillPointsB = Random.Range(12, 18);
                                data.maxSkillPointsGMax = Random.Range(3, 5);
                                data.maxSkillPointsGMin = Random.Range(2, 3);

                                data.attackB = Random.Range(1, 3);
                                data.defenceB = Random.Range(3, 5);
                                data.intelligenceB = Random.Range(1, 3);
                                data.gutsB = Random.Range(1, 3);
                                data.speedB = Random.Range(3, 5);
                                data.luckB = Random.Range(1, 3);

                                data.attackG = Random.Range(1, 4);
                                data.defenceG = Random.Range(1, 3);
                                data.intelligenceG = Random.Range(1, 4);
                                data.gutsG = Random.Range(1, 4);
                                data.speedG = Random.Range(1, 3);
                                data.luckG = Random.Range(1, 4);
                                break;
                        }
                    }

                    #region STAT VARIABLES
                    Vector2Int BSvlowBoundPTS = new Vector2Int(4, 9);
                    Vector2Int BSlowBoundPTS = new Vector2Int(6, 12);
                    Vector2Int BSavgBoundPTS = new Vector2Int(8, 15);
                    Vector2Int BShighBoundPTS = new Vector2Int(11, 18);
                    Vector2Int BSvhighBoundPTS = new Vector2Int(14, 25);

                    Vector2Int LWvlowBoundPTS = new Vector2Int(1, 1);
                    Vector2Int LWlowBoundPTS = new Vector2Int(1, 3);
                    Vector2Int LWavgBoundPTS = new Vector2Int(1, 4);
                    Vector2Int LWhighBoundPTS = new Vector2Int(2, 4);
                    Vector2Int LWvhighBoundPTS = new Vector2Int(3, 4);

                    Vector2Int HGvlowBoundPTS = new Vector2Int(1, 2);
                    Vector2Int HGlowBoundPTS = new Vector2Int(1, 3);
                    Vector2Int HGavgBoundPTS = new Vector2Int(1, 4);
                    Vector2Int HGhighBoundPTS = new Vector2Int(2, 5);
                    Vector2Int HGvhighBoundPTS = new Vector2Int(4, 7);

                    Vector2Int vlowBoundB = new Vector2Int(1, 2);
                    Vector2Int lowBoundB = new Vector2Int(1, 3);
                    Vector2Int avgBoundB = new Vector2Int(2, 4);
                    Vector2Int highBoundB = new Vector2Int(3, 5);
                    Vector2Int vhighBoundB = new Vector2Int(4, 6);

                    Vector2Int vlowBoundG = new Vector2Int(4, 5);
                    Vector2Int lowBoundG = new Vector2Int(3, 5);
                    Vector2Int avgBoundG = new Vector2Int(2, 4);
                    Vector2Int highBoundG = new Vector2Int(2, 3);
                    Vector2Int vhighBoundG = new Vector2Int(1, 2);
                    #endregion

                    EditorGUILayout.LabelField("Base stats");
                    hpDist = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Health distribution", hpDist);
                    spDist = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Stamina distribution", spDist);
                    strDist = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Strength distribution", strDist);
                    dexDist = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Dexterity distribution", dexDist);
                    vitDist = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Vitality distribution", vitDist);
                    agilDist = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Agility distribution", agilDist);
                    gutDist = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Gut distribution", gutDist);

                    EditorGUILayout.LabelField("Growth stats");
                    hpDistG = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Health growth distribution", hpDistG);
                    spDistG = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Stamina growth distribution", spDistG);
                    strDistG = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Strength growth distribution", strDistG);
                    dexDistG = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Dexterity growth distribution", dexDistG);
                    vitDistG = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Vitality growth distribution", vitDistG);
                    agilDistG = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Agility growth distribution", agilDistG);
                    gutDistG = (STAT_DIST_AMOUNT)EditorGUILayout.EnumPopup("Gut growth distribution", gutDistG);


                    if (GUILayout.Button("Generate Stat distribution2"))
                    {
                        #region HP
                        switch (hpDist)
                        {
                            case STAT_DIST_AMOUNT.VERY_LOW:
                                data.maxHitPointsB = Random.Range(BSvlowBoundPTS.x, BSvlowBoundPTS.y);
                                break;
                            case STAT_DIST_AMOUNT.LOW:
                                data.maxHitPointsB = Random.Range(BSlowBoundPTS.x, BSlowBoundPTS.y);
                                break;
                            case STAT_DIST_AMOUNT.AVERAGE:
                                data.maxHitPointsB = Random.Range(BSavgBoundPTS.x, BSavgBoundPTS.y);
                                break;
                            case STAT_DIST_AMOUNT.HIGH:
                                data.maxHitPointsB = Random.Range(BShighBoundPTS.x, BShighBoundPTS.y);
                                break;
                            case STAT_DIST_AMOUNT.VERY_HIGH:
                                data.maxHitPointsB = Random.Range(BSvhighBoundPTS.x, BSvhighBoundPTS.y);
                                break;
                        }
                        switch (hpDistG)
                        {
                            case STAT_DIST_AMOUNT.VERY_LOW:
                                data.maxHitPointsGMin = Random.Range(LWvlowBoundPTS.x, LWvlowBoundPTS.y);
                                if (data.maxHitPointsGMin > HGvlowBoundPTS.x)
                                {
                                    data.maxHitPointsGMax = Random.Range(data.maxHitPointsGMin, HGvlowBoundPTS.y);
                                }
                                else
                                {
                                    data.maxHitPointsGMax = Random.Range(HGvlowBoundPTS.x, HGvlowBoundPTS.y);
                                }
                                break;

                            case STAT_DIST_AMOUNT.LOW:
                                data.maxHitPointsGMin = Random.Range(LWlowBoundPTS.x, LWlowBoundPTS.y);
                                if (data.maxHitPointsGMin > HGlowBoundPTS.x)
                                {
                                    data.maxHitPointsGMax = Random.Range(data.maxHitPointsGMin, HGlowBoundPTS.y);
                                }
                                else
                                {
                                    data.maxHitPointsGMax = Random.Range(HGlowBoundPTS.x, HGlowBoundPTS.y);
                                }
                                break;

                            case STAT_DIST_AMOUNT.AVERAGE:
                                data.maxHitPointsGMin = Random.Range(LWavgBoundPTS.x, LWavgBoundPTS.y);
                                if (data.maxHitPointsGMin > HGavgBoundPTS.x)
                                {
                                    data.maxHitPointsGMax = Random.Range(data.maxHitPointsGMin, HGavgBoundPTS.y);
                                }
                                else
                                {
                                    data.maxHitPointsGMax = Random.Range(HGavgBoundPTS.x, HGavgBoundPTS.y);
                                }
                                break;

                            case STAT_DIST_AMOUNT.HIGH:
                                data.maxHitPointsGMin = Random.Range(LWhighBoundPTS.x, LWhighBoundPTS.y);
                                if (data.maxHitPointsGMin > HGhighBoundPTS.x)
                                {
                                    data.maxHitPointsGMax = Random.Range(data.maxHitPointsGMin, HGhighBoundPTS.y);
                                }
                                else
                                {
                                    data.maxHitPointsGMax = Random.Range(HGhighBoundPTS.x, HGhighBoundPTS.y);
                                }
                                break;

                            case STAT_DIST_AMOUNT.VERY_HIGH:
                                data.maxHitPointsGMin = Random.Range(LWvhighBoundPTS.x, LWvhighBoundPTS.y);
                                if (data.maxHitPointsGMin > HGvhighBoundPTS.x)
                                {
                                    data.maxHitPointsGMax = Random.Range(data.maxHitPointsGMin, HGvhighBoundPTS.y);
                                }
                                else
                                {
                                    data.maxHitPointsGMax = Random.Range(HGvhighBoundPTS.x, HGvhighBoundPTS.y);
                                }
                                break;
                        }
                        #endregion

                        #region SP
                        switch (spDist)
                        {
                            case STAT_DIST_AMOUNT.VERY_LOW:
                                data.maxSkillPointsB = Random.Range(BSvlowBoundPTS.x, BSvlowBoundPTS.y);
                                break;
                            case STAT_DIST_AMOUNT.LOW:
                                data.maxSkillPointsB = Random.Range(BSlowBoundPTS.x, BSlowBoundPTS.y);
                                break;
                            case STAT_DIST_AMOUNT.AVERAGE:
                                data.maxSkillPointsB = Random.Range(BSavgBoundPTS.x, BSavgBoundPTS.y);
                                break;
                            case STAT_DIST_AMOUNT.HIGH:
                                data.maxSkillPointsB = Random.Range(BShighBoundPTS.x, BShighBoundPTS.y);
                                break;
                            case STAT_DIST_AMOUNT.VERY_HIGH:
                                data.maxSkillPointsB = Random.Range(BSvhighBoundPTS.x, BSvhighBoundPTS.y);
                                break;
                        }
                        switch (spDistG)
                        {
                            case STAT_DIST_AMOUNT.VERY_LOW:
                                data.maxSkillPointsGMin = Random.Range(LWvlowBoundPTS.x, LWvlowBoundPTS.y);
                                if (data.maxSkillPointsGMin > HGvlowBoundPTS.x)
                                {
                                    data.maxSkillPointsGMax = Random.Range(data.maxSkillPointsGMin, HGvlowBoundPTS.y);
                                }
                                else
                                {
                                    data.maxSkillPointsGMax = Random.Range(HGvlowBoundPTS.x, HGvlowBoundPTS.y);
                                }
                                break;

                            case STAT_DIST_AMOUNT.LOW:
                                data.maxSkillPointsGMin = Random.Range(LWlowBoundPTS.x, LWlowBoundPTS.y);
                                if (data.maxSkillPointsGMin > HGlowBoundPTS.x)
                                {
                                    data.maxSkillPointsGMax = Random.Range(data.maxHitPointsGMin, HGlowBoundPTS.y);
                                }
                                else
                                {
                                    data.maxSkillPointsGMax = Random.Range(HGlowBoundPTS.x, HGlowBoundPTS.y);
                                }
                                break;

                            case STAT_DIST_AMOUNT.AVERAGE:
                                data.maxSkillPointsGMin = Random.Range(LWavgBoundPTS.x, LWavgBoundPTS.y);
                                if (data.maxSkillPointsGMin > HGavgBoundPTS.x)
                                {
                                    data.maxSkillPointsGMax = Random.Range(data.maxSkillPointsGMin, HGavgBoundPTS.y);
                                }
                                else
                                {
                                    data.maxSkillPointsGMax = Random.Range(HGavgBoundPTS.x, HGavgBoundPTS.y);
                                }
                                break;

                            case STAT_DIST_AMOUNT.HIGH:
                                data.maxSkillPointsGMin = Random.Range(LWhighBoundPTS.x, LWhighBoundPTS.y);
                                if (data.maxSkillPointsGMin > HGhighBoundPTS.x)
                                {
                                    data.maxSkillPointsGMax = Random.Range(data.maxSkillPointsGMin, HGhighBoundPTS.y);
                                }
                                else
                                {
                                    data.maxSkillPointsGMax = Random.Range(HGhighBoundPTS.x, HGhighBoundPTS.y);
                                }
                                break;

                            case STAT_DIST_AMOUNT.VERY_HIGH:
                                data.maxSkillPointsGMin = Random.Range(LWvhighBoundPTS.x, LWvhighBoundPTS.y);
                                if (data.maxSkillPointsGMin > HGvhighBoundPTS.x)
                                {
                                    data.maxSkillPointsGMax = Random.Range(data.maxSkillPointsGMin, HGvhighBoundPTS.y);
                                }
                                else
                                {
                                    data.maxSkillPointsGMax = Random.Range(HGvhighBoundPTS.x, HGvhighBoundPTS.y);
                                }
                                break;
                        }
                        #endregion

                        #region STRENGTH
                        {
                            int statB = 0;
                            int statG = 0;
                            switch (strDist)
                            {
                                case STAT_DIST_AMOUNT.VERY_LOW:
                                    statB = Random.Range(vlowBoundB.x, vlowBoundB.y);
                                    statG = Random.Range(vlowBoundG.x, vlowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.LOW:
                                    statB = Random.Range(lowBoundB.x, lowBoundB.y);
                                    statG = Random.Range(lowBoundG.x, lowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.AVERAGE:
                                    statB = Random.Range(avgBoundB.x, avgBoundB.y);
                                    statG = Random.Range(avgBoundB.x, avgBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.HIGH:
                                    statB = Random.Range(highBoundB.x, highBoundB.y);
                                    statG = Random.Range(highBoundG.x, highBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.VERY_HIGH:
                                    statB = Random.Range(vhighBoundB.x, vhighBoundB.y);
                                    statG = Random.Range(vhighBoundG.x, vhighBoundG.y);
                                    break;
                            }
                            data.attackB = statB;
                            data.attackG = statG;
                        }
                        #endregion

                        #region VITALITY
                        {
                            int statB = 0;
                            int statG = 0;
                            switch (strDist)
                            {
                                case STAT_DIST_AMOUNT.VERY_LOW:
                                    statB = Random.Range(vlowBoundB.x, vlowBoundB.y);
                                    statG = Random.Range(vlowBoundG.x, vlowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.LOW:
                                    statB = Random.Range(lowBoundB.x, lowBoundB.y);
                                    statG = Random.Range(lowBoundG.x, lowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.AVERAGE:
                                    statB = Random.Range(avgBoundB.x, avgBoundB.y);
                                    statG = Random.Range(avgBoundB.x, avgBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.HIGH:
                                    statB = Random.Range(highBoundB.x, highBoundB.y);
                                    statG = Random.Range(highBoundG.x, highBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.VERY_HIGH:
                                    statB = Random.Range(vhighBoundB.x, vhighBoundB.y);
                                    statG = Random.Range(vhighBoundG.x, vhighBoundG.y);
                                    break;
                            }
                            data.defenceB = statB;
                            data.defenceG = statG;
                        }
                        #endregion

                        #region DEXTERITY
                        {
                            int statB = 0;
                            int statG = 0;
                            switch (strDist)
                            {
                                case STAT_DIST_AMOUNT.VERY_LOW:
                                    statB = Random.Range(vlowBoundB.x, vlowBoundB.y);
                                    statG = Random.Range(vlowBoundG.x, vlowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.LOW:
                                    statB = Random.Range(lowBoundB.x, lowBoundB.y);
                                    statG = Random.Range(lowBoundG.x, lowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.AVERAGE:
                                    statB = Random.Range(avgBoundB.x, avgBoundB.y);
                                    statG = Random.Range(avgBoundB.x, avgBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.HIGH:
                                    statB = Random.Range(highBoundB.x, highBoundB.y);
                                    statG = Random.Range(highBoundG.x, highBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.VERY_HIGH:
                                    statB = Random.Range(vhighBoundB.x, vhighBoundB.y);
                                    statG = Random.Range(vhighBoundG.x, vhighBoundG.y);
                                    break;
                            }
                            data.intelligenceB = statB;
                            data.intelligenceG = statG;
                        }
                        #endregion

                        #region AGILITY
                        {
                            int statB = 0;
                            int statG = 0;
                            switch (strDist)
                            {
                                case STAT_DIST_AMOUNT.VERY_LOW:
                                    statB = Random.Range(vlowBoundB.x, vlowBoundB.y);
                                    statG = Random.Range(vlowBoundG.x, vlowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.LOW:
                                    statB = Random.Range(lowBoundB.x, lowBoundB.y);
                                    statG = Random.Range(lowBoundG.x, lowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.AVERAGE:
                                    statB = Random.Range(avgBoundB.x, avgBoundB.y);
                                    statG = Random.Range(avgBoundB.x, avgBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.HIGH:
                                    statB = Random.Range(highBoundB.x, highBoundB.y);
                                    statG = Random.Range(highBoundG.x, highBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.VERY_HIGH:
                                    statB = Random.Range(vhighBoundB.x, vhighBoundB.y);
                                    statG = Random.Range(vhighBoundG.x, vhighBoundG.y);
                                    break;
                            }
                            data.speedB = statB;
                            data.speedG = statG;
                        }
                        #endregion

                        #region GUTS
                        {
                            int statB = 0;
                            int statG = 0;
                            switch (strDist)
                            {
                                case STAT_DIST_AMOUNT.VERY_LOW:
                                    statB = Random.Range(vlowBoundB.x, vlowBoundB.y);
                                    statG = Random.Range(vlowBoundG.x, vlowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.LOW:
                                    statB = Random.Range(lowBoundB.x, lowBoundB.y);
                                    statG = Random.Range(lowBoundG.x, lowBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.AVERAGE:
                                    statB = Random.Range(avgBoundB.x, avgBoundB.y);
                                    statG = Random.Range(avgBoundB.x, avgBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.HIGH:
                                    statB = Random.Range(highBoundB.x, highBoundB.y);
                                    statG = Random.Range(highBoundG.x, highBoundG.y);
                                    break;

                                case STAT_DIST_AMOUNT.VERY_HIGH:
                                    statB = Random.Range(vhighBoundB.x, vhighBoundB.y);
                                    statG = Random.Range(vhighBoundG.x, vhighBoundG.y);
                                    break;
                            }
                            data.gutsB = statB;
                            data.gutsG = statG;
                        }
                        #endregion
                    }
                    #endregion
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name: ");
                    data.name = EditorGUILayout.TextArea(data.name);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Base stats: ");
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Health: ");
                    data.maxHitPointsB = ChangeStat(ref data.maxHitPointsB);
                    EditorGUILayout.LabelField("Growth min: ");
                    data.maxHitPointsGMin = ChangeStat(ref data.maxHitPointsGMin);
                    EditorGUILayout.LabelField("Growth max: ");
                    data.maxHitPointsGMax = ChangeStat(ref data.maxHitPointsGMax);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Stamina: ");
                    data.maxSkillPointsB = EditorGUILayout.IntField(data.maxSkillPointsB);
                    EditorGUILayout.LabelField("Growth min: ");
                    data.maxSkillPointsGMin = EditorGUILayout.IntField(data.maxSkillPointsGMin);
                    EditorGUILayout.LabelField("Growth max: ");
                    data.maxSkillPointsGMax = EditorGUILayout.IntField(data.maxSkillPointsGMax);

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Strength: ");
                    data.attackB = EditorGUILayout.IntField(data.attackB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.attackG = EditorGUILayout.IntField(data.attackG);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Vitality: ");
                    data.defenceB = EditorGUILayout.IntField(data.defenceB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.defenceG = EditorGUILayout.IntField(data.defenceG);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Dexterity: ");
                    data.intelligenceB = EditorGUILayout.IntField(data.intelligenceB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.intelligenceG = EditorGUILayout.IntField(data.intelligenceG);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Agility: ");
                    data.speedB = EditorGUILayout.IntField(data.speedB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.speedG = EditorGUILayout.IntField(data.speedG);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Guts: ");
                    data.gutsB = EditorGUILayout.IntField(data.gutsB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.gutsG = EditorGUILayout.IntField(data.gutsG);
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Luck: ");
                    data.luckB = EditorGUILayout.IntField(data.luckB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.luckG = EditorGUILayout.IntField(data.luckG);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Base experience: ");
                    data.baseExpYeild = EditorGUILayout.FloatField(data.baseExpYeild);
                    EditorGUILayout.Space();
                    listArray = EditorGUILayout.IntField(listArray);
                    if (GUILayout.Button("New animation Array"))
                    {
                        data.characterAnims = new Sprite[listArray];
                    }
                    if (data.characterAnims != null)
                    {
                        for (int i = 0; i < data.characterAnims.Length; i++)
                        {
                            data.characterAnims[i] = EditorGUILayout.ObjectField(data.characterAnims[i],typeof(Sprite) , false) as Sprite;
                        }
                    }
                    break;
                #endregion

                #region MOVES
                case 2:

                    if (data.moveDatabase == null)
                    {
                        data.moveDatabase = new System.Collections.Generic.List<s_move>();
                    }
                    else
                    {
                        for (int i = 0; i < data.moveDatabase.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            data.moveDatabase[i] = EditorGUILayout.ObjectField(data.moveDatabase[i], typeof(s_move), false) as s_move;
                            EditorGUILayout.LabelField(" learn at level: ");
                            if (GUILayout.Button("Delete"))
                            {
                                data.moveDatabase.RemoveAt(i);
                                //To reset the whole thing
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button("Add new Move"))
                        {
                            data.moveDatabase.Add(new s_move());
                        }
                    }
                    break;
                #endregion

                #region ELEMENTS
                case 3:

                    elementSliderSelector = (ELEMENT)EditorGUILayout.EnumPopup(elementSliderSelector);

                    EditorGUILayout.BeginHorizontal();
                    switch (elementSliderSelector)
                    {
                        case ELEMENT.NORMAL:
                            element = "Strike";
                            break;
                        case ELEMENT.PEIRCE:
                            element = "Peirce";
                            break;
                        case ELEMENT.FORCE:
                            element = "Force";
                            break;
                        case ELEMENT.FIRE:
                            element = "Fire";
                            break;
                        case ELEMENT.ICE:
                            element = "Ice";
                            break;
                        case ELEMENT.WIND:
                            element = "Wind";
                            break;
                        case ELEMENT.ELECTRIC:
                            element = "Electirc";
                            break;
                        case ELEMENT.EARTH:
                            element = "Earth";
                            break;
                        case ELEMENT.POISON:
                            element = "Poison";
                            break;
                    }
                    if (elementSliderSelector != ELEMENT.UNKNOWN)
                    {
                        EditorGUILayout.LabelField(element + ": ");
                        data.elementTypeCharts[(int)elementSliderSelector] = ChangeStatFloatSlider(ref data.elementTypeCharts[(int)elementSliderSelector], -1.9f, 2.9f);
                        EditorGUILayout.EndHorizontal();
                    }
                    for (int i = 0; i < 13; i++)
                    {
                        ELEMENT elemen = (ELEMENT)i;
                        string str = "";

                        switch (elemen)
                        {

                            case ELEMENT.NORMAL:
                                str = "Strike";
                                break;
                            case ELEMENT.PEIRCE:
                                str = "Peirce";
                                break;
                            case ELEMENT.PSYCHIC:
                                str = "Psychic";
                                break;
                            case ELEMENT.LIGHT:
                                str = "Light";
                                break;
                            case ELEMENT.DARK:
                                str = "Dark";
                                break;
                            case ELEMENT.FORCE:
                                str = "Force";
                                break;
                            case ELEMENT.FIRE:
                                str = "Fire";
                                break;
                            case ELEMENT.ICE:
                                str = "Ice";
                                break;
                            case ELEMENT.WIND:
                                str = "Wind";
                                break;
                            case ELEMENT.ELECTRIC:
                                str = "Electirc";
                                break;
                            case ELEMENT.EARTH:
                                str = "Earth";
                                break;
                            case ELEMENT.POISON:
                                str = "Poison";
                                break;
                        }

                        ///NOTE THAT 
                        ///-0.000001 -> -1 IS REFLECT
                        ///-1.000001 -> -2 IS ABSORB
                        ///THEY ARE CALCULATED BASED ON THEIR .0 POINTS
                        ///THE FULL NUMBERS JUST TELL WHAT TYPE IT IS

                        if (data.elementTypeCharts[i] == 0)
                            EditorGUILayout.LabelField(str + ": " + data.elementTypeCharts[i] + " Immune");
                        else if (data.elementTypeCharts[i] > 0 && data.elementTypeCharts[i] < 1)
                            EditorGUILayout.LabelField(str + ": " + data.elementTypeCharts[i] + " Resistant");
                        else if (data.elementTypeCharts[i] >= 1 && data.elementTypeCharts[i] < 2)
                            EditorGUILayout.LabelField(str + ": " + data.elementTypeCharts[i] + "");
                        else if (data.elementTypeCharts[i] >= 2 && data.elementTypeCharts[i] < 3)
                            EditorGUILayout.LabelField(str + ": " + data.elementTypeCharts[i] + " Weak");
                        else if (data.elementTypeCharts[i] < 0 && data.elementTypeCharts[i] > -1)
                            EditorGUILayout.LabelField(str + ": " + data.elementTypeCharts[i] + " Reflect");
                        else if (data.elementTypeCharts[i] <= 2 && data.elementTypeCharts[i] > -3)
                            EditorGUILayout.LabelField(str + ": " + data.elementTypeCharts[i] + " Absorb");

                    }
                    EditorGUILayout.Space();

                    actionSliderSelector = (ACTION_TYPE)EditorGUILayout.EnumPopup(actionSliderSelector);
                    if (actionSliderSelector != ACTION_TYPE.NONE)
                    {
                        EditorGUILayout.BeginHorizontal();
                        switch (actionSliderSelector)
                        {
                            case ACTION_TYPE.COMFORT:
                                element = "Comfort";
                                break;

                            case ACTION_TYPE.FLIRT:
                                element = "Flirt";
                                break;

                            case ACTION_TYPE.PLAYFUL:
                                element = "Playful";
                                break;

                            case ACTION_TYPE.THREAT:
                                element = "Threat";
                                break;

                            case ACTION_TYPE.RESERVED:
                                element = "Reserved";
                                break;

                        }
                        EditorGUILayout.LabelField(element + ": ");
                        data.actionTypeCharts[(int)actionSliderSelector] = ChangeStatFloatSlider(ref data.actionTypeCharts[(int)actionSliderSelector], -1.9f, 2.9f);
                        EditorGUILayout.EndHorizontal();
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        ACTION_TYPE elemen = (ACTION_TYPE)i;
                        string str = "";

                        switch (elemen)
                        {

                            case ACTION_TYPE.COMFORT:
                                str = "Comfort";
                                break;

                            case ACTION_TYPE.FLIRT:
                                str = "Flirt";
                                break;

                            case ACTION_TYPE.RESERVED:
                                str = "Reserved";
                                break;

                            case ACTION_TYPE.PLAYFUL:
                                str = "Playful";
                                break;

                            case ACTION_TYPE.THREAT:
                                str = "Threat";
                                break;
                        }

                        ///NOTE THAT 
                        ///-0.000001 -> -1 IS REFLECT
                        ///-1.000001 -> -2 IS ABSORB
                        ///THEY ARE CALCULATED BASED ON THEIR .0 POINTS
                        ///THE FULL NUMBERS JUST TELL WHAT TYPE IT IS

                        if (data.actionTypeCharts[i] == 0)
                            EditorGUILayout.LabelField(str + ": " + data.actionTypeCharts[i] + " Immune");
                        else if (data.actionTypeCharts[i] > 0 && data.actionTypeCharts[i] < 1)
                            EditorGUILayout.LabelField(str + ": " + data.actionTypeCharts[i] + " Resistant");
                        else if (data.actionTypeCharts[i] >= 1 && data.actionTypeCharts[i] < 2)
                            EditorGUILayout.LabelField(str + ": " + data.actionTypeCharts[i] + "");
                        else if (data.actionTypeCharts[i] >= 2 && data.actionTypeCharts[i] < 3)
                            EditorGUILayout.LabelField(str + ": " + data.actionTypeCharts[i] + " Weak");
                        else if (data.actionTypeCharts[i] < 0 && data.actionTypeCharts[i] > -1)
                            EditorGUILayout.LabelField(str + ": " + data.actionTypeCharts[i] + " Reflect");
                        else if (data.actionTypeCharts[i] <= 2 && data.actionTypeCharts[i] > -3)
                            EditorGUILayout.LabelField(str + ": " + data.actionTypeCharts[i] + " Absorb");

                    }
                    break;
                #endregion

                #region AI
                case 4:
                    //o_battleCharData bo = new o_battleCharData();
                    //bo.characterAI[];
                    if (data.characterAI != null)
                    {
                        //data.characterAI = EditorGUILayout.PropertyField(data.characterAI);
                        foreach (charAI ai in data.characterAI)
                        {

                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Important");
                            ai.isImportant = EditorGUILayout.Toggle(ai.isImportant);
                            ai.conditions = (charAI.CONDITIONS)EditorGUILayout.EnumPopup(ai.conditions);
                            switch (ai.conditions)
                            {

                                case charAI.CONDITIONS.ALWAYS:
                                    if (ai.moveName == null)
                                        EditorGUILayout.LabelField("Always use [NO MOVE SELECTED]");
                                    else
                                        EditorGUILayout.LabelField("Always use " + ai.moveName.name);

                                    for (int i = 0; i < data.moveDatabase.Count; i++)
                                    {
                                        if (data.moveDatabase[i] != null)
                                        {
                                            if (GUILayout.Button(data.moveDatabase[i].name))
                                            {
                                                ai.moveName = data.moveDatabase[i];
                                            }
                                        }
                                    }
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                                case charAI.CONDITIONS.USER_PARTY_HP_LOWER:
                                    EditorGUILayout.LabelField("If party member's health is lower than " + ai.healthPercentage * 100 + "%, use ");
                                    ai.moveName = EditorGUILayout.ObjectField(ai.moveName, typeof(s_move), false) as s_move;
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                                case charAI.CONDITIONS.TARGET_PARTY_HP_HIGHER:
                                    EditorGUILayout.LabelField("If target's health is higher than " + ai.healthPercentage * 100 + "%, use ");
                                    ai.moveName = EditorGUILayout.ObjectField(ai.moveName, typeof(s_move), false) as s_move;
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                                case charAI.CONDITIONS.USER_PARTY_HP_HIGHER:
                                    EditorGUILayout.LabelField("If party member's health is higher than " + ai.healthPercentage * 100 + "%, use ");
                                    ai.moveName = EditorGUILayout.ObjectField(ai.moveName, typeof(s_move), false) as s_move;
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                                case charAI.CONDITIONS.TARGET_PARTY_HP_LOWER:
                                    EditorGUILayout.LabelField("If target's health is lower than " + ai.healthPercentage * 100 + "%, use ");

                                    ai.moveName = EditorGUILayout.ObjectField(ai.moveName, typeof(s_move), false) as s_move;
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;

                                case charAI.CONDITIONS.SELF_HP_HIGHER:
                                    EditorGUILayout.LabelField("If the user's health is higher than " + ai.healthPercentage * 100 + "%, use ");

                                    ai.moveName = EditorGUILayout.ObjectField(ai.moveName, typeof(s_move), false) as s_move;
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;

                                case charAI.CONDITIONS.SELF_SP_LOWER:
                                    EditorGUILayout.LabelField("If the user's stamina is higher than " + ai.healthPercentage * 100 + "%, use ");

                                    ai.moveName = EditorGUILayout.ObjectField(ai.moveName, typeof(s_move), false) as s_move;
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;

                                case charAI.CONDITIONS.SELF_HP_LOWER:
                                    EditorGUILayout.LabelField("If the user's health is higher than " + ai.healthPercentage * 100 + "%, use ");

                                    ai.moveName = EditorGUILayout.ObjectField(ai.moveName, typeof(s_move), false) as s_move;
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;

                                case charAI.CONDITIONS.SELF_SP_HIGHER:
                                    EditorGUILayout.LabelField("If the user's stamina is higher than " + ai.healthPercentage * 100 + "%, use ");

                                    ai.moveName = EditorGUILayout.ObjectField(ai.moveName, typeof(s_move), false) as s_move;
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                            }
                            EditorGUILayout.LabelField("Always use [NO MOVE SELECTED]");
                            ai.turnCounters = (charAI.TURN_COUNTER)EditorGUILayout.EnumPopup(ai.turnCounters);
                            switch (ai.turnCounters) {
                                case charAI.TURN_COUNTER.TURN_COUNTER_EQU:
                                    EditorGUILayout.LabelField("Turn counter");
                                    ai.number1 = EditorGUILayout.IntField(ai.number1);
                                    break;

                                case charAI.TURN_COUNTER.ROUND_COUNTER_EQU:
                                    EditorGUILayout.LabelField("Round counter");
                                    ai.number2 = EditorGUILayout.IntField(ai.number2);
                                    break;

                                case charAI.TURN_COUNTER.ROUND_TURN_COUNTER_EQU:
                                    EditorGUILayout.LabelField("Turn counter");
                                    ai.number1 = EditorGUILayout.IntField(ai.number1);
                                    EditorGUILayout.Space();
                                    EditorGUILayout.LabelField("Round counter");
                                    ai.number2 = EditorGUILayout.IntField(ai.number2);
                                    break;
                            }
                            EditorGUILayout.Space();
                        }
                    }
                    break;
                #endregion

                case 5:
                    base.OnInspectorGUI();
                    break;
            }


            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();
    }
}