using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

/*
public class s_rpgloader : s_mapManager
{
    public List<RuleTile> ruleTiles = new List<RuleTile>();

    private new void Awake()
    {
        base.Awake();
        LevEd = this;
        InitializeLoader();
        InitializeGameWorld();
        s_camera.cam.SetPlayer(player);
        DontDestroyOnLoad(this);
    }
}
*/

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
