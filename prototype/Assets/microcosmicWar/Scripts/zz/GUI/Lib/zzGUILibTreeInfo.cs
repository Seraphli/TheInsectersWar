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
    public Object objectData;

    GUIContent _content;

    public Dictionary<string, string> stringInfo;

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
    public Texture2D expandedImage;
    public zzGUILibTreeElement[] elements = new zzGUILibTreeElement[0];
    public zzGUILibTreeNode[] nodes = new zzGUILibTreeNode[0];

    public bool changed = false;

    public void applyChange() 
    { 
        //告知绘图部分,节点内容已改变
        changed = true; 
    }

    public void setData(zzGUILibTreeNode pData)
    {
        elements = pData.elements;
        nodes = pData.nodes;
        applyChange();
    }

    GUIContent _expandedNodeContent;

    public GUIContent expandedNodeContent
    {
        get
        {
            if (_expandedNodeContent == null)
            {
                _expandedNodeContent = new GUIContent(name, expandedImage);
            }
            return _expandedNodeContent;
        }
    }

    public GUIContent collapsedNodeContent
    {
        get
        {
            return content;

        }
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSelected"></param>
    /// <returns>是否点击</returns>
    bool drawSelfNode(ref zzGUILibTreeElement pSelected)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(imageSize * TreeDepth);
        bool lNewExpanded = GUILayout.Toggle(
            expanded,
            expanded ? treeNode.expandedNodeContent : treeNode.collapsedNodeContent,
            getStyle(pSelected, treeNode),
            GUILayout.Height(imageSize));
        GUILayout.EndHorizontal();

        if (expanded != lNewExpanded)
        {
            pSelected = treeNode;
            expanded = lNewExpanded;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSelected"></param>
    /// <returns>是否点击</returns>
    bool drawSubElement(ref zzGUILibTreeElement pSelected)
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
        {
            pSelected = lNewSelected;
            return true;
        }
        return false;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSelected"></param>
    /// <returns>是否点击</returns>
    public bool drawGUI(ref zzGUILibTreeElement pSelected)
    {

        if (treeNode.changed)
            setTreeNode(treeNode);

        //zzGUILibTreeElement lNewSelected = null;

        //print("TreeDepth:" + TreeDepth);
        //print(directoryInfo.Name+Folders.Count);
        bool lIsClick = false;
        GUILayout.BeginVertical();
        {
            foreach (var lView in subViews)
            {
                //执行drawSelfNode时会改变expanded,所以先将其保存
                bool lPreExpanded = lView.expanded;
                lView.imageSize = imageSize;
                lIsClick |= lView.drawSelfNode(ref pSelected);
                if (lPreExpanded)
                    lIsClick|=lView.drawGUI(ref pSelected);
            }
            lIsClick |= drawSubElement(ref pSelected);
        }
        GUILayout.EndVertical();
        return lIsClick;
    }
}

public class zzGUILibListView
{
    [System.Serializable]
    public class TitleInfo
    {
        public string name;
        public string showName;
        public float width;
    }
    public GUIStyle selectedStyle;
    public GUIStyle notSelectedStyle;
    public GUIStyle titleStyle;

    zzGUILibTreeNode treeNode;

    public void setTreeNode(zzGUILibTreeNode pTreeNode)
    {
        treeNode = pTreeNode;
    }

    //public string[] titleList;

    //public float[] titleWidthList;
    public TitleInfo[] titleList;

    bool drawSubElement(ref zzGUILibTreeElement pSelected)
    {
        return false;
    }

    GUIStyle getStyle(object pSelected, object pShowed)
    {
        if (pSelected == pShowed)
            return selectedStyle;
        return notSelectedStyle;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pSelected"></param>
    /// <returns>是否点击</returns>
    public bool drawGUI(ref zzGUILibTreeElement pSelected)
    {

        //if (treeNode.changed)
        //    setTreeNode(treeNode);
        bool lOut = false;
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                foreach (var lTitleInfo in titleList)
                {
                    GUILayout.Label(lTitleInfo.showName, titleStyle,
                        GUILayout.Width(lTitleInfo.width));
                }
            }
            GUILayout.EndHorizontal();
            foreach (var lElement in treeNode.elements)
            {
                var lStyle = getStyle(pSelected, lElement);
                var lShowData = lElement.stringInfo;
                bool lIsClick = false;
                GUILayout.BeginHorizontal();
                {
                    if (lShowData == null)
                    {
                        foreach (var lTitleInfo in titleList)
                            lIsClick |= GUILayout.Button("", lStyle, GUILayout.Width(lTitleInfo.width));
                    }
                    else
                    {
                        foreach (var lTitleInfo in titleList)
                        {
                            string lData;
                            if (lShowData.ContainsKey(lTitleInfo.name))
                                lData = lShowData[lTitleInfo.name];
                            else
                                lData = "";
                            lIsClick |= GUILayout.Button(lData, lStyle, GUILayout.Width(lTitleInfo.width));
                        }
                    }
                }
                if (lIsClick)
                {
                    pSelected = lElement;
                    lOut = true;
                }
                GUILayout.EndHorizontal();
            }

        }
        GUILayout.EndVertical();

        return false;
    }

}