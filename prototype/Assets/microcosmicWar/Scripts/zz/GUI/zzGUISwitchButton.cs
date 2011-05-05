using UnityEngine;
using System.Collections;

public class zzGUISwitchButton : zzInterfaceGUI
{
    [SerializeField]
    bool _isOn = false;

    public bool isOn
    {
        get
        {
            return _isOn;
        }
        set
        {
            if (_isOn != value)
                switchButton();
        }
    }

    //public GUIContent buttonOnContent = new GUIContent();
    //public GUIContent buttonOffContent = new GUIContent();

    public zzGUIStyle buttonOnContentAndStyle = new zzGUIStyle();
    public zzGUIStyle buttonOffContentAndStyle = new zzGUIStyle();

    //public bool useDefaultStyle = true;
    //public GUIStyle style;

    public void setOn()
    {
        if (!_isOn)
            switchButton();
    }

    public void setOff()
    {
        if (_isOn)
            switchButton();
    }

    public delegate void SwitchEvent(bool lIsOn);

    public  void nullSwitchEvent(bool lIsOn){}

    SwitchEvent switchEvent;

    void Start()
    {
        if (switchEvent == null)
            switchEvent = nullSwitchEvent;
    }

    public void addSwitchEventReceiver(SwitchEvent pReceiver)
    {
        switchEvent += pReceiver;
    }

    public void addClickEventReceiver(VoidCallFunc pReceiver)
    {
        switchEvent += (x) => pReceiver();
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
        _isOn = !_isOn;
        switchEvent(_isOn);
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
        if (_isOn)
            return buttonOnContentAndStyle;
        return buttonOffContentAndStyle;
    }
}