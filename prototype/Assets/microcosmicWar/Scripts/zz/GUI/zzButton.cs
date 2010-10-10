
using UnityEngine;
using System.Collections;


public class zzButton : zzInterfaceGUI
{
    //FIXME_VAR_TYPE position= new Rect();
    public GUIContent content = new GUIContent();
    public bool useDefaultStyle = false;
    public GUIStyle style = new GUIStyle();
    public zzInterfaceGUI.GUICallFunc clickCall = nullGUICallback;

    public override void impGUI()
    {
        if (_drawButton())
            clickCall(this);
    }

    bool _drawButton()
    {
        if (useDefaultStyle)
            return GUI.Button(getPosition(), content);
        return GUI.Button(getPosition(), content, style);
    }

    public override void setText(string pText)
    {
        content.text = pText;
    }

    public virtual void setClickCall(zzInterfaceGUI.GUICallFunc pCall)
    {
        clickCall = pCall;
    }
}
