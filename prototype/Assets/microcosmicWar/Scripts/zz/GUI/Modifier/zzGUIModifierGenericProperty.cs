using UnityEngine;


public abstract class zzGUIModifierGenericProperty<T> : zzGUIModifierBase
{
    public Component component;
    public string memberName;

    System.Func<T> getFunc;
    System.Action<T> setFunc;

    void Start()
    {
        var lProperty = component.GetType().GetProperty(memberName);
        getFunc = (System.Func<T>)System.Delegate.CreateDelegate(
             typeof(System.Func<T>), component, lProperty.GetGetMethod());
        setFunc = (System.Action<T>)System.Delegate.CreateDelegate(
             typeof(System.Action<T>), component, lProperty.GetSetMethod());
    }

    T lastValue;

    public override void modifierBegin()
    {
        lastValue = getFunc();
        widgetValue = lastValue;
    }

    public override void modifierEnd()
    {
        if (!lastValue.Equals( widgetValue))
            setFunc(widgetValue);
    }

    public abstract T widgetValue
    {
        get;
        set;
    }
}