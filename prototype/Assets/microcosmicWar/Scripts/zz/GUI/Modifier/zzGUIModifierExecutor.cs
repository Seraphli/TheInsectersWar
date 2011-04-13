using UnityEngine;

public abstract class zzGUIModifierBase:MonoBehaviour
{
    public bool onlyModifyWhenPlaying = false;
    public abstract void modifierBegin();
    public abstract void modifierEnd();
}

public class zzGUIModifierExecutor : zzGUIContainer
{

    public override void impGUI(Rect rect)
    {
        //var lSubs = getSubsByDepth();
        var lModifiers = GetComponents<zzGUIModifierBase>();
        foreach (var lModifier in lModifiers)
        {
            if (Application.isPlaying || !lModifier.onlyModifyWhenPlaying)
                lModifier.modifierBegin();
        }
        impSubs();
        foreach (var lModifier in lModifiers)
        {
            if (Application.isPlaying || !lModifier.onlyModifyWhenPlaying)
                lModifier.modifierEnd();
        }
    }
}