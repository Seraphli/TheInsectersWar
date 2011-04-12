using UnityEngine;
using UnityEditor;

public class zzGUICreatorWindow : EditorWindow
{
    [MenuItem("Window/zz GUI Creator")]
    static void Init()
    {
        var lWindow = (zzGUICreatorWindow)EditorWindow.GetWindow(typeof(zzGUICreatorWindow), false, "zz GUI Creator");
    }

    class WidgetInfo
    {
        public WidgetInfo(string pName, System.Type pType)
        {
            name = pName;
            type = pType;
        }
        public string name;
        public System.Type type;
        public bool validate(Transform pTransform)
        {
            return pTransform && pTransform.GetComponent<zzInterfaceGUI>();
        }

        public zzInterfaceGUI addGUI(Transform pTransform)
        {
            var lGUI = pTransform.GetComponent<zzInterfaceGUI>();
            var lObject = new GameObject(name);
            if (lGUI is zzGUIContainer)
            {
                lObject.transform.parent = pTransform;
            }
            else
            {
                lObject.transform.parent = pTransform;
            }
            zzInterfaceGUI lNewGUiWidget = (zzInterfaceGUI)lObject.AddComponent(type);
            lNewGUiWidget.useRelativePosition = new zzGUIRelativeUsedInfo(true, true, true, true);
            lNewGUiWidget.relativePosition = new Rect(0.25f, 0.25f, 0.5f, 0.5f);
            return lNewGUiWidget;

        }

        public void drawAndCheck(Transform pTransform)
        {
            if (GUILayout.Button(name,
                GUILayout.ExpandWidth(false),
                GUILayout.ExpandWidth(false)))
                addGUI(pTransform);
        }
    }

    void OnSelectionChange()
    {
        Repaint();
    }

    WidgetInfo[] widgetInfoList;

    void OnEnable()
    {
        widgetInfoList = new WidgetInfo[] {
            widget<zzWindow>("Window"),
            widget<zzButton>("Button"),
            widget<zzGUISwitchButton>("SwitchButton"),
        };
    }

    static WidgetInfo widget<T>(string pName)
    {
        return new WidgetInfo(pName, typeof(T));
    }


    void OnGUI()
    {
        //GUIScale = EditorGUI.Slider(new Rect(80,0,150,20),GUIScale, 1f, 3f);
        GUILayout.BeginVertical();
        var lSelection = Selection.activeTransform;
        foreach (var lWidgetInfo in widgetInfoList)
        {
            if (lWidgetInfo.validate(lSelection))
                lWidgetInfo.drawAndCheck(lSelection);
        }
        GUILayout.EndVertical();
    }

}