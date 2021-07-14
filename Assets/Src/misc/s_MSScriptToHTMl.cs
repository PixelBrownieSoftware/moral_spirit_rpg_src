using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoundation2;
using MagnumFoundation2.Objects;

public class s_MSScriptToHTMl : s_eventsToHTML
{
    protected override void ConvertScript(ev_details script)
    {
        switch (script.eventType) {
            default:
                base.ConvertScript(script);
                break;

            case EVENT_TYPES.CUSTOM_FUNCTION:
                switch (script.funcName) {
                    case "BIG_TEXT":
                        if (prevScript != null)
                        {
                            if (prevScript.eventType == EVENT_TYPES.CUSTOM_FUNCTION)
                            {
                                if (prevScript.funcName != script.funcName)
                                {
                                    WriteLine("<h3>Narrator</h3>");
                                    prevScript = script;
                                }
                            }
                        }
                        else
                        {
                            WriteLine("<h3>Narrator</h3>");
                            prevScript = script;
                        }
                        WriteLine("<p>" + script.string0 + "</p>");
                        break;
                }
                break;
        }
    }
}
