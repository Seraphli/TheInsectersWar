using UnityEngine;

public class zzGUILayoutModifierExecutor : zzGUILayoutBase
{
    public override void impGUI()
    {
        //var lSubs = getSubsByDepth();
        var lModifiers = GetComponents<zzGUIModifierBase>();
        foreach (var lModifier in lModifiers)
        {
            if (Application.isPlaying || !lModifier.onlyModifyWhenPlaying)
                lModifier.modifierBegin();
        }
        foreach (Transform lSub in transform)
        {
            var lGUILayout = lSub.GetComponent<zzGUILayoutBase>();
            if(lGUILayout)
            {
                lGUILayout.impGUI();
                break;
            }
        }
        foreach (var lModifier in lModifiers)
        {
            if (Application.isPlaying || !lModifier.onlyModifyWhenPlaying)
                lModifier.modifierEnd();
        }
    }
}