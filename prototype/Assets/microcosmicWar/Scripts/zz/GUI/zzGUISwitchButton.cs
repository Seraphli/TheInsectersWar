using UnityEngine;
using System.Collections;

class zzGUISwitchButton : zzInterfaceGUI
{
    public bool isOn = false;

    public GUIContent buttonOnContent = new GUIContent();
    public GUIContent buttonOffContent = new GUIContent();

    public bool useDefaultStyle = true;
    public GUIStyle style;

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
        if (useDefaultStyle)
            return GUI.Button(rect, getNowContent());
        return GUI.Button(rect, getNowContent(), style);
    }


    GUIContent  getNowContent()
    {
        if (isOn)
            return buttonOnContent;
        return buttonOffContent;
    }
}