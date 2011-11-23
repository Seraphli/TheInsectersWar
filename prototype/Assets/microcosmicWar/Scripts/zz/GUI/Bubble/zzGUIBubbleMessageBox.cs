using UnityEngine;

public class zzGUIBubbleMessageBox : zzInterfaceGUI
{
    public string text = string.Empty;
    //public Transform boxPosition;
    public zzGUIBubbleComputeRect bubbleCompute;
    public Color color = Color.white;
    public float timestamp;
    //public Rect rect;
    public float fadeOutBeginPos = 5f;
    public float fadeOutEndPos = 7f;
    public GUIStyle style = new GUIStyle();

    public static bool canChangeLayout
    {
        get
        {
            return Event.current.type != EventType.Layout
                        && Event.current.type != EventType.Repaint;
        }
    }

    public void drawLayoutLabel()
    {
        var lTime = Time.realtimeSinceStartup;
        var lColor = color;
        lColor.a *= 1f - Mathf.InverseLerp(
            timestamp + fadeOutBeginPos,
            timestamp + fadeOutEndPos,
            lTime);
        GUI.color = lColor;
        GUILayout.Label(text, style);
        if (lTime - timestamp > fadeOutEndPos
            && canChangeLayout)
            bubbleCompute.bubblePosition = null;
    }

    public void drawLayoutMessage()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                drawLayoutLabel();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    //public void drawMessage()
    //{
    //    if (boxPosition)
    //    {
    //        var lRect = rect;
    //        var lScreenPoint = Camera.main.WorldToScreenPoint(boxPosition.position);
    //        lRect.x += lScreenPoint.x;
    //        lRect.y += Screen.height - lScreenPoint.y;
    //    }
    //    else
    //        Destroy(gameObject);
    //}

    public override void impGUI(Rect pRect)
    {
        GUILayout.BeginArea(pRect);
        drawLayoutMessage();
        GUILayout.EndArea();
    }
}