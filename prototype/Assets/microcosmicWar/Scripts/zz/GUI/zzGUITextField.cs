
using UnityEngine;
using System.Collections;

public class zzGUITextField : zzInterfaceGUI
{
    StringCallFunc textChangedEvent;

    public void addTextChangedReceiver(StringCallFunc pReceiver)
    {
        textChangedEvent += pReceiver;
    }

    void Start()
    {
        if (textChangedEvent == null)
            textChangedEvent = nullStringCallFunc;
    }

    public string text = string.Empty;

    [SerializeField]
    private bool useDefaultStyle = false;

    [SerializeField]
    private GUIStyle _style = new GUIStyle();

    public override void impGUI(Rect rect)
    {
        string lNewText = _drawField(rect);
        if(text!=lNewText)
        {
            text = lNewText;
            textChangedEvent(text);
        }
    }

    string _drawField(Rect rect)
    {
        if (useDefaultStyle)
            return GUI.TextField(rect, text);
        return GUI.TextField(rect, text, _style);
    }
}