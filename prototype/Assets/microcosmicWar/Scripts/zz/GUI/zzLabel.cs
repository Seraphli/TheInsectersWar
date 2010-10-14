
using UnityEngine;
using System.Collections;


class zzLabel : zzInterfaceGUI
{
    //FIXME_VAR_TYPE position= new Rect();
    //public GUIContent _content = new GUIContent();
    //public GUIStyle _style = new GUIStyle();

    public zzGUIStyle ContentAndStyle;

    public override void impGUI()
    {
        GUI.Label(getPosition(), ContentAndStyle.Content, ContentAndStyle.Style);
    }

    public override void setText(string pText)
    {
        ContentAndStyle.Content.text = pText;
    }
}