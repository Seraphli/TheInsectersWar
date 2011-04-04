﻿using UnityEngine;
using System.Collections;

public class zzGUIToolbar : zzInterfaceGUI
{
    public GUIContent[] contents = new GUIContent[0];
    public int selected;
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
        int lNewSelected = _drawToolbar(rect);
        if(lNewSelected!=selected)
        {
            selected = lNewSelected;
            selectedChangeEvent(selected);
        }
    }

    public int _drawToolbar(Rect rect)
    {
        if (useDefaultStyle)
            return GUI.Toolbar(rect, selected, contents);

        return GUI.Toolbar(rect, selected, contents, style);

    }
}