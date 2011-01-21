using UnityEngine;
using System.Collections;

class zzGUISwitchButton : zzInterfaceGUI
{
    public bool isDown = false;

    public GUIContent buttonDownContent = new GUIContent();
    public GUIContent buttonUpContent = new GUIContent();

    public bool useDefaultStyle = true;
    public GUIStyle style;

    public override void impGUI(Rect rect)
    {
        if (_drawButton(rect))
            isDown = !isDown;
    }

    bool _drawButton(Rect rect)
    {
        if (useDefaultStyle)
            return GUI.Button(rect, getNowContent());
        return GUI.Button(rect, getNowContent(), style);
    }


    GUIContent  getNowContent()
    {
        if (isDown)
            return buttonDownContent;
        return buttonUpContent;
    }
}