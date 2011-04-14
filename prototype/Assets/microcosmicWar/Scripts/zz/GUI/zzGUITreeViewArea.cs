using UnityEngine;
using System.Collections;

public class zzGUITreeViewArea: zzGUIContainer
{
    StringCallFunc elementClickedEvent;
    StringCallFunc nodeClickedEvent;
    System.Action<Object> elementClickedObjectEvent;

    static void nullClickedObjectEvent(Object p){}

    public void addElementClickedEvent(StringCallFunc pReceiver)
    {
        elementClickedEvent += pReceiver;
    }

    public void addElementClickedObjectEvent(System.Action<Object> pReceiver)
    {
        elementClickedObjectEvent += pReceiver;
    }

    public void addNodeClickedEvent(StringCallFunc pReceiver)
    {
        nodeClickedEvent += pReceiver;
    }

    public zzGUILibTreeInfo treeInfo;

    zzGUILibTreeView rootTreeView;

    public int imageSize = 30;
    public GUIStyle selectedStyle;
    public GUIStyle notSelectedStyle;

    public zzGUILibTreeElement selectedElement;

    void Start()
    {
        if (elementClickedEvent == null)
            elementClickedEvent = nullStringCallFunc;
        if (nodeClickedEvent == null)
            nodeClickedEvent = nullStringCallFunc;
        if (elementClickedObjectEvent == null)
            elementClickedObjectEvent = nullClickedObjectEvent;
        rootTreeView = new zzGUILibTreeView();
        rootTreeView.selectedStyle = selectedStyle;
        rootTreeView.notSelectedStyle = notSelectedStyle;
        rootTreeView.setTreeNode(treeInfo.treeInfo);
    }

    Vector2 viewScroll;
    public void drawTree()
    {
        if (Application.isPlaying)
        {
            viewScroll = GUILayout.BeginScrollView(viewScroll);
            var lNewSelected = selectedElement;
            rootTreeView.imageSize = imageSize;

            if (rootTreeView.drawGUI(ref lNewSelected))
            {
                selectedElement = lNewSelected;
                if (lNewSelected is zzGUILibTreeNode)
                    nodeClickedEvent(selectedElement.stringData);
                else// (lNewSelected is zzGUILibTreeElement)
                {
                    elementClickedEvent(selectedElement.stringData);
                    elementClickedObjectEvent(selectedElement.objectData);
                }
            }
            GUILayout.EndScrollView();
        }
    }

    public zzGUIStyle ContentAndStyle = new zzGUIStyle();

    public override void impGUI(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            //GUILayout.BeginArea(rect, ContentAndStyle.Content);
            drawTree();
            //GUILayout.EndArea();
            return;
        }

        GUILayout.BeginArea(rect, ContentAndStyle.Content, ContentAndStyle.Style);
        drawTree();
        GUILayout.EndArea();
    }
}