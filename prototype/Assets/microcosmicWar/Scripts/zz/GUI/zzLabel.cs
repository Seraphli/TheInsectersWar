
using UnityEngine;
using System.Collections;


class zzLabel : zzInterfaceGUI
{
    //FIXME_VAR_TYPE position= new Rect();
    public GUIContent content = new GUIContent();
    public GUIStyle style = new GUIStyle();

    public override void impGUI()
    {
        GUI.Label(getPosition(), content, style);
    }

    public override void setText(string pText)
    {
        content.text = pText;
    }
}