using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Item")]
public class rpg_item : ScriptableObject
{
    public enum TYPE
    {
        RECOVER,
        WEAPON,
        ASSIST,
        KEY
    }
    public s_move action;
    public string description;
    public TYPE itemType;
}
