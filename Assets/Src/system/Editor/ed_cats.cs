using UnityEngine;
using UnityEditor;

/*
[CanEditMultipleObjects]
[CustomEditor(typeof(o_battleChar))]
public class ed_cats : Editor
{
public override void OnInspectorGUI()
{
    base.OnInspectorGUI();
    o_battleChar tra = (o_battleChar)target;
    for (int i = 0; i < tra.moveDatabase.Count; i++)
    {
        EditorGUILayout.Space();
        o_battleChar.move_learn mov = tra.moveDatabase[i];
        rpg_globals rpg = GameObject.Find("General").GetComponent<rpg_globals>();
        foreach (TextAsset move in rpg.moveDatabaseJson)
        {
            if (GUILayout.Button(move.name))
            {
                mov.moveName = move.name;
            }
        }
        if (mov.level != 0)
            EditorGUILayout.LabelField("Move name: " + mov.moveName + ", level learned: " + mov.level);
        else
            EditorGUILayout.LabelField("Move name: " + mov.moveName + ", Start with");
        mov.level = EditorGUILayout.IntField(mov.level);

        EditorGUILayout.Space();
    }
}
}
*/