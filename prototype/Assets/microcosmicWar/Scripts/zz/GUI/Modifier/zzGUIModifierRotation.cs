using UnityEngine;

public class zzGUIModifierRotation : zzGUIModifierBase
{
    public float angle;
    Matrix4x4 preMatrix = GUI.matrix;

    public override void modifierBegin()
    {
        preMatrix = GUI.matrix;
        var lModifierExecutor = GetComponent<zzGUIModifierExecutor>();
        var lScreenPosition = lModifierExecutor.screenPosition;
        GUIUtility.RotateAroundPivot(angle, new Vector2(lScreenPosition.x, lScreenPosition.y));
    }

    public override void modifierEnd()
    {
        GUI.matrix = preMatrix;
    }
}