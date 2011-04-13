using UnityEngine;


public class zzGUITransform : zzGUIContainer
{
    //public UIVector2 ofsetPosition;
    public Vector2 scale = Vector2.one;
    public float angle;

    public bool changeColor = false;
    public Color color;

    public override void impSubs()
    {
        Color lPreColor = Color.clear;
        if (changeColor)
        {
            lPreColor = GUI.color;
            GUI.color = color;
        }

        if (angle != 0f)
        {
            var lPreMatrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle,new Vector2(screenPosition.x, screenPosition.y));
            drawScaledSub();
            GUI.matrix = lPreMatrix;
        }
        else
            drawScaledSub();

        if (changeColor)
        {
            GUI.color = lPreColor;
        }
    }

    void drawScaledSub()
    {
        Rect lSelfRect = getPosition();
        foreach (zzInterfaceGUI lSub in getSubsByDepth())
        {
            Rect lSubRect = lSub.getPosition();
            lSubRect.x = lSubRect.x * scale.x + lSelfRect.x;
            lSubRect.y = lSubRect.y * scale.y + lSelfRect.y;
            lSubRect.width *= scale.x;
            lSubRect.height *= scale.y;
            lSub.renderGUI(lSubRect);
        }

    }

}