using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zzGUILibTreeInfo : MonoBehaviour
{
    public zzGUILibTreeNode treeInfo;
}

[System.Serializable]
public class zzGUILibTreeElement
{
    public string name;
    public Texture2D image;

    //用以保存自定义数据
    public string stringData;
    public Component componentData;

    GUIContent _content;

    public GUIContent content
    {
        get
        {
            if(_content==null)
            {
                _content = new GUIContent(name, image);
            }
            return _content;
        }
    }
}

[System.Serializable]
public class zzGUILibTreeNode : zzGUILibTreeElement
{
    public zzGUILibTreeElement[] elements = new zzGUILibTreeElement[0];
    public zzGUILibTreeNode[] nodes = new zzGUILibTreeNode[0];

    public bool changed = false;

    public void applyChange() 
    { 
        changed = true; 
    }

    public void setData(zzGUILibTreeNode pData)
    {
        elements = pData.elements;
        nodes = pData.nodes;
        applyChange();
    }
}

public class zzGUILibTreeView
{
    public GUIStyle selectedStyle;
    public GUIStyle notSelectedStyle;
    //public Rect imageSize = new Rect(0, 0, 30, 30);
    public int imageSize = 30;

    //为-1时是根节点,不显示
    int TreeDepth = -1;
    bool expanded = false;
    zzGUILibTreeNode treeNode;

    public void setTreeNode(zzGUILibTreeNode pTreeNode)
    {
        treeNode = pTreeNode;
        var lSubTreeNode = pTreeNode.nodes;
        subViews = new List<zzGUILibTreeView>(lSubTreeNode.Length);
        for (int i = 0; i < lSubTreeNode.Length; ++i)
        {
            var lGUITreeView = new zzGUILibTreeView();
            lGUITreeView.TreeDepth = TreeDepth + 1;
            lGUITreeView.selectedStyle = selectedStyle;
            lGUITreeView.notSelectedStyle = notSelectedStyle;
            lGUITreeView.setTreeNode(lSubTreeNode[i]);
            subViews.Add(lGUITreeView);
        }
        pTreeNode.changed = false;
    }

    GUIStyle getStyle(object pSelected,object pShowed)
    {
        if (pSelected == pShowed)
            return selectedStyle;
        return notSelectedStyle;
    }

    List<zzGUILibTreeView> subViews = new List<zzGUILibTreeView>();

    void drawSelfNode(ref zzGUILibTreeElement pSelected)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(imageSize * TreeDepth);
        bool lNewExpanded = GUILayout.Toggle(expanded, treeNode.content,
            getStyle(pSelected, treeNode), GUILayout.Height(imageSize));
        GUILayout.EndHorizontal();

        if (expanded != lNewExpanded)
        {
            pSelected = treeNode;
            expanded = lNewExpanded;
        }
    }

    void drawSubElement(ref zzGUILibTreeElement pSelected)
    {
        zzGUILibTreeElement lNewSelected = null;
        foreach (var lElement in treeNode.elements)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(imageSize * (TreeDepth + 1));
            if (GUILayout.Button(lElement.content,
                    getStyle(pSelected, lElement),
                    GUILayout.Height(imageSize))
                )
                lNewSelected = lElement;
            GUILayout.EndHorizontal();
        }

        if (lNewSelected != null)
            pSelected = lNewSelected;

    }

    public void drawGUI(ref zzGUILibTreeElement pSelected)
    {

        if (treeNode.changed)
            setTreeNode(treeNode);

        zzGUILibTreeElement lNewSelected = null;
        ;
        //print("TreeDepth:" + TreeDepth);
        //print(directoryInfo.Name+Folders.Count);
        GUILayout.BeginVertical();
        {
            foreach (var lView in subViews)
            {
                //执行drawSelfNode时会改变expanded,所以先将其保存
                bool lPreExpanded = lView.expanded;
                lView.imageSize = imageSize;
                lView.drawSelfNode(ref pSelected);
                if (lPreExpanded)
                    lView.drawGUI(ref pSelected);
            }
            drawSubElement(ref pSelected);
        }
        GUILayout.EndVertical();
    }
}