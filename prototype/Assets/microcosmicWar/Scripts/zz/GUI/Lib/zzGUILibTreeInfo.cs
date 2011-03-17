using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zzGUILibTreeInfo:MonoBehaviour
{
    public zzGUILibTreeNode treeInfo;
}

[System.Serializable]
public class zzGUILibTreeElement
{
    public string name;
    public Texture2D image;

    //用以保存自定义数据
    public object objectData;
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

}

public class zzGUILibTreeView
{
    public GUIStyle selectedStyle;
    public GUIStyle notSelectedStyle;
    //public Rect imageSize = new Rect(0, 0, 30, 30);
    public int imageSize = 30;

    int TreeDepth = 0;
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
    }

    GUIStyle getStyle(object pSelected,object pShowed)
    {
        if (pSelected == pShowed)
            return selectedStyle;
        return notSelectedStyle;
    }

    List<zzGUILibTreeView> subViews = new List<zzGUILibTreeView>();

    public void drawGUI(ref zzGUILibTreeElement pSelected)
    {
        zzGUILibTreeElement lNewSelected = null;
        bool lNewExpanded;
        //print("TreeDepth:" + TreeDepth);
        //print(directoryInfo.Name+Folders.Count);
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(imageSize * TreeDepth);
            lNewExpanded = GUILayout.Toggle(expanded, treeNode.content,
                getStyle(pSelected, treeNode), GUILayout.Height(imageSize));
            GUILayout.EndHorizontal();
            if (expanded)
            {
                foreach (var lView in subViews)
                {
                    lView.imageSize = imageSize;
                    lView.drawGUI(ref pSelected);
                }

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
            }

        }
        GUILayout.EndVertical();

        if (lNewSelected != null)
            pSelected = lNewSelected;
        if(expanded != lNewExpanded)
        {
            pSelected = treeNode;
            expanded = lNewExpanded;
        }
    }
}