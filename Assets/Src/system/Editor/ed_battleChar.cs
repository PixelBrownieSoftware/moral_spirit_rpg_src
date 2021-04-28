using System.IO;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

/*
public class ed_battleChar : EditorWindow
{
    //Character Action Stats Editor
    [MenuItem("Brownie/CASE")]
    static void init()
    {
        GetWindow<ed_battleChar>("CASE");
    }

    List<charAI> characterAI;
    Sprite[] sprites;
    int listArray;
    int tab = 0;
    int charAILeng = 0;
    o_battleCharData data = null;
    ELEMENT elementSliderSelector = ELEMENT.NORMAL;
    ACTION_TYPE actionSliderSelector = ACTION_TYPE.NONE;
    bool isLoadedCharacter = false;
    string directoryMove;
    string element;

    private void OnGUI()
    {
        if (GUILayout.Button("Load Character"))
        {
            directoryMove = EditorUtility.OpenFilePanel("Save Json move file", "Assets/Data/Characters/", "");
            if (directoryMove != "") {
                string fil = File.ReadAllText(directoryMove);
                if (fil != null) {
                    data = JsonUtility.FromJson<o_battleCharData>(fil);
                    characterAI = data.characterAI.ToList();
                    isLoadedCharacter = true;
                }
            }
        }
        if (GUILayout.Button("New Character"))
        {
            data = new o_battleCharData();
        }
        if (GUILayout.Button("Clear Character"))
        {
            data = null;
        }
        if (data != null)
        {
            tab = GUILayout.Toolbar(tab ,new string[] { "Overview", "Stats", "Moves", "Elements", "AI" });
            switch (tab)
            {
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
                            if (i % data.luckG == 0)
                                tempLuc++;

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

                case 1:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name: ");
                    data.name = EditorGUILayout.TextArea(data.name);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Base stats: ");
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Health: ");
                    data.maxHitPointsB = EditorGUILayout.IntField(data.maxHitPointsB);
                    EditorGUILayout.LabelField("Growth min: ");
                    data.maxHitPointsGMin = EditorGUILayout.IntField(data.maxHitPointsGMin);
                    EditorGUILayout.LabelField("Growth max: ");
                    data.maxHitPointsGMax = EditorGUILayout.IntField(data.maxHitPointsGMax);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stamina: ");
                    data.maxSkillPointsB = EditorGUILayout.IntField(data.maxSkillPointsB);
                    EditorGUILayout.LabelField("Growth min: ");
                    data.maxSkillPointsGMin = EditorGUILayout.IntField(data.maxSkillPointsGMin);
                    EditorGUILayout.LabelField("Growth max: ");
                    data.maxSkillPointsGMax = EditorGUILayout.IntField(data.maxSkillPointsGMax);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Strength: ");
                    data.attackB = EditorGUILayout.IntField(data.attackB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.attackG = EditorGUILayout.IntField(data.attackG);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Vitality: ");
                    data.defenceB = EditorGUILayout.IntField(data.defenceB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.defenceG = EditorGUILayout.IntField(data.defenceG);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Dexterity: ");
                    data.intelligenceB = EditorGUILayout.IntField(data.intelligenceB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.intelligenceG = EditorGUILayout.IntField(data.intelligenceG);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Agility: ");
                    data.speedB = EditorGUILayout.IntField(data.speedB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.speedG = EditorGUILayout.IntField(data.speedG);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Guts: ");
                    data.gutsB = EditorGUILayout.IntField(data.gutsB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.gutsG = EditorGUILayout.IntField(data.gutsG);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Luck: ");
                    data.luckB = EditorGUILayout.IntField(data.luckB);
                    EditorGUILayout.LabelField("Growth turns: ");
                    data.luckG = EditorGUILayout.IntField(data.luckG);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Base experience: ");
                    data.baseExpYeild = EditorGUILayout.IntField(data.baseExpYeild);
                    EditorGUILayout.Space();
                    listArray = EditorGUILayout.IntField(listArray);
                    if (GUILayout.Button("New animation Array"))
                    {
                        data.characterAnims = new string[listArray];
                    }
                    if (data.characterAnims != null)
                    {
                        for (int i = 0; i < data.characterAnims.Length; i++)
                        {
                            data.characterAnims[i] = EditorGUILayout.TextField(data.characterAnims[i]);
                        }
                    }
                    break;

                case 2:

                    if (data.moveDatabase == null) {
                        data.moveDatabase = new System.Collections.Generic.List<o_battleChar.move_learn>();
                    } else{

                        for (int i = 0; i < data.moveDatabase.Count; i++) {
                            EditorGUILayout.BeginHorizontal();
                            data.moveDatabase[i].moveName = EditorGUILayout.TextField(data.moveDatabase[i].moveName);
                            EditorGUILayout.LabelField(" learn at level: ");
                            data.moveDatabase[i].level = EditorGUILayout.IntField(data.moveDatabase[i].level);
                            if (GUILayout.Button("Delete"))
                            {
                                data.moveDatabase.RemoveAt(i);
                                //To reset the whole thing
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button("Add new Move")) {
                            data.moveDatabase.Add(new o_battleChar.move_learn(0, "New move"));
                        }
                    }
                    break;

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
                        data.elementTypeCharts[(int)elementSliderSelector] = EditorGUILayout.Slider(data.elementTypeCharts[(int)elementSliderSelector], -1.9f, 2.9f);
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
                            case ELEMENT.WATER:
                                str = "Water";
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
                    switch (actionSliderSelector)
                    {
                        case ACTION_TYPE.COMFORT:
                            element = "Comfort";
                            break;

                        case ACTION_TYPE.FLIRT:
                            element = "Flirt";
                            break;

                        case ACTION_TYPE.INTELLECT:
                            element = "Intellect";
                            break;

                        case ACTION_TYPE.PLAYFUL:
                            element = "Playful";
                            break;

                        case ACTION_TYPE.THREAT:
                            element = "Threat";
                            break;

                    }
                    if (actionSliderSelector != ACTION_TYPE.NONE)
                    {
                        EditorGUILayout.LabelField(element + ": ");
                        data.actionTypeCharts[(int)actionSliderSelector] = EditorGUILayout.Slider(data.actionTypeCharts[(int)actionSliderSelector], -1.9f, 2.9f);
                        EditorGUILayout.EndHorizontal();
                    }
                    for (int i = 0; i < 5; i++)
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

                            case ACTION_TYPE.INTELLECT:
                                str = "Intellect";
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

                case 4:
                    //o_battleCharData bo = new o_battleCharData();
                    //bo.characterAI[];
                    if (characterAI != null)
                    {
                        foreach (charAI ai in characterAI) {

                            EditorGUILayout.BeginHorizontal();
                            ai.conditions = (charAI.CONDITIONS)EditorGUILayout.EnumPopup(ai.conditions);
                            switch (ai.conditions)
                            {

                                case charAI.CONDITIONS.ALWAYS:
                                    EditorGUILayout.LabelField("Always use ");
                                    ai.moveName = EditorGUILayout.TextField(ai.moveName);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                                case charAI.CONDITIONS.USER_PARTY_HP_LOWER:
                                    EditorGUILayout.LabelField("If party member's health is lower than " + ai.healthPercentage * 100 + "%, use ");
                                    ai.moveName = EditorGUILayout.TextField(ai.moveName);
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                                case charAI.CONDITIONS.TARGET_PARTY_HP_HIGHER:
                                    EditorGUILayout.LabelField("If target's health is higher than " + ai.healthPercentage * 100 + "%, use ");
                                    ai.moveName = EditorGUILayout.TextField(ai.moveName);
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                                case charAI.CONDITIONS.USER_PARTY_HP_HIGHER:
                                    EditorGUILayout.LabelField("If party member's health is higher than " + ai.healthPercentage * 100 + "%, use ");
                                    ai.moveName = EditorGUILayout.TextField(ai.moveName);
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                                case charAI.CONDITIONS.TARGET_PARTY_HP_LOWER:
                                    EditorGUILayout.LabelField("If target's health is lower than " + ai.healthPercentage * 100 + "%, use ");
                                    ai.moveName = EditorGUILayout.TextField(ai.moveName);
                                    ai.healthPercentage = EditorGUILayout.Slider(ai.healthPercentage, 0, 1);
                                    EditorGUILayout.LabelField("On Party member?");
                                    ai.onParty = EditorGUILayout.Toggle(ai.onParty);
                                    break;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button("Add new AI action"))
                        {
                            characterAI.Add(new charAI());
                        }
                    }
                    else
                    {
                        characterAI = new List<charAI>();
                    }
                    break;
            }


            EditorGUILayout.Space();




            if (GUILayout.Button("Save Character As"))
            {
                directoryMove = EditorUtility.SaveFilePanel("Save Json move file", "Assets/Data/Characters/", data.name, "txt");
                if (directoryMove != null)
                {
                    data.characterAI = characterAI.ToArray();
                    string json = JsonUtility.ToJson(data, true);
                    File.WriteAllText(directoryMove, json);
                }
            }
            if (isLoadedCharacter) {
            } else {
            }
        }
    }
}
*/
