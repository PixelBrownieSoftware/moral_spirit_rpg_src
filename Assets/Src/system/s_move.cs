using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TARGET_MOVE_TYPE
{
    SINGLE,
    RANDOM,
    ALL
}

[System.Serializable]
[CreateAssetMenu (menuName = "Move")]
public class s_move : ScriptableObject
{
    public bool isMultiHit = false;
    public bool isFixed = false;
    public bool onSelf = false;
    public bool onTeam;
    public int cost;    //If it's physical, it costs HP, if it's a magic move, it would cost SP, talk moves cost neither but they are often weak
    public bool affectedBySilence;  //Can still preform even if silenced
    public bool multiTech;  //For multi-Tech moves, these will automatically generate if two or more characters fufil the move requirements
    public string actionDescription;    //For when the attack is preformed
    public string infoDescription;  //To describe the attack
    public ELEMENT element;     //Irrelevant for act moves
    public ACTION_TYPE action_type;     //Action moves may contain this, most will be set to none
    public MOVE_TYPE moveType;     //Action moves may contain this, most will be set to none
    public TARGET_MOVE_TYPE target;
    public int power;
    public s_moveAnim[] preAnim;  //The move's animation that plays when it is excecuted
    public s_moveAnim[] animation;  //The move's animation that plays when it is excecuted
    public string[] moveRequirements; //The move requirements for multiTechs
    public int accuracy = 85;

    public int str_inc;
    public int vit_inc;
    public int dex_inc;
    public int luc_inc;
    public int agi_inc;
    public int gut_inc;

    public bool buffUser = false;
    public bool buffTarget = true;
    public bool canLearn = true;
    public bool excludeUser = false;

    public int str_req;
    public int dex_req;
    public int vit_req;
    public int agi_req;
    public int gut_req;

    public string customFx;

    public s_statusEffectChance statusEffectChances;
    public STATUS_MOVE_TYPE statusMoveType;

    [System.Serializable]
    public struct s_statusEffectChance {
        public float statusEffectChance; // 0 - 100% expressed in decimal
        public STATUS_EFFECT status_effect;
    }

    [System.Serializable]
    public struct s_moveAnim
    {
        public float duration;
        public Sprite image;
        public string name; //i.e. animation name
        public Vector2 position;
        public enum ANIM_TYPE { 
            IMAGE,
            CALCUATION,
            INFLICT_STATUS,
            ANIMATION,
            CAMERA,
            SOUND
        };
        public MagnumFoundation2.System.s_camera.CAMERA_MODE camMode;
        public ANIM_TYPE type;
        public enum MOVEPOSTION
        {
            ON_TARGET,
            FIXED,
            ALL_SAME_TIME,
            ALL_LEFT_TO_RIGHT,
            ALL_RIGHT_TO_LEFT,
            ON_USER
        }
        public MOVEPOSTION pos;
        public RuntimeAnimatorController anim;
    }
}
