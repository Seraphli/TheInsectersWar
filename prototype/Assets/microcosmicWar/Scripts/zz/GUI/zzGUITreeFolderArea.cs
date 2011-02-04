using UnityEngine;
using System.IO;
using System.Collections;

public class zzGUITreeFolderArea : zzGUIContainer
{
    public bool relativePath = true;
    public string rootFolder;
    public string selectedName;

    zzGUILibTreeFolderDraw rootTreeFolderDraw = new zzGUILibTreeFolderDraw();

    public GUIStyle selectedStyle;
    public GUIStyle notSelectedStyle;

    void Start()
    {
        if (!Application.isPlaying)
            return;
        if (selectChangedEvent == null)
            selectChangedEvent = nullSelectedEventReceiver;

        DirectoryInfo lDirectoryInfo;
        string lRootFolder;
        if (relativePath)
            lRootFolder = Application.dataPath + "/../" + rootFolder;
        else
            lRootFolder = rootFolder;

        if (Directory.Exists(lRootFolder))
            lDirectoryInfo = new DirectoryInfo(lRootFolder);
        else
            lDirectoryInfo = Directory.CreateDirectory(lRootFolder);

        rootTreeFolderDraw.directoryInfo = lDirectoryInfo;
        rootTreeFolderDraw.selectedStyle = selectedStyle;
        rootTreeFolderDraw.notSelectedStyle = notSelectedStyle;
    }

    public zzGUIStyle ContentAndStyle = new zzGUIStyle();

    Vector2 fileScroll;

    public delegate void SelectChangedEvent(string choose);

    static void nullSelectedEventReceiver(string choose){}

    SelectChangedEvent selectChangedEvent;

    public void addSelectedEventReceiver(SelectChangedEvent pReceiver)
    {
        selectChangedEvent += pReceiver;
    }

    public void drawTree()
    {
        if (Application.isPlaying)
        {
            fileScroll = GUILayout.BeginScrollView(fileScroll);
            string lNewSelectedName = selectedName;
            rootTreeFolderDraw.drawGUI(ref lNewSelectedName);
            if (selectedName != lNewSelectedName)
            {
                selectedName = lNewSelectedName;
                selectChangedEvent(selectedName);
            }
            GUILayout.EndScrollView();
        }
    }

    public override void impGUI(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            //print("_useDefaultStyle");
            GUILayout.BeginArea(rect, ContentAndStyle.Content);
            drawTree();
            GUILayout.EndArea();
            return;
        }
        //print("not _useDefaultStyle");
        GUILayout.BeginArea(rect, ContentAndStyle.Content, ContentAndStyle.Style);
        drawTree();
        GUILayout.EndArea();  
    }
}