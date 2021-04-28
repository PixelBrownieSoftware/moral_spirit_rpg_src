using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using MagnumFoundation2.System;
using MagnumFoundation2.System.Core;
using MagnumFoundation2.Objects;

[System.Serializable]
public class RPG_save : dat_save
{
    public o_battleCharSaveData[] partyMembers;

    [System.Serializable]
    public struct sav_item {
        public string itemName;
        public int amount;
        public sav_item(string itemName, int amount) {
            this.itemName = itemName;
            this.amount = amount;
        }
    }
    public sav_item[] savedItems;
    
    public RPG_save(dat_globalflags gbflg, int health, int MAXhp, string currentmap, Vector2 location, List<o_battleCharData> partyMembers, List<triggerState> trigStates)
        : base(gbflg, health, MAXhp, currentmap, location, trigStates)
    {
        List<o_battleCharSaveData> pMembers = new List<o_battleCharSaveData>();
        int i = 0;

        foreach (o_battleCharData da in partyMembers) {
            o_battleCharSaveData pm = new o_battleCharSaveData();
            pm.level = da.level;

            pm.attack = da.attack;
            pm.defence = da.defence;
            pm.intelligence = da.intelligence;
            pm.guts = da.guts;
            pm.luck = da.luck;
            pm.speed = da.speed;

            pm.level = da.level;
            pm.exp = da.exp;

            pm.hitPoints = da.hitPoints;
            pm.maxHitPoints = da.maxHitPoints;

            pm.skillPoints = da.skillPoints;
            pm.maxSkillPoints = da.maxSkillPoints;

            pm.dataSrc = da.dataSrc.name;

            foreach (s_move m in da.currentMoves) {
                pm.currentMoves.Add(m.name);
            }

            pMembers.Add(pm);
            i++;
        }
        this.partyMembers = pMembers.ToArray();

        Dictionary<string, int> inv = rpg_globals.gl.inventory;
        List<sav_item> its = new List<sav_item>();
        foreach (KeyValuePair<string, int> it in inv)
        {
            its.Add(new sav_item(it.Key, it.Value));
        }
        savedItems = its.ToArray();
    }
    public RPG_save(List<o_battleCharData> partyMembers)
    {
        List<o_battleCharSaveData> pMembers = new List<o_battleCharSaveData>();
        int i = 0;

        foreach (o_battleCharData da in partyMembers)
        {
            o_battleCharSaveData pm = new o_battleCharSaveData();
            pm.level = da.level;
            pm.name = da.name;

            pm.attack = da.attack;
            pm.defence = da.defence;
            pm.intelligence = da.intelligence;
            pm.guts = da.guts;
            pm.luck = da.luck;
            pm.speed = da.speed;

            pm.level = da.level;
            pm.exp = da.exp;

            pm.hitPoints = da.hitPoints;
            pm.maxHitPoints = da.maxHitPoints;

            pm.skillPoints = da.skillPoints;
            pm.maxSkillPoints = da.maxSkillPoints;

            pm.dataSrc = da.dataSrc.name;

            foreach (s_move m in da.currentMoves)
            {
                pm.currentMoves.Add(m.name);
            }

            pMembers.Add(pm);
            Debug.Log(pm.name);
            i++;
        }
        this.partyMembers = pMembers.ToArray();

        Dictionary<string, int> inv = rpg_globals.gl.inventory;
        List<sav_item> its = new List<sav_item>();
        foreach (KeyValuePair<string, int> it in inv)
        {
            its.Add(new sav_item(it.Key, it.Value));
        }
        savedItems = its.ToArray();
        Debug.Log(this.partyMembers.Length);
    }
    public RPG_save()
    {
    }
}


/// <summary>
/// To store the game's battle data like enemies
/// </summary>

public class rpg_globals : s_globals
{
    public static float money;
    public Image fadeFX;
    public Image overworldMenuBox;
    public Dictionary<string, int> inventory;
    public List<rpg_item> inventoryData = new List<rpg_item>();
    public rpg_item[] allItems;
    public Text characterStats;

    public enum RPG_STATE
    {
        OVERWORLD,
        BATTLE
    }
    s_battlesyst battleSystem;
    public int menuChoice;
    public RPG_STATE GameState;
    public GameObject battleScene;
    public GameObject overworldScene;
    public static rpg_globals gl;
    public c_enemy battleStarter;   //Character to despawn once the battle has been intialized;

    public Text shopText;
    public Image shopBox;

    public s_guiList strStat;
    public s_guiList vitStat;
    public s_guiList intStat;
    public s_guiList gutStat;
    public s_guiList agStat;
    public s_guiList luStat;

    public Slider characterMenEXP;
    public Slider characterMenHP;
    public Slider characterMenSP;
    public Text characterMenName;
    public Text characterMenHPTxt;
    public Text characterMenSPTxt;
    public Text characterMenEXPTxt;

    public GameObject statsMenu;
    public GameObject itemsMenu;

    public Image StatusButton;
    public Image ItemsButton;

    public Text pressTextMenu;

    public GameObject[] menuItemButtons;
    public s_battlesyst.menu_button[] menuItemButtonsSelector;

