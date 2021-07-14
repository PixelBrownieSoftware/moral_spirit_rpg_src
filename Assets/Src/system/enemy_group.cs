using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2.Objects;

[CreateAssetMenu]
public class enemy_group : ScriptableObject
{
    public ev_script endEvent;
    public BattleCharacterData[] enemies;
    public int maxLevel;
    public int minLevel;
    public int[] fixedLevel;
    public bool isFixedLevel = false;
    public s_battleEvents[] battleEvents;

    public bool allRelations = false;
    public bool isFleeable = true;
    public string songName = "spirit_match";
}
