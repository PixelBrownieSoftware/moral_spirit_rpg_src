using System.IO;
using UnityEngine;
using UnityEditor;




[CustomEditor(typeof(rpg_globals))]
public class ed_rpgGlobals : Editor
{
    public string directoryMove;
    public string directoryGroup;
    bool[] foldoutlist;
    int animationLeng;

    public override void OnInspectorGUI()
    {
        rpg_globals tra = (rpg_globals)target;

        base.OnInspectorGUI();

        if (tra != null)
        {
            /*
            #region EDIT CURRENT MOVE
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            tra.currentMove.name = EditorGUILayout.TextArea(tra.currentMove.name);
            EditorGUILayout.BeginHorizontal();
            tra.currentMove.moveType = (MOVE_TYPE)EditorGUILayout.EnumPopup(tra.currentMove.moveType);
            tra.currentMove.element = (ELEMENT)EditorGUILayout.EnumPopup(tra.currentMove.element);
            switch (tra.currentMove.moveType)
            {

                case MOVE_TYPE.TALK:
                    tra.currentMove.action_type = (ACTION_TYPE)EditorGUILayout.EnumPopup(tra.currentMove.action_type);
                    break;

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("On team?");
            tra.currentMove.onTeam = EditorGUILayout.Toggle(tra.currentMove.onTeam);
            EditorGUILayout.EndHorizontal();

            switch (tra.currentMove.moveType)
            {
                case MOVE_TYPE.SPECIAL:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stamina point cost");
                    tra.currentMove.spCost = EditorGUILayout.IntField(tra.currentMove.spCost);
                    EditorGUILayout.EndHorizontal();
                    break;

                case MOVE_TYPE.TALK:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Courage point cost");
                    tra.currentMove.cpCost = EditorGUILayout.IntField(tra.currentMove.cpCost);
                    EditorGUILayout.EndHorizontal();
                    break;

            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base power");
            tra.currentMove.power = EditorGUILayout.IntField(tra.currentMove.power);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Action description (when used)");
            tra.currentMove.actionDescription = EditorGUILayout.TextArea(tra.currentMove.actionDescription);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Info description (when viewed on menu)");
            tra.currentMove.infoDescription = EditorGUILayout.TextArea(tra.currentMove.infoDescription);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Animation");
            EditorGUILayout.Space();
            if (tra.currentMove.animation != null)
            {
                int leng = tra.currentMove.animation.Length;
                for (int i = 0; i < leng; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Position: ");
                    tra.currentMove.animation[i].pos = (s_move.s_moveAnim.MOVEPOSTION)EditorGUILayout.EnumFlagsField(tra.currentMove.animation[i].pos);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Animation name:");
                    tra.currentMove.animation[i].name = EditorGUILayout.TextArea(tra.currentMove.animation[i].name);
                    EditorGUILayout.EndHorizontal();
                }
            }

            animationLeng = EditorGUILayout.IntField(animationLeng);
            if (GUILayout.Button("New Feild"))
            {
                tra.currentMove.animation = new s_move.s_moveAnim[animationLeng];
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            #endregion
            #region SAVE/LOAD MOVES
            if (GUILayout.Button("New Move"))
            {
                tra.currentMove = new s_move();
                directoryMove = null;
            }
            if (GUILayout.Button("Save Move As"))
            {
                directoryMove = EditorUtility.SaveFilePanel("Save Json move file", "Assets/Data/Moves/", tra.currentMove.name, "txt");
                if (directoryMove != null)
                {
                    string json = JsonUtility.ToJson(tra.currentMove, true);
                    File.WriteAllText(directoryMove, json);
                }
            }
            if (GUILayout.Button("Load Move"))
            {
                directoryMove = EditorUtility.OpenFilePanel("Open Json move file", "Assets/Data/Moves/", "txt");
                if (directoryMove != null)
                {
                    string fil = File.ReadAllText(directoryMove);
                    if (fil != null)
                    {
                        tra.currentMove = JsonUtility.FromJson<s_move>(fil);
                        animationLeng = tra.currentMove.animation.Length;
                    }
                }
            }

            if (directoryMove != null)
            {
                if (GUILayout.Button("Save Move: " + tra.currentMove.name))
                {
                    string json = JsonUtility.ToJson(tra.currentMove);
                    File.WriteAllText(directoryMove, json);
                }
            }
            #endregion
            */
        }
    }
}
/*
           if (foldoutlist != null)
           {
               for (int i = 0; i < tra.moveDatabase.Count; i++)
               {
                   s_move mov = tra.moveDatabase[i];
                   foldoutlist[i] = EditorGUILayout.Foldout(foldoutlist[i], mov.name);
                   if (foldoutlist[i])
                   {
                       mov.name = EditorGUILayout.TextArea(mov.name);
                       EditorGUILayout.BeginHorizontal();
                       mov.moveType = (MOVE_TYPE)EditorGUILayout.EnumPopup(mov.moveType);
                       mov.element = (ELEMENT)EditorGUILayout.EnumPopup(mov.element);
                       switch (mov.moveType)
                       {

                           case MOVE_TYPE.TALK:
                               mov.action_type = (ACTION_TYPE)EditorGUILayout.EnumPopup(mov.action_type);
                               break;

                       }
                       EditorGUILayout.EndHorizontal();

                       EditorGUILayout.BeginHorizontal();
                       EditorGUILayout.LabelField("On team?");
                       mov.onTeam = EditorGUILayout.Toggle(mov.onTeam);
                       EditorGUILayout.EndHorizontal();

                       switch (mov.moveType)
                       {
                           case MOVE_TYPE.SKILL:
                               EditorGUILayout.BeginHorizontal();
                               EditorGUILayout.LabelField("Stamina point cost");
                               mov.spCost = EditorGUILayout.IntField(mov.spCost);
                               EditorGUILayout.EndHorizontal();
                               break;

                           case MOVE_TYPE.TALK:
                               EditorGUILayout.BeginHorizontal();
                               EditorGUILayout.LabelField("Courage point cost");
                               mov.cpCost = EditorGUILayout.IntField(mov.cpCost);
                               EditorGUILayout.EndHorizontal();
                               break;

                       }
                       EditorGUILayout.BeginHorizontal();
                       EditorGUILayout.LabelField("Base power");
                       mov.power = EditorGUILayout.IntField(mov.power);
                       EditorGUILayout.EndHorizontal();

                       EditorGUILayout.BeginHorizontal();
                       EditorGUILayout.LabelField("Action description (when used)");
                       mov.actionDescription = EditorGUILayout.TextArea(mov.actionDescription);
                       EditorGUILayout.EndHorizontal();

                       EditorGUILayout.BeginHorizontal();
                       EditorGUILayout.LabelField("Info description (when viewed on menu)");
                       mov.infoDescription = EditorGUILayout.TextArea(mov.infoDescription);
                       EditorGUILayout.EndHorizontal();
                   }

                   EditorGUILayout.Space();
               }
           }
           else {
               foldoutlist = new bool[tra.moveDatabase.Count];
           }
           */
