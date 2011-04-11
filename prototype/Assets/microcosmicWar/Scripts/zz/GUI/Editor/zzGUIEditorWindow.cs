using UnityEngine;
using UnityEditor;

public class zzGUIEditorWindow: EditorWindow
{
    [MenuItem("Window/zz GUI")]
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
        root = null;
        if (Selection.activeTransform)
        {
            lGUISelection = Selection.activeTransform.GetComponent<zzInterfaceGUI>();
            root = findGUIRoot(lGUISelection);
            if(root)
            {
                GUIEditorHelper.selection = lGUISelection;
                showCameraRender = GUIEditorHelper.showCameraRender;
            }
        }
        Repaint();

        //_GUIEditorHelper = null;
        //if (lGUISelection)
        //{
        //    var lGUIRoot = lGUISelection.root;
        //    if (lGUIRoot)
        //    {
        //        _GUIEditorHelper = lGUIRoot.GetComponent<zzGUIEditorHelper>();
        //        if (!_GUIEditorHelper)
        //            _GUIEditorHelper = lGUIRoot.gameObject.AddComponent<zzGUIEditorHelper>();
        //    }

        //}

        //if (_GUIEditorHelper)
        //{
        //    _GUIEditorHelper.selection = lGUISelection;
        //}
    }

    //void Update()
    //{
    //    //Debug.Log(Input.GetMouseButton(0));
    //}

    //void OnInspectorUpdate()
    //{
    //    //Debug.Log("zzGUIEditorWindow.OnInspectorUpdate");
    //    //Repaint();
    //}

    //zzGUIRenderDebuger getGUIRenderDebuger(zzInterfaceGUI pGUI)
    //{
    //    var lOut = pGUI.root as zzGUIRenderDebuger;
    //    if(!lOut)
    //        lOut = pGUI.root.gameObject.AddComponent<zzGUIRenderDebuger>();
    //    return lOut;
    //}

    //zzGUIEditorHelper _GUIEditorHelper;

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

    zzGUIRenderDebuger GUIRenderDebuger
    {
        get
        {
            var lOut = root.GetComponent<zzGUIRenderDebuger>();
            if (!lOut)
                lOut = root.gameObject.AddComponent<zzGUIRenderDebuger>();
            return lOut;
        }
    }

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
            var lGUIRenderDebuger = GUIRenderDebuger;
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
            //var lSelection = _GUIEditorHelper.selection;
            //EditorGUILayout.LabelField("Name", lSelection.name);
            //EditorGUILayout.RectField("Screen Position", lSelection.screenPosition);
            //var lRenderDebuger = getGUIRenderDebuger(lSelection);

            //lGUIRenderDebuger.position = new Rect(0,0,lShowRect.width,lShowRect.height);
            //lGUIRenderDebuger.drawGUI();

            lGUIEditorHelper.useMouseEvent(Event.current);
            root.customPosition = lGUIRect;

            root.OnGUI();
            lGUIEditorHelper.OnGUI();

            GUI.EndGroup();
            lCamara.rect = new Rect(0, 0, 1, 1);

            GUI.EndScrollView();

        }
        //BeginWindows();
        //windowRect = GUI.Window(1, windowRect, doWindow, "Hi There");
        //EndWindows();

        //Debug.Log(Event.current.type);
        //EditorGUIUtility.RenderGameViewCameras(new Rect(50, 50, position.width, position.height),
        //    new Rect(30, 30, 100, 100), false, false);

        //EditorGUIUtility.AddCursorRect(new Rect(10, 10, 100, 100), MouseCursor.Link); 
        //if (Event.current.type== EventType.MouseDown)
        //{
        //    Debug.Log("zzGUIEditorWindow.OnGUI");
        //    Debug.Log(Event.current.type);

        //}
    }

    //void doWindow(int pID)
    //{
    //    GUI.DragWindow();

    //    if (_GUIEditorHelper && _GUIEditorHelper.selection)
    //    {
    //        var lCamara = Camera.main;
    //        var lPreCamaraRect = lCamara.rect;
    //        lCamara.pixelRect = windowRect;
    //        Camera.main.Render();
    //        var lSelection = _GUIEditorHelper.selection;
    //        ((zzGUIRender)lSelection.root).OnGUI();
    //        _GUIEditorHelper.OnGUI();

    //        lCamara.rect = new Rect(0,0,1,1);
    //    }
    //}
}