using UnityEngine;
using System.Collections;

public class zzGUITreeViewArea: zzGUIContainer
{
    public zzGUILibTreeInfo treeInfo;

    zzGUILibTreeView rootTreeView;

    public int imageSize = 30;
    public GUIStyle selectedStyle;
    public GUIStyle notSelectedStyle;

    public zzGUILibTreeElement selectedElement;

    void Start()
    {
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
            rootTreeView.drawGUI(ref lNewSelected);
            if (selectedElement != lNewSelected)
            {
                selectedElement = lNewSelected;
                //selectChangedEvent(selectedName);
            }
            GUILayout.EndScrollView();
        }
    }

    public zzGUIStyle ContentAndStyle = new zzGUIStyle();

    public override void impGUI(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            GUILayout.BeginArea(rect, ContentAndStyle.Content);
            drawTree();
            GUILayout.EndArea();
            return;
        }

        GUILayout.BeginArea(rect, ContentAndStyle.Content, ContentAndStyle.Style);
        drawTree();
        GUILayout.EndArea();
    }
}