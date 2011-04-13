using UnityEngine;

public abstract class zzGUIModifierBase:MonoBehaviour
{
    public abstract void modifierBegin();
    public abstract void modifierEnd();
}

public class zzGUIModifierExecutor : zzGUIContainer
{

    public override void impGUI(Rect rect)
    {
        var lSubs = getSubsByDepth();
        var lModifiers = GetComponents<zzGUIModifierBase>();
        foreach (var lModifier in lModifiers)
        {
            lModifier.modifierBegin();
        }
        impSubs();
        foreach (var lModifier in lModifiers)
        {
            lModifier.modifierEnd();
        }
    }
}