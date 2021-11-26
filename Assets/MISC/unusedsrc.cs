/*
 * 
 * 
 * 
added on 09/10/2020

if (!middleOfAction)
            {
                if (isPlayerTurn)
                {
                    currentCharacter = playerCharacterTurnQueue.Peek();

                    if (currentCharacter.hitPoints == 0 || currentCharacter.skillPoints == -currentCharacter.maxSkillPoints)
                    {
                        playerCharacterTurnQueue.Dequeue();
                        playerCharacterTurnQueue.Enqueue(currentCharacter);
                    }

                    if (!playerUpTurn)
                    {
                        //s_soundmanager.sound.PlaySound(ref upTurn, false);
                        playerUpTurn = true;
                    }
                    PlayerChoice();
                    if (netTurn <= 0)
                    {
                        pressTurn = oppositionCharacterTurnQueue.Count;
                        isPlayerTurn = false;
                    }
                }
                else
                {
                    currentCharacter = oppositionCharacterTurnQueue.Peek();
                    EnemySelectAttack(ref currentCharacter);

                    if (netTurn <= 0)
                    {
                        pressTurn = playerCharacterTurnQueue.Count;
                        isPlayerTurn = true;
                    }
                }
            }
            if (currentMove != null && !middleOfAction)
            {
                s_battleAction move = currentMove;
                s_move mov = move.move;

                if (!middleOfAction)
                {
                    middleOfAction = true;
                    if (move != null)
                    {
                        actionDisp.text = move.user.name + " used " + move.move.name + " on " + move.target.name + "\n";
                        switch (mov.moveType)
                        {
                            case MOVE_TYPE.PHYSICAL:
                                move.user.hitPoints -= mov.spCost;
                                break;

                            case MOVE_TYPE.SPECIAL:
                                move.user.skillPoints -= mov.spCost;
                                break;

                            case MOVE_TYPE.TALK:
                                if (isPlayerTurn)
                                    playerMoralePoints -= mov.spCost;
                                else
                                    enemyMoralePoints -= mov.spCost;
                                break;
                        }
                        StartCoroutine(PlayAniamtion(move.move.animation, move));
                    }
                }
            }
 
 
 */

/*
PUT HERE ON 13/11/2021
case BATTLE_ENGINE_STATE.NEGOTIATE_MENU:

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
break;
*/
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
/*
PUT HERE ON 13/11/2021

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
                        
                        */
/*
PUT HERE ON 13/11/2021
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
*/
/*
PUT HERE ON 13/11/2021
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
/*
PUT HERE ON 13/11/2021
public void SpawnDamageObject(Vector2 position, int damage)
{
    s_object du = hitObj;
    du.GetComponent<Animator>().Play("damageAnim");
    du.transform.GetChild(1).GetComponent<Text>().text = "";
    du.transform.GetChild(1).GetComponent<Text>().text = "" + damage;
    du.transform.position = position;
    
}*/

/*
PUT HERE ON 13/11/2021
private void OnGUI()
{
   // DrawPressTurn();
    for (int i = 0; i < players.Count; i++)
    {
        o_battleChar b = players[i];
        if (drawExp)
        {
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
        }
    }
}
*/
/*
PUT HERE ON 13/11/2021

    public void HighlightMenuButton(ref Image img) {
        menuSelector.rectTransform.position = img.rectTransform.position;
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
PUT HERE ON 13/11/2021
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
PUT HERE ON 13/11/2021
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

/*
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
*/