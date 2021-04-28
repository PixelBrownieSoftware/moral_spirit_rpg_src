using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CanEditMultipleObjects]
[CustomEditor(typeof(enemy_group))]
public class ed_enGroup : Editor
{
    enemy_group enGroup;
    public string directoryMove;
    public string directoryGroup;
    public List<o_battleCharData> names;
    public int maxLevel, minLevel;
    rpg_globals RBGB;
    public int leng;

    public List<s_bEvent> battleEvents = new List<s_bEvent>();

    public List<o_battleCharData> characterDictionary;

    int tab = 0;
    enemy_group data;
    ELEMENT elementSliderSelector = ELEMENT.NORMAL;
    ACTION_TYPE actionSliderSelector = ACTION_TYPE.NONE;
    string element;

    public override void OnInspectorGUI()
    {
        enGroup = (enemy_group)target;
        tab = GUILayout.Toolbar(tab, new string[] { "Members", "Events", "Raw data"});
        switch (tab)
        {
            /*
            case 0:
                if (enGroup.enemies != null)
                    for (int i = 0; i < enGroup.enemies.Length; i++)
                    {
                        enGroup.enemies[i] = EditorGUILayout.ObjectField(enGroup.enemies[i], typeof(BattleCharacterData), false) as BattleCharacterData;
                    }
                leng = EditorGUILayout.IntField(leng);
                if (GUILayout.Button("New Entity"))
                {
                    enGroup.enemies = new BattleCharacterData[leng];
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Level :");
                minLevel = EditorGUILayout.IntField(minLevel);
                EditorGUILayout.LabelField(" to ");
                maxLevel = EditorGUILayout.IntField(maxLevel);
                EditorGUILayout.EndHorizontal();
                break;

            case 1:
                if (enGroup.battleEvents != null)
                    for (int i = 0; i < enGroup.battleEvents.Length; i++)
                    {
                        s_battleEvents bev = enGroup.battleEvents[i];
                        bev.enabled = true;
                        EditorGUILayout.BeginHorizontal();
                        bev.battleCheckCond = (s_battleEvents.B_CHECK_COND)EditorGUILayout.EnumPopup(bev.battleCheckCond);
                        bev.battleCond = (s_battleEvents.B_COND)EditorGUILayout.EnumPopup(bev.battleCond);
                        switch (bev.battleCond)
                        {
                            case s_battleEvents.B_COND.TURNS_ELAPSED:
                                bev.int0 = EditorGUILayout.IntField("Turns elapsed: ", bev.int0);
                                break;
                            case s_battleEvents.B_COND.HEALTH:
                                bev.float0 = EditorGUILayout.Slider("Health percentage: ", bev.float0, 0, 1);
                                break;
                        }
                        EditorGUILayout.EndHorizontal();
                        bev.battleAction = (s_bEvent.B_ACTION_TYPE)EditorGUILayout.EnumPopup(bev.battleAction);
                        switch (bev.battleAction)
                        {
                            case s_bEvent.B_ACTION_TYPE.DIALOGUE:
                                bev.string0 = EditorGUILayout.TextField("Dialogue message: ", bev.string0);
                                break;
                            case s_bEvent.B_ACTION_TYPE.MOVE:
                                EditorGUILayout.BeginHorizontal();
                                bev.string0 = EditorGUILayout.TextField("Dialogue message: ", bev.string0);
                                EditorGUILayout.EndHorizontal();
                                break;

                            case s_bEvent.B_ACTION_TYPE.END_BATTLE:
                                EditorGUILayout.LabelField("End battle");
                                bev.name = EditorGUILayout.TextField("Jump to label on end: ", bev.name);
                                break;
                        }
                    }
                enGroup.endEvent = EditorGUILayout.TextArea(enGroup.endEvent);
                break;

                */
            case 0:
                base.OnInspectorGUI();
                break;
        }


        EditorGUILayout.Space();
    }
}
