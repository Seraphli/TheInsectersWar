
using UnityEngine;
using System.Collections;


public class zzGUI
{
    static zzInterfaceGUI _root;
    static zzWindow _focusedWindow;

    public static void _setFocusedWindow(zzWindow pWindow)
    {
        _focusedWindow = pWindow;
    }

    public static void _setRoot(zzInterfaceGUI pRoot)
    {
        var lRootPosition = pRoot.getPosition();
        originOfCoordinates = new Vector2(lRootPosition.x, lRootPosition.y);
        _root = pRoot;
    }

    public static zzInterfaceGUI root
    {
        get { return _root; }
    }

    public static zzWindow focusedWindow
    {
        get { return _focusedWindow; }
    }

    public static Vector2 originOfCoordinates;
}

[ExecuteInEditMode]
public class zzGUIRender : zzGUIContainer
{
    public void OnGUI()
    {
        cursorOnControl = null;
        calculateAndSetPosition();
        originOfCoordinates = new Vector2(position.x, position.y);
        zzGUI._setRoot(this);
        _renderGUI(position);
        _useCustomPosition = false;
    }

    bool _useCustomPosition = false;

    Rect _customPosition;

    public Rect customPosition
    {
        set
        {
            _useCustomPosition = true;
            _customPosition = value;
        }
    }

    //public override float calculateWidth()
    //{
    //    return Screen.width;
    //}

    //public override float calculateHeight()
    //{
    //    return Screen.height;
    //}

    public override Rect calculatePosition()
    {
        if (_useCustomPosition)
            return _customPosition;
        return new Rect(0f, 0f, Screen.width, Screen.height);
    }

    public zzInterfaceGUI cursorOnControl;
}