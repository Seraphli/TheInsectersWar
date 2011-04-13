using UnityEngine;
using System.Collections;

public class zzGUISwitchButton : zzInterfaceGUI
{
    public bool isOn = false;

    //public GUIContent buttonOnContent = new GUIContent();
    //public GUIContent buttonOffContent = new GUIContent();

    public zzGUIStyle buttonOnContentAndStyle = new zzGUIStyle();
    public zzGUIStyle buttonOffContentAndStyle = new zzGUIStyle();

    //public bool useDefaultStyle = true;
    //public GUIStyle style;

    public void setOn()
    {
        if (!isOn)
            switchButton();
    }

    public void setOff()
    {
        if (isOn)
            switchButton();
    }

    public delegate void SwitchEvent(bool lIsOn);

    SwitchEvent switchEvent;

    public void addSwitchEventReceiver(SwitchEvent pReceiver)
    {
        switchEvent += pReceiver;
    }

    public override void impGUI(Rect rect)
    {
        if (_drawButton(rect))
        {
            switchButton();
        }
    }

    public void switchButton()
    {
        isOn = !isOn;
        switchEvent(isOn);
    }

    bool _drawButton(Rect rect)
    {
        return _drawButton(getNowContentAndStyle(), rect);
    }

    bool _drawButton(zzGUIStyle pStyle, Rect rect)
    {
        if (pStyle.UseDefaultStyle)
            return GUI.Button(rect, pStyle.Content);
        return GUI.Button(rect, pStyle.Content, pStyle.Style);
    }


    zzGUIStyle getNowContentAndStyle()
    {
        if (isOn)
            return buttonOnContentAndStyle;
        return buttonOffContentAndStyle;
    }
}