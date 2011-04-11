
using UnityEngine;
using System.Collections;


public class zzGUITransform : zzGUIContainer
{
    //public UIVector2 ofsetPosition;
    public Vector2 scale;
    public float angle;

    public override void impSubs()
    {
        if (angle != 0f)
        {
            var lPreMatrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle,new Vector2(screenPosition.x, screenPosition.y));
            drawScaledSub();
            GUI.matrix = lPreMatrix;
        }
        else
            drawScaledSub();

    }

    void drawScaledSub()
    {
        foreach (zzInterfaceGUI lSub in getSubsByDepth())
        {
            Rect lSelfRect = getPosition();
            Rect lSubRect = lSub.getPosition();
            lSubRect.x = lSubRect.x * scale.x + lSelfRect.x;
            lSubRect.y = lSubRect.y * scale.y + lSelfRect.y;
            lSubRect.width *= scale.x;
            lSubRect.height *= scale.y;
            lSub.renderGUI(lSubRect);
        }

    }

}