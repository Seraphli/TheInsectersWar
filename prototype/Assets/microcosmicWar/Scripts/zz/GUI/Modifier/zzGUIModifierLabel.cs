using UnityEngine;

public class zzGUIModifierLabel : zzGUIModifierBase
{
    public zzLabel label;
    public Component component;
    public string memberName;

    System.Func<string> getTextFunc;

    void Start()
    {
        var lProperty = component.GetType().GetProperty(memberName);
        getTextFunc = (System.Func<string>)System.Delegate.CreateDelegate(
             typeof(System.Func<string>), component,lProperty.GetGetMethod());
    }

    public override void modifierBegin()
    {
        label.setText(getTextFunc());
    }

    public override void modifierEnd() { }
}