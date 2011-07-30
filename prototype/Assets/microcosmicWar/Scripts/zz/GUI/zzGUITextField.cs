
using UnityEngine;
using System.Collections;

public class zzGUITextField : zzInterfaceGUI
{
    StringCallFunc textChangedEvent;
    System.Action enterKeyEvent;

    public void addEnterKeyEventReceiver(System.Action pReceiver)
    {
        enterKeyEvent += pReceiver;
    }

    public void addTextChangedReceiver(StringCallFunc pReceiver)
    {
        textChangedEvent += pReceiver;
    }

    void Start()
    {
        enabled = false;
        if (textChangedEvent == null)
            textChangedEvent = nullStringCallFunc;
        if (enterKeyEvent == null)
            enterKeyEvent = nullVoidCallFunc;
    }

    public string text = string.Empty;

    [SerializeField]
    private bool useDefaultStyle = false;

    [SerializeField]
    private GUIStyle _style = new GUIStyle();

    public override void impGUI(Rect rect)
    {
        enabled |= (Event.current.type == EventType.KeyDown
            && Event.current.character == '\n');
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

    public override void setText(string pText)
    {
        if (text != pText)
        {
            text = pText;
            textChangedEvent(text);
        }
    }

    void Update()
    {
        enterKeyEvent();
        enabled = false;
    }
}