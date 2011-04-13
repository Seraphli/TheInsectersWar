
using UnityEngine;
using System.Collections;


public class zzLabel : zzInterfaceGUI
{
    //FIXME_VAR_TYPE position= new Rect();
    //public GUIContent _content = new GUIContent();
    //public GUIStyle _style = new GUIStyle();

    public zzGUIStyle ContentAndStyle = new zzGUIStyle();

    public override void impGUI(Rect rect)
    {
        GUI.Label(rect, ContentAndStyle.Content, ContentAndStyle.Style);
    }

    public override void setText(string pText)
    {
        ContentAndStyle.Content.text = pText;
    }
}