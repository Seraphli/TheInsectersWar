using UnityEngine;
using System.Collections;

public class zzGUIToggle : zzInterfaceGUI
{
    public bool isOn = false;
    public zzGUIStyle contentAndStyle = new zzGUIStyle();


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
        var lNewValue = _drawButton(rect,isOn);
        if (lNewValue != isOn)
            return true;
        return false;
    }

    bool _drawButton( Rect rect,bool pIsOn)
    {
        if (contentAndStyle.UseDefaultStyle)
            return GUI.Toggle(rect, pIsOn, contentAndStyle.Content);
        return GUI.Toggle(rect, pIsOn, contentAndStyle.Content, contentAndStyle.Style);
    }

}