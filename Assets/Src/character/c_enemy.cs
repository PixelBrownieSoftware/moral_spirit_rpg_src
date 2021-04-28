using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.Objects;

[System.Serializable]
public class s_bEvent
{
    public enum B_ACTION_TYPE
    {
        DIALOGUE,
        MOVE,
        END_BATTLE,
        SET_FLAG,
        CHECK_FLAG
    }
    public B_ACTION_TYPE battleAction;
    public int int0;    //Turns elapsed, Health, Stamina condition
    public int int1;    //Damage dealt
    public float float0;    //Damage dealt
    public string name;    //Name of character which needs to be checked
    public string string0;    //Dialogue that is said/Move to be preformed
    public string[] stringArr;    //Dialogue that is said/Move to be preformed
    public bool enabled = true;
    public ev_script eventScript;
}
[System.Serializable]
public class s_battleEvents{
    //public List<s_bEvent> events;
    public ev_script eventScript;
    public enum B_COND
    {
        HEALTH,
        STAMINA,
        TURNS_ELAPSED,
        MOVE_USED
    }
    public enum B_CHECK_COND
    {
        PER_TURN,
        ON_START
    }
    public B_COND battleCond;
    public B_CHECK_COND battleCheckCond;
    public bool enabled = true;
    public int int0;
}


public class c_enemy : o_overworlC
{

    public enemy_group enemyGroup;
    public string eGroupName;
    public BoxCollider2D col;
    public bool touched = false;
    public bool AIActive = true;
    float spawnTimer;
    bool goingBackToSpawn = false;

    public Transform[] waypoints;
    public int currentWP = 0;
    public u_encounter encounterGrp;

    new void Start()
    {
        DisableAttack();
        Initialize();
        spawnpoint = transform.position;
        base.Start();
    }

    new private void Update()
    {
        base.Update();

        c_player c = IfTouchingGetCol<c_player>(col);
        if (c != null)
        {
            if (!touched)
            {
                touched = true;
                c.control = false;
                c.CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
                rpg_globals.gl.SwitchToBattle(this);
            }

        }
    }
    
    public IEnumerator GoBackToSpawn() {
        goingBackToSpawn = true;
        float t = 0;
        collision.enabled = false;
        terminalspd = terminalSpeedOrigin * 2.45f;
        while (!CheckTargetDistance(spawnpoint, 20))
        {
            direction = LookAtTarget(spawnpoint);
            rendererObj.color = Color.Lerp(Color.white, Color.clear, t);
            t += Time.deltaTime * 11.5f;
            CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
        t = 0;
        while (rendererObj.color != Color.white)
        {
            rendererObj.color = Color.Lerp(Color.clear, Color.white, t);
            t += Time.deltaTime * 15f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        terminalspd = terminalSpeedOrigin;
        collision.enabled = true;
        goingBackToSpawn = false;
    }

    public override void ArtificialIntelleginceControl()
    {
        if (waypoints != null)
        {
            if (waypoints.Length > 0)
            {
                currentWP = currentWP % waypoints.Length;
                if (!CheckTargetDistance(waypoints[currentWP].position, 25f))
                {
                    CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                    direction = LookAtTarget(waypoints[currentWP].position);
                }
                else
                {
                    currentWP++;
                }
            }
        }
        /*
        if (target == null)
            target = GetClosestTarget<c_player>();
        else
        if (CheckTargetDistance(target, 200))
        {
            spawnTimer = 0.87f;
            if (AIActive)
            {
                CHARACTER_STATE = CHARACTER_STATES.STATE_MOVING;
                direction = LookAtTarget(target);
            }
        }
        else
        {
            if(!goingBackToSpawn)
                CHARACTER_STATE = CHARACTER_STATES.STATE_IDLE;
            if (spawnTimer > 0)
                spawnTimer -= Time.deltaTime;
            else if (!CheckTargetDistance(target, 50))
            {
                if (!goingBackToSpawn)
                    StartCoroutine(GoBackToSpawn());
            }
        }
        */

    }
}
