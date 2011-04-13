using UnityEngine;
using UnityEditor;

public class zzGUIEditorWindow: EditorWindow
{
    //static void addGUI<T>(string pName) where T : zzInterfaceGUI
    //{
    //    var lGUI = Selection.activeTransform.GetComponent<zzInterfaceGUI>();
    //    var lObject = new GameObject(pName);
    //    if(lGUI is zzGUIContainer)
    //    {
    //        lObject.transform.parent = Selection.activeTransform;
    //    }
    //    else
    //    {
    //        lObject.transform.parent = Selection.activeTransform.parent;
    //    }
    //    zzInterfaceGUI lNewGUiControl= lObject.AddComponent<T>();
    //    lNewGUiControl.useRelativePosition = new zzGUIRelativeUsedInfo(true, true, true, true);
    //    lNewGUiControl.relativePosition = new Rect(0.25f, 0.25f, 0.5f, 0.5f);

    //}

    //static bool validateAddGUI()
    //{
    //    return Selection.activeTransform != null
    //        && Selection.activeTransform.GetComponent<zzInterfaceGUI>();
    //}

    //[MenuItem("Component/zz GUI/Add Window")]
    //static void menuItemAddWindow()
    //{
    //    addGUI<zzWindow>("Window");
    //}

    //[MenuItem("Component/zz GUI/Add Window",true)]
    //static bool validateMenuItemAddWindow()
    //{
    //    return validateAddGUI();
    //}



    //[MenuItem("Component/zz GUI/Add Button")]
    //static void menuItemAddWindow()
    //{
    //    addGUI<zzButton>("Button");
    //}

    //[MenuItem("Component/zz GUI/Add Button", true)]
    //static bool validateMenuItemAddButton()
    //{
    //    return validateAddGUI();
    //}

    //[MenuItem("Component/zz GUI/Add SwitchButton")]
    //static void menuItemAddWindow()
    //{
    //    addGUI<zzGUISwitchButton>("SwitchButton");
    //}

    //[MenuItem("Component/zz GUI/Add SwitchButton", true)]
    //static bool validateMenuItemAddWindow()
    //{
    //    return validateAddGUI();
    //}

    [MenuItem("Window/zz GUI Editor")]
    static void Init()
    {
        var lWindow = (zzGUIEditorWindow)EditorWindow.GetWindow(typeof(zzGUIEditorWindow), false, "zz GUI");
        lWindow.autoRepaintOnSceneChange = true;
        lWindow.wantsMouseMove = true;
    }

    zzGUIRender findGUIRoot(zzInterfaceGUI pGUI)
    {
        while (pGUI && !(pGUI is zzGUIRender))
        {
            if (pGUI.transform.parent)
                pGUI = pGUI.getParent();
            else
                pGUI = null;
        }
        return pGUI as zzGUIRender;
    }

    zzGUIRender root;

    void OnSelectionChange()
    {
        zzInterfaceGUI lGUISelection = null;
        zzGUIRender lNewRoot = null;
        if (Selection.activeTransform)
        {
            lGUISelection = Selection.activeTransform.GetComponent<zzInterfaceGUI>();
            lNewRoot = findGUIRoot(lGUISelection);
            if (lNewRoot)
            {
                root = lNewRoot;
                GUIEditorHelper.selection = lGUISelection;
                showCameraRender = GUIEditorHelper.showCameraRender;
                Repaint();
            }
        }

    }

    zzGUIEditorHelper GUIEditorHelper
    {
        get
        {
            var lOut = root.GetComponent<zzGUIEditorHelper>();
            if (!lOut)
                lOut = root.gameObject.AddComponent<zzGUIEditorHelper>();
            return lOut;
        }
    }

    //zzGUIRenderDebuger GUIRenderDebuger
    //{
    //    get
    //    {
    //        var lOut = root.GetComponent<zzGUIRenderDebuger>();
    //        if (!lOut)
    //            lOut = root.gameObject.AddComponent<zzGUIRenderDebuger>();
    //        return lOut;
    //    }
    //}

    public Rect windowRect = new Rect(20, 20, 500, 300);
    public Vector2 scrollPosition;
    public float GUIScale = 1f;
    public bool showCameraRender = false;
    void OnGUI()
    {
        //GUIScale = EditorGUI.Slider(new Rect(80,0,150,20),GUIScale, 1f, 3f);
        GUILayout.BeginHorizontal();
        GUIScale = EditorGUILayout.Slider( GUIScale, 1f, 3f);
        GUILayout.FlexibleSpace();
        showCameraRender = EditorGUILayout.Toggle("Camera Render", showCameraRender);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        var lShowRect = new Rect(10,30,position.width-20,position.height-40);
        var lCameraRect = new Rect(10, 10 , position.width - 20, position.height - 40);
        var lFullRect = new Rect(0, 0, lShowRect.width, lShowRect.height);
        var lGUIRect = new Rect(0, 0, lShowRect.width * GUIScale, lShowRect.height * GUIScale);
        //var lGroupRect = new Rect(10, 30, lShowRect.width * GUIScale, lShowRect.height * GUIScale);
        //if (_GUIEditorHelper && _GUIEditorHelper.selection)
        if(root)
        {
            var lGUIEditorHelper = GUIEditorHelper;
            scrollPosition = GUI.BeginScrollView(lShowRect, scrollPosition, lGUIRect);

            lGUIEditorHelper.windowPosition = position;
            GUI.BeginGroup(lGUIRect);

            zzGUILibDrawLine.DrawBox(lGUIRect, lGUIEditorHelper.backgroudColor);

            var lCamara = Camera.main;
            if (showCameraRender)
            {
                lCamara.pixelRect = lCameraRect;
                Camera.main.Render();
            }
            lGUIEditorHelper.showCameraRender = showCameraRender;

            lGUIEditorHelper.useMouseEvent(Event.current);
            root.customPosition = lGUIRect;
            BeginWindows();
            root.OnGUI();
            EndWindows();
            lGUIEditorHelper.OnGUI();

            GUI.EndGroup();
            lCamara.rect = new Rect(0, 0, 1, 1);

            GUI.EndScrollView();

        }
        
    }

    //void resetAllWindowID()
    //{
    //    if(root)
    //}

    //int setWindowID(zzInterfaceGUI pWidget,int pID)
    //{
    //    if(pWidget is zzGUIContainer)
    //    {
    //        if(pWidget is zzWindow)
    //            ((zzWindow)pWidget).ID = pID;
    //    }
    //}

    //int windowId

}