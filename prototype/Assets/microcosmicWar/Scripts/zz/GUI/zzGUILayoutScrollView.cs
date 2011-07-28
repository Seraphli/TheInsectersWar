using UnityEngine;

public class zzGUILayoutScrollView: zzGUIContainer
{
    public bool allUseDefault = true;
    public bool useDefaultStyle = true;

    public bool alwaysShowHorizontal;
    public GUIStyle horizontalScrollbarStyle;

    public bool alwaysShowVertical;
    public GUIStyle verticalScrollbarStyle;

    public bool useDefaultBackgroundStyle = true;
    public GUIStyle background;
    public Vector2 viewScroll;

    public override void impGUI(Rect rect)
    {
        if (allUseDefault)
            viewScroll = GUILayout.BeginScrollView(viewScroll);
        else if (useDefaultStyle)
        {
            viewScroll = GUILayout.BeginScrollView(
                viewScroll,
                alwaysShowHorizontal,
                alwaysShowVertical);
        }
        else
        {
            if (useDefaultBackgroundStyle)
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