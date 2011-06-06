using UnityEngine;
using System.Collections;

public class zzGUIToolbar : zzInterfaceGUI
{
    public GUIContent[] contents = new GUIContent[0];

    public int _selected;

    public int selected
    {
        get { return _selected; }
        set
        {
            if (value != _selected)
            {
                _selected = value;
                selectedChangeEvent(_selected);
            }
        }
    }
    public bool useDefaultStyle = true;
    public GUIStyle style;

    public delegate void SelectedChangeEvent(int pSelected);
    SelectedChangeEvent selectedChangeEvent;

    public void addSelectedChangeReceiver(SelectedChangeEvent pReceiver)
    {
        selectedChangeEvent += pReceiver;
    }

    public override void impGUI(Rect rect)
    {
        selected = _drawToolbar(rect);
    }

    public int _drawToolbar(Rect rect)
    {
        if (useDefaultStyle)
            return GUI.Toolbar(rect, _selected, contents);

        return GUI.Toolbar(rect, _selected, contents, style);

    }
}