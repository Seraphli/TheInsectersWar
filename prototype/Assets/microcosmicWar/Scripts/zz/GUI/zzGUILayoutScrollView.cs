using UnityEngine;

public class zzGUILayoutScrollView: zzGUIContainer
{
    public bool _allUseDefault = true;

    [SerializeField]
    bool _useDefaultStyle = true;

    public bool useDefaultStyle
    {
        get { return _useDefaultStyle; }
        set { _useDefaultStyle = value; }
    }

    public bool alwaysShowHorizontal;
    public GUIStyle horizontalScrollbarStyle;

    public bool alwaysShowVertical;
    public GUIStyle verticalScrollbarStyle;

    public bool _useDefaultBackgroundStyle = true;

    //public bool useDefaultBackgroundStyle
    //{
    //    get { return _useDefaultBackgroundStyle; }
    //    set { _useDefaultBackgroundStyle = value; }
    //}

    public GUIStyle background;
    public Vector2 viewScroll;

    public void toTop()
    {
        viewScroll.y = 0f;
    }

    public void toBottom()
    {
        viewScroll.y = float.MaxValue;
    }

    public void toLeft()
    {
        viewScroll.x = 0;
    }

    public void toRight()
    {
        viewScroll.x = float.MaxValue;
    }

    public override void impGUI(Rect rect)
    {
        if (_allUseDefault)
            viewScroll = GUILayout.BeginScrollView(viewScroll);
        else if (_useDefaultStyle)
        {
            viewScroll = GUILayout.BeginScrollView(
                viewScroll,
                alwaysShowHorizontal,
                alwaysShowVertical);
        }
        else
        {
            if (_useDefaultBackgroundStyle)
                viewScroll = GUILayout.BeginScrollView(
                    viewScroll, 
                    alwaysShowHorizontal,
                    alwaysShowVertical,
                    horizontalScrollbarStyle,
                    verticalScrollbarStyle);
            else
                viewScroll = GUILayout.BeginScrollView(
                    viewScroll,
                    alwaysShowHorizontal,
                    alwaysShowVertical,
                    horizontalScrollbarStyle,
                    verticalScrollbarStyle,
                    background);
        }
        impSubs();
        GUILayout.EndScrollView();
    }
}