    public enum RPG_MENU_STATE
    {
        OFF,
        MENU,
        STATUS,
        SKILLS,
        ITEMS,
        HEAL_TARGET
    }
    RPG_MENU_STATE MenuState = RPG_MENU_STATE.OFF;
    bool isItem = false;

    public o_battleCharData healTarg;
    /// This stores all the data of the party members
    /// To be added into battles. This data is saved in.
    public List<o_battleCharData> partyMembers = new List<o_battleCharData>();
    public List<o_battleCharData> enemyDatabase = new List<o_battleCharData>();
    
    public o_battleChar[] enemySlots;
    public o_battleChar[] playerSlots;

    //This will increase as you go along in the game
    public int extraSkillAmount = 3;
    //The skills you learn from enemies which you can equip on to your party members
    public List<s_move> extraSkills = new List<s_move>();

    //All the saved move databases
    public List<s_move> moveDatabase = new List<s_move>();
    //All the saved character databases

    //The move that is being edited
    //[HideInInspector]
    public s_move currentMove;
    //The enemy group that is being edited
    public enemy_group currentGroup;
    public string eventLabelName;
    public BattleCharacterData firstCharacter;
    s_move healMove = null;
    rpg_item healItem = null;
    o_battleCharData currentPartyMemberSelection = null;

    public s_guiHP targetHealer;
    public s_guiHP userHealer;
    public bool menuAble = true;

    public GameObject promptObj;
    public Text promptTextObj;
    public List<BattleCharacterData> dataCharacters;
    public u_encounter enc;

    public override void LoadSaveData()
    {
    }
    public override void StartStuff()
    {
        base.StartStuff();

        if (s_mainmenu.isload)
        {
            RPG_save sav = (RPG_save)s_mainmenu.save;
            foreach (o_battleCharSaveData s in sav.partyMembers)
            {
                AddMemeber(s);
            }
            if (sav.savedItems != null)
            {
                foreach (RPG_save.sav_item it in sav.savedItems)
                {
                    AddItem(it.itemName, it.amount);
                }
            }
        }
        else
        {
            ///Add in evan
            AddMemeber(firstCharacter, 1);
        }
        s_menuhandler.GetInstance().SwitchMenu("OpenMenu");
    }

    new void Awake()
    {
        if (gl == null)
            gl = this;

        battleSystem = battleScene.GetComponent<s_battlesyst>();
        base.Awake();
        AddKeys();
        inventory = new Dictionary<string, int>();
        /*
        foreach (TextAsset ta in moveDatabaseJson)
        {
            moveDatabase.Add(JsonUtility.FromJson<s_move>(ta.text));
        }
        foreach (TextAsset ta in characterDatabaseJson)
        {
            o_battleCharData g = JsonUtility.FromJson<o_battleCharData>(ta.text);
            characterDatabase2.Add(g);
        }
        */
        menuItemButtonsSelector = new s_battlesyst.menu_button[menuItemButtons.Length];
        for (int i = 0; i < menuItemButtons.Length; i++)
        {
            GameObject o = menuItemButtons[i];
            menuItemButtonsSelector[i] = new s_battlesyst.menu_button();
            menuItemButtonsSelector[i].selector = o.transform.GetChild(0).GetComponent<Image>();
            menuItemButtonsSelector[i].img = o.transform.GetChild(1).GetComponent<Image>();
            menuItemButtonsSelector[i].txt = o.transform.GetChild(2).GetComponent<Text>();
        }
        //AddMemeber("Sebastian", 5);
    }

    public void ShowPrompt() {
        promptTextObj.text = GetKeyPref("select").ToString() + " to open.";
        promptObj.gameObject.SetActive(true);
    }
    public void HidePrompt()
    {
        promptObj.gameObject.SetActive(false);
    }

    public rpg_item GetItem(string n)
    {
        foreach (rpg_item it in inventoryData)
        {
            if (n == it.name)
                return it;
        }
        return null;
    }

    /*
    public List<o_battleCharData> GetCharacterData()
    {
        List<o_battleCharData> chd = new List<o_battleCharData>();
        foreach (TextAsset ta in characterDatabaseJson)
        {
            o_battleCharData g = JsonUtility.FromJson<o_battleCharData>(ta.text);
            chd.Add(g);
        }
        return chd;
    }
    */

