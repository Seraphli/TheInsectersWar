using UnityEngine;

public class zzGUIModifierLabel : zzGUIModifierBase
{
    public zzInterfaceGUI label;
    public Component component;
    public string memberName;

    System.Func<string> getTextFunc;

    void Start()
    {
        var lProperty = component.GetType().GetProperty(memberName);
        if (lProperty.PropertyType == typeof(string))
            getTextFunc = (System.Func<string>)System.Delegate.CreateDelegate(
                 typeof(System.Func<string>), component, lProperty.GetGetMethod());
        else
            getTextFunc = () => lProperty.GetValue(component, null).ToString();
    }

    public override void modifierBegin()
    {
        label.setText(getTextFunc());
    }

    public override void modifierEnd() { }
}