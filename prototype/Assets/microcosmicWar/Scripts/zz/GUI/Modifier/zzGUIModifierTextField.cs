using UnityEngine;

public class zzGUIModifierTextField : zzGUIModifierGenericProperty<string>
{
    public zzGUITextField textField;

    public override string widgetValue
    {
        get
        {
            return textField.text;
        }
        set
        {
            textField.text=value;
        }
    }
}