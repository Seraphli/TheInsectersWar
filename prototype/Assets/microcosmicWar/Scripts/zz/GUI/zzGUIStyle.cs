
using UnityEngine;
using System.Collections;

[System.Serializable]
public class zzGUIStyle
{
    [SerializeField]
    private GUIContent _content = new GUIContent();

    public GUIContent Content
    {
        get { return _content; }
        set { _content = value; }
    }

    [SerializeField]
    private bool _useDefaultStyle = false;

    public bool UseDefaultStyle
    {
        get { return _useDefaultStyle; }
        set { _useDefaultStyle = value; }
    }

    [SerializeField]
    private GUIStyle _style = new GUIStyle();

    public GUIStyle Style
    {
        get { return _style; }
        set { _style = value; }
    }

}