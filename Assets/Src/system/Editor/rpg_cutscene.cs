using UnityEngine;
using UnityEditor;
using MagnumFoundation2.Objects;
using MagnumFoundation2Editor;

[CustomEditor(typeof(ev_script), true)]
public partial class rpg_cutscene : ed_event
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Debug.Log("gggg");
    }

    public new void OnEnable()
    {
        base.OnEnable();
    }

    public override void DrawCustomEvents(string evType, ref Rect rect, ref SerializedProperty element, ref float sep)
    {
        base.DrawCustomEvents(evType, ref rect, ref element, ref sep);
        switch (evType)
        {
            case "REMOVE_PARTY_MEMBER":
                DrawFeild(ref rect, element, "string0", "Character name", 100, ref sep);
                break;

            case "ADD_PARTY_MEMBER":
                Debug.Log(evType);
                DrawFeild(ref rect, element, "string0", "Character name", 100, ref sep);
                DrawFeild(ref rect, element, "int0", "Character level", 100, ref sep);
                break;

            case "START_BATTLE":
                DrawFeild(ref rect, element, "scrObj", "Group name", 100, ref sep);
                break;
                /*
            case "BIG_TEXT":
                DrawFeild(ref rect, element, "scrObj", "Group name", 100, ref sep);
                break;
                */
        }
    }
}