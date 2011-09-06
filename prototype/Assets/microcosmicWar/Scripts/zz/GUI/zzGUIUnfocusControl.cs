using UnityEngine;

public class zzGUIUnfocusControl:zzInterfaceGUI
{
    void Awake()
    {
        depth = int.MaxValue;
    }

    public override void impGUI(Rect rect)
    {
        var lControlName = GetInstanceID().ToString();
        GUI.SetNextControlName(lControlName);
        GUI.FocusControl(lControlName);
    }
}