    public void AddMemeber(BattleCharacterData data, int level)
    {
        o_battleCharData newCharacter = new o_battleCharData();
        {
            newCharacter.level = level;
            newCharacter.name = data.name;
            int tempHP = data.maxHitPointsB;
            int tempSP = data.maxSkillPointsB;
            int tempStr = data.attackB;
            int tempVit = data.defenceB;
            int tempDx = data.intelligenceB;
            int tempAg = data.speedB;
            int tempGut = data.gutsB;
            int tempLuc = data.luckB;

            newCharacter.inBattle = true;

            newCharacter.currentMoves = new List<s_move>();

            foreach (o_battleChar.move_learn mov in data.moveDatabase)
            {
                if (mov.level <= level)
                    newCharacter.currentMoves.Add(mov.move);
            }

            for (int i = 1; i < level; i++)
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

                tempHP += UnityEngine.Random.Range(data.maxHitPointsGMin, data.maxHitPointsGMax);
                tempSP += UnityEngine.Random.Range(data.maxSkillPointsGMin, data.maxSkillPointsGMax);
            }
            newCharacter.hitPoints = newCharacter.maxHitPoints = tempHP;
            newCharacter.skillPoints = newCharacter.maxSkillPoints = tempSP;

            newCharacter.attack = tempStr;
            newCharacter.defence = tempVit;
            newCharacter.intelligence = tempDx;
            newCharacter.speed = tempAg;
            newCharacter.guts = tempGut;
            newCharacter.luck = tempLuc;
        }
        newCharacter.dataSrc = data;
        partyMembers.Add(newCharacter);
    }

    public void AddMemeber(o_battleCharSaveData saveDat)
    {
        print(saveDat.name);
        BattleCharacterData data = dataCharacters.Find(x => x.name == saveDat.name);
        o_battleCharData newCharacter = new o_battleCharData();
        {
            newCharacter.level = saveDat.level;
            newCharacter.name = data.name;
            int tempHP = data.maxHitPointsB;
            int tempSP = data.maxSkillPointsB;
            int tempStr = data.attackB;
            int tempVit = data.defenceB;
            int tempDx = data.intelligenceB;
            int tempAg = data.speedB;
            int tempGut = data.gutsB;
            int tempLuc = data.luckB;

            newCharacter.inBattle = true;

            newCharacter.currentMoves = new List<s_move>();

            foreach (o_battleChar.move_learn mov in data.moveDatabase)
            {
                if (mov.level <= saveDat.level)
                    newCharacter.currentMoves.Add(mov.move);
            }

            for (int i = 1; i < saveDat.level; i++)
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

                tempHP += UnityEngine.Random.Range(data.maxHitPointsGMin, data.maxHitPointsGMax);
                tempSP += UnityEngine.Random.Range(data.maxSkillPointsGMin, data.maxSkillPointsGMax);
            }
            newCharacter.maxHitPoints = tempHP;
            newCharacter.maxSkillPoints = tempSP;
            newCharacter.hitPoints = saveDat.hitPoints;
            newCharacter.skillPoints = saveDat.skillPoints;

            newCharacter.attack = tempStr;
            newCharacter.defence = tempVit;
            newCharacter.intelligence = tempDx;
            newCharacter.speed = tempAg;
            newCharacter.guts = tempGut;
            newCharacter.luck = tempLuc;
        }
        newCharacter.dataSrc = data;
        partyMembers.Add(newCharacter);
    }
    public IEnumerator Fade(bool fadeIn)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 1.4f;

            if (fadeIn)
                fadeFX.color = Color.Lerp(Color.clear, Color.black, t);
            else
                fadeFX.color = Color.Lerp(Color.black, Color.clear, t);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        yield return new WaitForSecondsRealtime(0.1f);
    }
    public void ResetCharacters()
    {
        foreach (o_battleChar bc in enemySlots)
        {
            bc.name = "";
            bc.level = 1;
            bc.baseExpYeild = 0;
            bc.moveDatabase.Clear();
            bc.GetComponent<SpriteRenderer>().sprite = null;
        }

        foreach (o_battleChar bc in playerSlots)
        {
            bc.name = "";
            bc.level = 1;
            bc.baseExpYeild = 0;
            //bc.moveDatabase.Clear();
            bc.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public void SetEventFlag()
    {

    }

    public void SwitchToOverworld()
    {
        ///Replacing old player data with new data
        foreach (o_battleChar plObj in playerSlots)
        {
            for (int i = 0; i < partyMembers.Count; i++)
            {
                if (plObj.name == partyMembers[i].name)
                {
                    //partyMembers[i].hitPoints = plObj.hitPoints;
                    //partyMembers[i].skillPoints = plObj.skillPoints;
                    partyMembers[i].moveDatabase = new List<o_battleChar.move_learn>();
                    partyMembers[i].moveDatabase = plObj.moveDatabase;
                    //foreach(s_move mov in plObj.skillMoves)
                    //pl.currentMoves.Add(mov.name);
                    break;
                }
            }
        }
        ResetCharacters();
        if (eventLabelName != "")
        {
            //s_rpgEvent.rpgEv.JumpToEvent(eventLabelName, false);
            eventLabelName = "";
        }

        battleSystem.isActive = false;
        if (battleStarter != null)
        {
            Destroy(battleStarter.gameObject);
            battleStarter = null;
        }
        GameState = RPG_STATE.OVERWORLD;
        s_camera.cam.SetPlayer(player);
        player.control = true;
        player.rendererObj.color = Color.white;
        if(enc != null)
            enc.DestroyAllEnemies();
        StartCoroutine(Fade(false));
    }

    public IEnumerator BattleTransition(enemy_group enemy)
    {
        //Fade stuff
        Time.timeScale = 0;
        player.control = false;
        yield return StartCoroutine(Fade(true));
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        s_camera.cam.SetPlayer(null);
        Camera.main.transform.position = new Vector3(0, 0, -10);
        SwitchToBattle(enemy);
    }

    public void SwitchToBattle(c_enemy enemy)
    {
        s_menuhandler.GetInstance().SwitchMenu("EMPTY");
        battleStarter = enemy;
        StartCoroutine(BattleTransition(enemy.enemyGroup));
    }

    public void SetStats(o_battleChar bc)
    {
        o_battleCharData character = partyMembers.Find(x => x.name == bc.name);
        character.level = bc.level;
        character.exp = bc.exp;
        character.attack = bc.attack;
        character.defence = bc.defence;
        character.guts = bc.guts;
        character.intelligence = bc.intelligence;
        if (bc.hitPoints <= 0)
            character.hitPoints = 1;
        else
            character.hitPoints = bc.hitPoints;
        print("HP: " + character.hitPoints);
        if (character.hitPoints > character.maxHitPoints)
            character.hitPoints = character.maxHitPoints;
        character.maxHitPoints = bc.maxHitPoints;
        character.maxSkillPoints = bc.maxSkillPoints;
        character.skillPoints = bc.skillPoints;
    }

    public void PositionEnemies(int quantity)
    {
        switch (quantity)
        {
            case 1:
                enemySlots[0].transform.position = new Vector3(0, enemySlots[0].transform.position.y);
                break;
            case 2:
                enemySlots[0].transform.position = new Vector3(-45, enemySlots[0].transform.position.y);
                enemySlots[1].transform.position = new Vector3(45, enemySlots[1].transform.position.y);
                break;
            case 3:
                enemySlots[0].transform.position = new Vector3(-45, enemySlots[0].transform.position.y);
                enemySlots[1].transform.position = new Vector3(45, enemySlots[1].transform.position.y);
                enemySlots[2].transform.position = new Vector3(95, enemySlots[2].transform.position.y);
                break;

            case 4:
                enemySlots[0].transform.position = new Vector3(-45, enemySlots[0].transform.position.y);
                enemySlots[1].transform.position = new Vector3(45, enemySlots[1].transform.position.y);
                enemySlots[2].transform.position = new Vector3(95, enemySlots[1].transform.position.y);
                break;
        }
    }

    public void SwitchToBattle(enemy_group groupEnemy)
    {
        print(groupEnemy.name);
        s_camera.cam.SetPlayer(null);
        player.control = false;
        player.direction = Vector2.zero;
        Camera.main.transform.position = new Vector3(0, 0, -10);

        List<o_battleCharData> charactersInBattle = new List<o_battleCharData>();
        charactersInBattle = partyMembers;

        battleSystem.players = new List<o_battleChar>();
        for (int i = 0; i < charactersInBattle.Count; i++)
        {
            o_battleCharData bc = charactersInBattle[i];
            playerSlots[i].skillMoves = new List<s_move>();
            playerSlots[i].name = bc.name;
            playerSlots[i].level = bc.level;
            playerSlots[i].baseExpYeild = bc.baseExpYeild;
            playerSlots[i].maxHitPoints = bc.maxHitPoints;
            playerSlots[i].hitPoints = bc.hitPoints;
            playerSlots[i].maxSkillPoints = bc.maxSkillPoints;
            playerSlots[i].skillPoints = bc.skillPoints;
            playerSlots[i].defence = bc.defence;
            playerSlots[i].intelligence = bc.intelligence;
            playerSlots[i].guts = bc.guts;
            playerSlots[i].speed = bc.speed;
            playerSlots[i].attack = bc.attack;
            playerSlots[i].actionTypeCharts = bc.dataSrc.actionTypeCharts;
            playerSlots[i].elementTypeCharts = bc.dataSrc.elementTypeCharts;
            playerSlots[i].moveDatabase = bc.moveDatabase;
            playerSlots[i].data = bc.dataSrc;
            /*
            foreach (o_battleChar.move_learn ml in bc.moveDatabase)
            {
                AddMove(ref playerSlots[i], ml.moveName);
            }
            ///FOR NOW ADD IN ALL OF THE PLAYER AND ENEY CHARACTERS TO THE QUEUE
            battleSystem.characterTurnQueue.Enqueue(playerSlots[i]);
            */
            playerSlots[i].skillMoves = new List<s_move>();
            foreach (s_move ml in bc.currentMoves)
            {
                AddMove(ref playerSlots[i], ml);
            }

            battleSystem.players.Add(playerSlots[i]);
            battleSystem.allCharacters.Add(playerSlots[i]);
            if (bc.dataSrc.characterAnims != null)
            {
                playerSlots[i].sprites = bc.dataSrc.characterAnims;
                playerSlots[i].rend.sprite = bc.dataSrc.back;
            }
            playerSlots[i].itemUsed = null;
            battleSystem.playerCharacterTurnQueue.Enqueue(playerSlots[i]);
        }

        battleSystem.opposition = new List<o_battleChar>();
        ///Copy data from enemy group to battle
        for (int i = 0; i < groupEnemy.enemies.Length; i++)
        {
            BattleCharacterData enemyName = groupEnemy.enemies[i];
            if (groupEnemy.isFixedLevel)
                SetStatsOpponent(ref enemySlots[i], enemyName, groupEnemy, groupEnemy.fixedLevel[i]);
            else
                SetStatsOpponent(ref enemySlots[i], enemyName, groupEnemy, UnityEngine.Random.Range(groupEnemy.minLevel, groupEnemy.maxLevel));
            //enemySlots[i].GetComponent<SpriteRenderer>().sprite = enem.GetComponent<SpriteRenderer>().sprite;
            battleSystem.opposition.Add(enemySlots[i]);
        }

        for (int i = 0; i < battleSystem.opposition.Count; i++)
        {
            o_battleChar bc = enemySlots[i];
            bc.skillMoves = new List<s_move>();
            foreach (o_battleChar.move_learn ml in bc.moveDatabase)
            {
                if(bc.level >= ml.level)
                    AddMove(ref bc, ml.move);
            }
            battleSystem.allCharacters.Add(bc);
            battleSystem.oppositionCharacterTurnQueue.Enqueue(bc);
        }
        foreach (o_battleChar bc in enemySlots)
        {
            SpriteRenderer spr = bc.GetComponent<SpriteRenderer>();

            if (bc.name == "")
            {
                spr.sprite = null;
            }
        }
        PositionEnemies(battleSystem.opposition.Count);

        if (!battleSystem.isActive)
        {
        }
        ///TODO: ADD ENEMY
        ///

        /*
        for (int i =0; i < enemy.enemyGroup.enemyNames.Length; i++)
        {
            string en = enemy.enemyGroup.enemyNames[i];
            o_battleChar bc = enemyDatabase.Find(x => x.name == en);
            if (bc != null)
                battleSystem.opposition.Add(bc);
        }
        */
        battleSystem.roundNum = 0;
        battleSystem.enemyGroup = groupEnemy;
        battleSystem.pressTurn = battleSystem.players.Count;
        battleSystem.OnBattleEvents = groupEnemy.battleEvents;
        GameState = RPG_STATE.BATTLE;
        battleSystem.isActive = true;
        overworldScene.gameObject.SetActive(false);
        battleScene.gameObject.SetActive(true);
        battleSystem.StartBattleRoutine();
        //battleSystem.UpdatePlayerMeterPos();

        StartCoroutine(Fade(false));
    }

    public void SetStatsOpponent(ref o_battleChar charObj, BattleCharacterData enem, enemy_group enemyGroup, int level)
    {
        int tempLvl = level;
        int tempHP = enem.maxHitPointsB;
        int tempSP = enem.maxSkillPointsB;
        int tempStr = enem.attackB;
        int tempVit = enem.defenceB;
        int tempDx = enem.intelligenceB;
        int tempAg = enem.speedB;
        int tempGut = enem.gutsB;
        int tempLuc = enem.luckB;

        for (int i = 1; i < tempLvl; i++)
        {
            if (i % enem.attackG == 0)
                tempStr++;
            if (i % enem.defenceG == 0)
                tempVit++;
            if (i % enem.intelligenceG == 0)
                tempDx++;
            if (i % enem.speedG == 0)
                tempAg++;
            if (i % enem.gutsG == 0)
                tempGut++;
            //if (i % enem.luckG == 0)
                //tempLuc++;

            tempHP += UnityEngine.Random.Range(enem.maxHitPointsGMin, enem.maxHitPointsGMax + 1);
            tempSP += UnityEngine.Random.Range(enem.maxSkillPointsGMin, enem.maxSkillPointsGMax + 1);
        }
        if (enem.characterAnims != null)
        {
            if (enem.characterAnims.Length > 0) {
                charObj.sprites[0] = enem.characterAnims[0];
                charObj.rend.sprite = enem.characterAnims[0];
            }
        }
        if(enem.front != null)
            charObj.rend.sprite = enem.front;
        charObj.name = enem.name;
        charObj.level = tempLvl;
        charObj.baseExpYeild = enem.baseExpYeild;
        charObj.exp = enem.baseExpYeild;
        charObj.maxHitPoints = tempHP;
        charObj.hitPoints = charObj.maxHitPoints;
        charObj.maxSkillPoints = tempSP;
        charObj.skillPoints = charObj.maxSkillPoints;
        charObj.defence = tempVit;
        charObj.intelligence = tempDx;
        charObj.guts = tempGut;
        charObj.attack = tempStr;
        charObj.speed = tempAg;
        charObj.actionTypeCharts = enem.actionTypeCharts;
        charObj.elementTypeCharts = enem.elementTypeCharts;
        charObj.moveDatabase = enem.moveDatabase;
        charObj.spareDrops = enem.itemDrop.ToArray();
        charObj.characterAI = enem.characterAI;
    }

    public void AddMove(ref o_battleChar character, s_move move)
    {
        character.skillMoves.Add(move);
    }

    public override void SaveData()
    {
        FileStream fs = new FileStream(saveDataName, FileMode.Create);
        BinaryFormatter bin = new BinaryFormatter();
        //s_mapManager lev = GameObject.Find("General").GetComponent<s_mapManager>();

        RPG_save sav = new RPG_save(partyMembers);

        //sav.currentmap = SceneManager.GetActiveScene().name;

        List<Tuple<string, float>> dvF = new List<Tuple<string, float>>();
        List<Tuple<string, int>> dvI = new List<Tuple<string, int>>();
        sav.location = new s_save_vector(player.transform.position.x, player.transform.position.y);
        sav.gbflg = new dat_globalflags(GlobalFlags);
        sav.currentmap = currentMapName;
        print(objectStates.Count);
        sav.trigStates = objectStates;
        //sav.floatVariables = dvF;
        //sav.intVariables = dvI;

        /*s
        new dat_globalflags(GlobalFlags), (int)player.health, (int)player.maxHealth, lev.mapDat.name,  
        
        if (isFixedSaveArea)
            sav.currentmap = fixedSaveAreaName;
        */

        bin.Serialize(fs, sav);
        fs.Close();
    }

    public void AddItem(string it, int amount)
    {
        if (inventory.ContainsKey(it))
        {
            rpg_globals.gl.inventory[it] += amount;
        }
        else
        {
            inventory.Add(it, 0);
            inventory[it] += amount;
        }
    }

    public void AddItem(rpg_item it, int amount)
    {
        if (inventory.ContainsKey(it.name))
        {
            rpg_globals.gl.inventory[it.name] += amount;
        }
        else
        {
            inventory.Add(it.name, 0);
            inventory[it.name] += amount;
        }
    }
    public void AddItem(rpg_item it)
    {
        if (inventory.ContainsKey(it.name))
        {
            rpg_globals.gl.inventory[it.name]++;
        }
        else
        {
            inventory.Add(it.name, 0);
            inventory[it.name]++;
        }
    }

    public void UseItem(string it)
    {
        if (inventory.ContainsKey(it))
        {
            rpg_globals.gl.inventory[it]--;
        }
    }

    public void HealParty() {
        foreach (o_battleCharData bc in partyMembers) {
            bc.hitPoints = bc.maxHitPoints;
        }
    }

    public new void Update()
    {
        if (overworldScene == null)
            overworldScene = GameObject.Find(currentlevelname);

        base.Update();
        if (isMainGame)
        {
            switch (GameState)
            {
                case RPG_STATE.OVERWORLD:
                    battleSystem.isActive = false;
                    battleScene.gameObject.SetActive(false);
                    if (overworldScene != null)
                        overworldScene.gameObject.SetActive(true);

                    if (Input.GetKeyDown(GetKeyPref("left")))
                    {
                        menuChoice--;
                    }
                    if (Input.GetKeyDown(GetKeyPref("right")))
                    {
                        menuChoice++;
                    }

                    switch (MenuState)
                    {
                        case RPG_MENU_STATE.OFF:
                            characterStats.text = "";
                            itemsMenu.gameObject.SetActive(false);
                            statsMenu.gameObject.SetActive(false);
                            overworldMenuBox.gameObject.SetActive(false);
                            targetHealer.gameObject.SetActive(false);
                            userHealer.gameObject.SetActive(false);
                            if (menuAble)
                            {
                                /*
                                if (Input.GetKeyDown(GetKeyPref("menu")))
                                {
                                    //Pause();
                                    Time.timeScale = 0;
                                    MenuState = RPG_MENU_STATE.MENU;
                                }
                                */
                            }
                            break;

                        case RPG_MENU_STATE.MENU:
                            Time.timeScale = 0;
                            StatusButton.gameObject.SetActive(true);
                            ItemsButton.gameObject.SetActive(true);

                            overworldMenuBox.gameObject.SetActive(true);
                            menuChoice = Mathf.Clamp(menuChoice, 0, 1);

                            characterStats.text = "" + "\n";

                            if (menuChoice == 0)
                                StatusButton.color = Color.yellow;
                            else
                                StatusButton.color = Color.white;

                            if (menuChoice == 1)
                                ItemsButton.color = Color.yellow;
                            else
                                ItemsButton.color = Color.white;

                            if (Input.GetKeyDown(GetKeyPref("select")))
                            {
                                switch (menuChoice)
                                {
                                    case 0:
                                        MenuState = RPG_MENU_STATE.STATUS;
                                        break;
                                    case 1:
                                        MenuState = RPG_MENU_STATE.ITEMS;
                                        break;
                                }
                                StatusButton.gameObject.SetActive(false);
                                ItemsButton.gameObject.SetActive(false);
                            }

                            if (Input.GetKeyDown(GetKeyPref("back")))
                            {
                                Time.timeScale = 1;
                                StatusButton.gameObject.SetActive(false);
                                ItemsButton.gameObject.SetActive(false);
                                MenuState = RPG_MENU_STATE.OFF;
                            }
                            break;

                        case RPG_MENU_STATE.STATUS:
                            statsMenu.gameObject.SetActive(true);
                            menuChoice = Mathf.Clamp(menuChoice, 0, partyMembers.Count - 1);

                            currentPartyMemberSelection = partyMembers[menuChoice];
                            strStat.amount = currentPartyMemberSelection.attack;
                            vitStat.amount = currentPartyMemberSelection.defence;
                            intStat.amount = currentPartyMemberSelection.intelligence;
                            gutStat.amount = currentPartyMemberSelection.guts;

                            characterMenEXP.value = currentPartyMemberSelection.exp;


                            characterMenHP.value = (((float)currentPartyMemberSelection.hitPoints / (float)currentPartyMemberSelection.maxHitPoints) * 100);
                            characterMenSP.value = (((float)currentPartyMemberSelection.skillPoints / (float)currentPartyMemberSelection.maxSkillPoints) * 100);

                            characterMenHPTxt.text = "HP: " + currentPartyMemberSelection.hitPoints + "/ " + currentPartyMemberSelection.maxHitPoints + " " 
                                + currentPartyMemberSelection.hitPoints + "/" + currentPartyMemberSelection.maxHitPoints;
                            characterMenSPTxt.text = "SP: " + currentPartyMemberSelection.skillPoints + "/ " + currentPartyMemberSelection.maxSkillPoints + " "
                                + currentPartyMemberSelection.skillPoints + "/" + currentPartyMemberSelection.maxSkillPoints; ;
                            characterMenEXPTxt.text = currentPartyMemberSelection.exp + "%";

                            characterMenName.text = "Name: " + currentPartyMemberSelection.name + " - Level: " + currentPartyMemberSelection.level;

                            if (Input.GetKeyDown(GetKeyPref("select")))
                            {
                                statsMenu.gameObject.SetActive(false);
                                menuChoice = 0;
                                itemsMenu.gameObject.SetActive(true);
                                MenuState = RPG_MENU_STATE.SKILLS;
                            }
                            if (Input.GetKeyDown(GetKeyPref("back")))
                            {
                                statsMenu.gameObject.SetActive(false);
                                MenuState = RPG_MENU_STATE.MENU;
                            }
                            break;

                        case RPG_MENU_STATE.ITEMS:
                            itemsMenu.gameObject.SetActive(true);
                            menuChoice = Mathf.Clamp(menuChoice, 0, inventory.Count - 1);

                            //This lists the stuff that will be presented in the inventory and the ammount
                            //Tried tuples, but that failed so I'm doing a much more crude but easier option

                            List<string> stuffName = new List<string>();
                            List<int> stuffAmount = new List<int>();

                            //cur = partyMembers[menuChoice];
                            foreach (KeyValuePair<string, int> it in inventory)
                            {
                                stuffName.Add(it.Key);
                                stuffAmount.Add(it.Value);
                            }
                            for (int i = 0; i < menuItemButtonsSelector.Length; i++)
                            {
                                menuItemButtonsSelector[i].txt.text = "";
                                menuItemButtonsSelector[i].selector.gameObject.SetActive(false);
                                menuItemButtonsSelector[i].img.gameObject.SetActive(false);
                            }
                            //inventoryData.Find(x => x.name == it.Key)
                            for (int i = 0; i < stuffName.Count; i++)
                            {
                                menuItemButtonsSelector[i].txt.text = stuffName[i] + " x " + stuffAmount[i];
                                menuItemButtonsSelector[i].selector.gameObject.SetActive(false);
                                menuItemButtonsSelector[i].img.gameObject.SetActive(true);
                                //print("Yes");
                                if (menuChoice == i)
                                    menuItemButtonsSelector[i].selector.gameObject.SetActive(true);
                                //characterStats.text += stuffName[i] + " x " + stuffAmount[i] + "\n";
                            }

                            if (Input.GetKeyDown(GetKeyPref("select")))
                            {
                                s_move mov = null;
                                rpg_item it = inventoryData.Find(x => x.name == stuffName[menuChoice]);
                                //targetHealer.chara = 
                                if (it != null)
                                {
                                    mov = it.action;
                                    if (mov != null)
                                    {
                                        //All status effects will be cured when the characters get out of battle
                                        if (mov.moveType == MOVE_TYPE.STATUS && mov.statusMoveType == STATUS_MOVE_TYPE.HEAL)
                                        {
                                            if (partyMembers.FindAll(x => x.hitPoints < x.maxHitPoints) != null)
                                            {

                                                isItem = true;
                                                userHealer.chara = currentPartyMemberSelection;
                                                healMove = mov;
                                                MenuState = RPG_MENU_STATE.HEAL_TARGET;
                                            }
                                        }
                                    }
                                }
                            }
                            characterStats.text = "";

                            characterStats.text +=
                                   "X to exit" + " \n";

                            if (Input.GetKeyDown(GetKeyPref("back")))
                                MenuState = RPG_MENU_STATE.MENU;
                            break;

                        case RPG_MENU_STATE.HEAL_TARGET:
                            menuChoice = Mathf.Clamp(menuChoice, 0, partyMembers.Count - 1);
                            o_battleCharData t = partyMembers[menuChoice];
                            if (partyMembers[menuChoice] != currentPartyMemberSelection)
                            {
                                targetHealer.chara = partyMembers[menuChoice];
                                targetHealer.gameObject.SetActive(true);
                            }
                            else
                            {
                                targetHealer.gameObject.SetActive(false);
                            }
                            userHealer.chara = currentPartyMemberSelection;
                            userHealer.gameObject.SetActive(true);
                            if (Input.GetKeyDown(GetKeyPref("select")))
                            {
                                if (!isItem)
                                {
                                    if (t.hitPoints < t.maxHitPoints && currentPartyMemberSelection.skillPoints > healMove.cost)
                                    {
                                        currentPartyMemberSelection.skillPoints -= healMove.cost;
                                        if (!healMove.isFixed)
                                            targetHealer.chara.hitPoints += Mathf.RoundToInt(healMove.power * currentPartyMemberSelection.intelligence);
                                        else
                                            targetHealer.chara.hitPoints += healMove.power;

                                    }
                                }
                                else
                                {
                                    targetHealer.chara.hitPoints += healMove.power;
                                }
                            }
                            if (Input.GetKeyDown(GetKeyPref("back")))
                            {
                                if (!isItem)
                                {
                                    MenuState = RPG_MENU_STATE.SKILLS;
                                    menuChoice = 0;
                                }
                                else
                                {
                                    MenuState = RPG_MENU_STATE.ITEMS;
                                }
                                userHealer.gameObject.SetActive(false);
                                targetHealer.gameObject.SetActive(false);
                            }
                            break;

                        case RPG_MENU_STATE.SKILLS:
                            if (currentPartyMemberSelection != null)
                            {
                                /*
                                characterStats.text = "";
                                cur = partyMembers[0];
                                for (int i = 0; i < cur.moveDatabase.Count; i++)
                                {
                                    s_move mov = cur.currentMoves[i];
                                    characterStats.text += "Name: " + mov.name;
                                    if (mov.moveType == MOVE_TYPE.PHYSICAL) {
                                        characterStats.text +="- " + mov.cost + "HP \n";
                                    } else if (mov.moveType == MOVE_TYPE.SPECIAL) {
                                        characterStats.text += "- " + mov.cost + " SP \n";
                                    }else  {
                                        characterStats.text +="\n";
                                    }
                                }
                                */
                            }
                            //characterStats.text += "X to exit" + " \n";

                            menuChoice = Mathf.Clamp(menuChoice, 0, currentPartyMemberSelection.currentMoves.Count - 1);

                            //This lists the stuff that will be presented in the inventory and the ammount
                            //Tried tuples, but that failed so I'm doing a much more crude but easier option

                            for (int i = 0; i < menuItemButtonsSelector.Length; i++)
                            {
                                menuItemButtonsSelector[i].txt.text = "";
                                menuItemButtonsSelector[i].selector.gameObject.SetActive(false);
                                menuItemButtonsSelector[i].img.gameObject.SetActive(false);
                            }
                            //inventoryData.Find(x => x.name == it.Key)
                            for (int i = 0; i < currentPartyMemberSelection.currentMoves.Count; i++)
                            {
                                s_move mov = currentPartyMemberSelection.currentMoves[i];
                                if (mov.cost > 0)
                                {
                                    if (mov.moveType == MOVE_TYPE.PHYSICAL)
                                        menuItemButtonsSelector[i].txt.text = mov.name + " - " + mov.cost + " HP";
                                    else
                                        menuItemButtonsSelector[i].txt.text = mov.name + " - " + mov.cost + " SP";
                                }
                                else
                                {
                                    menuItemButtonsSelector[i].txt.text = mov.name;
                                }
                                menuItemButtonsSelector[i].selector.gameObject.SetActive(false);
                                menuItemButtonsSelector[i].img.gameObject.SetActive(true);
                                //print("Yes");
                                if (menuChoice == i)
                                    menuItemButtonsSelector[i].selector.gameObject.SetActive(true);
                                //characterStats.text += stuffName[i] + " x " + stuffAmount[i] + "\n";
                            }

                            if (Input.GetKeyDown(GetKeyPref("select")))
                            {
                                //targetHealer.chara = 
                                s_move mov = currentPartyMemberSelection.currentMoves[menuChoice];
                                if (mov.moveType == MOVE_TYPE.SPECIAL && mov.statusMoveType == STATUS_MOVE_TYPE.HEAL)
                                {
                                    isItem = false;
                                    userHealer.chara = currentPartyMemberSelection;
                                    healMove = mov;
                                    MenuState = RPG_MENU_STATE.HEAL_TARGET;
                                }
                            }

                            if (Input.GetKeyDown(GetKeyPref("back")))
                            {
                                itemsMenu.gameObject.SetActive(false);
                                menuChoice = partyMembers.IndexOf(currentPartyMemberSelection);
                                MenuState = RPG_MENU_STATE.STATUS;
                            }
                            break;
                    }
                    break;

            }
        }
    }

    new void OnGUI()
    {
        base.OnGUI();

        if (GameState == RPG_STATE.OVERWORLD)
        {
            switch (MenuState)
            {
                case RPG_MENU_STATE.OFF:
                    //pressTextMenu.text = "Press " + GetKeyPref("menu").ToString() + " to open menu";
                    break;

                case RPG_MENU_STATE.MENU:

                    break;
            }
        }

    }
}
