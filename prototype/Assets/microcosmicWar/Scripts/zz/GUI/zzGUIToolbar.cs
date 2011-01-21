using UnityEngine;
using System.Collections;

public class zzGUIToolbar : zzInterfaceGUI
{
    public GUIContent[] contents = new GUIContent[0];
    public int selected;
    public bool useDefaultStyle = true;
    public GUIStyle style;

    public override void impGUI(Rect rect)
    {
        if (useDefaultStyle)
            selected = GUI.Toolbar(rect, selected, contents);
        else
            selected = GUI.Toolbar(rect, selected, contents, style);
    }
}