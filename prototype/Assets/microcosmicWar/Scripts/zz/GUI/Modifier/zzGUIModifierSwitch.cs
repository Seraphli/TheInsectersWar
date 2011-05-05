using UnityEngine;

public class zzGUIModifierSwitch : zzGUIModifierGenericProperty<bool>
{
    public zzGUISwitchButton switchButton;
    //public Component component;
    //public string memberName;

    //System.Func<bool> getFunc;
    //System.Action<bool> setFunc;

    //void Start()
    //{
    //    var lProperty = component.GetType().GetProperty(memberName);
    //    getFunc = (System.Func<bool>)System.Delegate.CreateDelegate(
    //         typeof(System.Func<bool>), component, lProperty.GetGetMethod());
    //    setFunc = (System.Action<bool>)System.Delegate.CreateDelegate(
    //         typeof(System.Action<bool>), component, lProperty.GetSetMethod());
    //}

    //bool lastValue;

    //public override void modifierBegin()
    //{
    //    lastValue = getFunc();
    //    switchButton.isOn = lastValue;
    //}

    //public override void modifierEnd() 
    //{
    //    if (lastValue != switchButton.isOn)
    //        setFunc(switchButton.isOn);
    //}
    public override bool widgetValue
    {
        get
        {
            return switchButton.isOn;
        }
        set
        {
            switchButton.isOn = value;
        }
    }
}