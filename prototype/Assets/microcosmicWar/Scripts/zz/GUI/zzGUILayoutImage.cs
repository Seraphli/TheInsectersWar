using UnityEngine;

public class zzGUILayoutImage : zzGUILayoutBase
{
    public Texture image;
    public zzGUIStyle ContentAndStyle = new zzGUIStyle();
    public override void impGUI()
    {
        //GUILayout.DrawTexture
        if (ContentAndStyle.UseDefaultStyle)
            GUILayout.Box(image);
        else
            GUILayout.Box(image, ContentAndStyle.Style);
    }
}