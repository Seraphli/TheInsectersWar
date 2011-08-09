using UnityEngine;

public class zzGUIModifierColor : zzGUIModifierBase
{
    public Color color;

    Color preColor;

    public override void modifierBegin()
    {
        preColor = GUI.color;
        GUI.color = color * preColor;
    }

    public override void modifierEnd()
    {
        GUI.color = preColor;
    }
}