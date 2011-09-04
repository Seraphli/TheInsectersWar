using UnityEngine;

public class zzGUIFocusControl : zzInterfaceGUI
{
    public override void impGUI(Rect rect)
    {
        foreach (Transform lTransform in transform)
        {
            var lControlName = GetInstanceID().ToString();
            zzInterfaceGUI lControl = lTransform.GetComponent<zzInterfaceGUI>();
            if(lControl)
            {
                GUI.SetNextControlName(lControlName);
                lControl.renderGUI();
                GUI.FocusControl(lControlName);
                break;
            }
        }
    }

}