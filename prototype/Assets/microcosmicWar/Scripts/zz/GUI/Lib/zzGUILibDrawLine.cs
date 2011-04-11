using UnityEngine;

/// <summary>
/// http://www.unifycommunity.com/wiki/index.php?title=DrawLine
/// </summary>
public class zzGUILibDrawLine
{
    static Texture2D _pixelTex;

    static Texture2D pixelTex
    {
        get
        {
            // Generate a single pixel texture if it doesn't exist
            if (!_pixelTex)
            {
                _pixelTex = new Texture2D(1, 1);
                _pixelTex.SetPixel(0, 0, Color.white);
                _pixelTex.Apply();
            }
            return _pixelTex;
        }
    }

    public static void DrawLine(Rect rect, Color color, float width) 
    { 
        DrawLine(
            new Vector2(rect.x, rect.y), 
            new Vector2(rect.x + rect.width, rect.y ),
            color, width);

        DrawLine(
            new Vector2(rect.x, rect.y + rect.height),
            new Vector2(rect.x + rect.width, rect.y + rect.height),
            color, width);

        DrawLine(
            new Vector2(rect.x, rect.y),
            new Vector2(rect.x, rect.y + rect.height),
            color, width);

        DrawLine(
            new Vector2(rect.x + rect.width, rect.y),
            new Vector2(rect.x + rect.width, rect.y + rect.height),
            color, width); 
    }

    public static void DrawBox(Rect pRect, Color color)
    {
        Color savedColor = GUI.color;
        GUI.color = color;

        GUI.DrawTexture(pRect, pixelTex);

        GUI.color = savedColor;
    }

    public static void DrawBox(Rect pRect, Vector2 pPivot, Color color, float pAngle)
    {
        Matrix4x4 matrix = GUI.matrix;

        Color savedColor = GUI.color;
        GUI.color = color;

        GUIUtility.RotateAroundPivot(pAngle, pPivot);

        GUI.DrawTexture(pRect, pixelTex);

        // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }

    public static void DrawBox(Vector2 pCenter,Vector2 pExtent, Color color, float pAngle)
    {
        //Matrix4x4 matrix = GUI.matrix;

        //Color savedColor = GUI.color;
        //GUI.color = color;

        //GUIUtility.RotateAroundPivot(pAngle, pCenter);

        //GUI.DrawTexture(
        //    new Rect(pCenter.x - pExtent.x,
        //        pCenter.y - pExtent.y,
        //        pExtent.x * 2f,
        //        pExtent.y * 2f),
        //    pixelTex);

        //// We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        //GUI.matrix = matrix;
        //GUI.color = savedColor;
        DrawBox(
            new Rect(pCenter.x - pExtent.x,
                pCenter.y - pExtent.y,
                pExtent.x * 2f,
                pExtent.y * 2f),
             pCenter,
             color,
             pAngle
        );
    }

    //static Texture2D lineTex;

    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        // Save the current GUI matrix, since we're going to make changes to it.
        Matrix4x4 matrix = GUI.matrix;


        // Store current GUI color, so we can switch it back later,
        // and set the GUI color to the color parameter
        Color savedColor = GUI.color;
        GUI.color = color;

        var lVectorAB = pointB - pointA;
        // Determine the angle of the line.
        float angle = Vector3.Angle(lVectorAB, Vector2.right);

        // Vector3.Angle always returns a positive number.
        // If pointB is above pointA, then angle needs to be negative.
        if (pointA.y > pointB.y) 
        { 
            angle = -angle; 
        }

        // Use ScaleAroundPivot to adjust the size of the line.
        // We could do this when we draw the texture, but by scaling it here we can use
        //  non-integer values for the width and length (such as sub 1 pixel widths).
        // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
        //  is centered on the origin at pointA.
        //GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));

        // Set the rotation for the line.
        //  The angle was calculated with pointA as the origin.
        var lBeginPos = new Vector2(pointA.x, pointA.y + width / 2f);
        GUIUtility.RotateAroundPivot(angle, lBeginPos);

        // Finally, draw the actual line.
        // We're really only drawing a 1x1 texture from pointA.
        // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
        //  render with the proper width, length, and angle.
        GUI.DrawTexture(
            new Rect(lBeginPos.x, lBeginPos.y, lVectorAB.magnitude, width),
            pixelTex);

        // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }
}