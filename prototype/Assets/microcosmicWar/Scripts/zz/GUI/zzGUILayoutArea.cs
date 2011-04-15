using UnityEngine;

public class zzGUILayoutArea : zzGUIContainer
{
    public zzGUIStyle ContentAndStyle = new zzGUIStyle();
    public override void impGUI(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            //print("_useDefaultStyle");
            GUILayout.BeginArea(rect, ContentAndStyle.Content);
            impSubs();
            GUILayout.EndArea();
            return;
        }
        //print("not _useDefaultStyle");
        GUILayout.BeginArea(rect, ContentAndStyle.Content, ContentAndStyle.Style);
        impSubs();
        GUILayout.EndArea();
    }
}