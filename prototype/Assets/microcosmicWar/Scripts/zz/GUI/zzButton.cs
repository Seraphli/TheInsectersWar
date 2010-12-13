
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

    public override void impGUI(Rect rect)
    {
        if (_drawButton(rect))
            clickCall(this);
    }

    bool _drawButton(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
            return GUI.Button(rect, ContentAndStyle.Content);
        return GUI.Button(rect, ContentAndStyle.Content, ContentAndStyle.Style);
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
