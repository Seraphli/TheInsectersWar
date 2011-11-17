using UnityEngine;

public class zzGUIModifierColorFromRender : zzGUIModifierBase
{
    public Renderer fromRender;

    Color preColor;

    public override void modifierBegin()
    {
        preColor = GUI.color;
        GUI.color = fromRender.material.color * preColor;
    }

    public override void modifierEnd()
    {
        GUI.color = preColor;
    }
}