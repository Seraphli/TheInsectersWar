
using UnityEngine;
using System.Collections;


public class zzButton : zzInterfaceGUI
{
    //FIXME_VAR_TYPE position= new Rect();
    //public GUIContent _content = new GUIContent();
    //public bool _useDefaultStyle = false;
    //public GUIStyle _style = new GUIStyle();
    public zzGUIStyle ContentAndStyle = new zzGUIStyle();
    public zzInterfaceGUI.GUICallFunc clickCall = nullGUICallback;

    public override void impGUI()
    {
        if (_drawButton())
            clickCall(this);
    }

    bool _drawButton()
    {
        if (ContentAndStyle.UseDefaultStyle)
            return GUI.Button(getPosition(), ContentAndStyle.Content);
        return GUI.Button(getPosition(), ContentAndStyle.Content, ContentAndStyle.Style);
    }

    public override void setText(string pText)
    {
        ContentAndStyle.Content.text = pText;
    }

    public virtual void setClickCall(zzInterfaceGUI.GUICallFunc pCall)
    {
        clickCall = pCall;
    }
}
