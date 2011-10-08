using UnityEngine;

public class zzGUIGroupTransform : zzGUIGroup
{
    public Vector2 scale = Vector2.one;
    public float angle;

    public bool changeColor = false;
    public Color color;

    public override Rect calculatePosition()
    {
        //useRelativePosition.x = true;
        Rect lOut = new Rect();

        lOut.width = calculateWidth() * scale.x;
        lOut.height = calculateHeight() * scale.y;
        var lPosX = relativePosition.x * getParent().getWidth();
        var lPosY = relativePosition.y * getParent().getWidth();
        lOut.x = lPosX - lOut.width / 2f;
        lOut.y = lPosY - lOut.height / 2f;
        return lOut;
    }
    public override void impGUI(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            //print("_useDefaultStyle");
            GUI.BeginGroup(rect, ContentAndStyle.Content);
            drawTransform(rect);
            GUI.EndGroup();
            return;
        }
        //print("not _useDefaultStyle");
        GUI.BeginGroup(rect, ContentAndStyle.Content, ContentAndStyle.Style);
        drawTransform(rect);
        GUI.EndGroup();
    }

    public void drawTransform(Rect rect)
    {

        if (angle != 0f)
        {
            //print(name);
            zzGUILibDrawLine.DrawBox(new Rect(rect.width / 2f - 2f, rect.height / 2f - 2f, 2f, 2f), Color.red);
            //var lPreMatrix = GUI.matrix;

            //print("lPreMatrix:"+lPreMatrix);
            ////var lScreenPoint = GUIUtility.GUIToScreenPoint(new Vector2(rect.width / 2f, rect.height/2f));
            //GUIUtility.RotateAroundPivot(angle, new Vector2(rect.width / 2f, rect.height / 2f));
            //print("GUI.matrix:" + GUI.matrix);

            Matrix4x4 lPreMatrix = GUI.matrix;
            //使用GUIToScreenPoint所以要重置矩阵
            GUI.matrix = Matrix4x4.identity;
            //无父级变换的位置
            Vector2 lScreenPoint = GUIUtility.GUIToScreenPoint(new Vector2(rect.width / 2f, rect.height / 2f));
            //变换
            lScreenPoint = (Vector2)(lPreMatrix.MultiplyPoint3x4((Vector3)lScreenPoint));
            var lMat1 = Matrix4x4.TRS((Vector3)lScreenPoint, Quaternion.Euler(0f, 0f, angle), Vector3.one);
            var lMat2 = Matrix4x4.TRS((Vector3)( - lScreenPoint ), Quaternion.identity, Vector3.one);
            Matrix4x4 matrixx2 = lMat1 * lMat2;
            GUI.matrix = matrixx2 * lPreMatrix;

            impSubs();
            GUI.matrix = lPreMatrix;
        }
        else
            impSubs();
    }
    
}