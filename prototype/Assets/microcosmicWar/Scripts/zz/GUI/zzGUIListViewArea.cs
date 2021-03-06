﻿using UnityEngine;

public class zzGUIListViewArea : zzGUIContainer
{
    public zzGUILibTreeInfo treeInfo;
    public zzGUILibTreeElementEvent eventObject;

    zzGUILibListView listView = new zzGUILibListView();

    public GUIStyle selectedStyle;
    public GUIStyle notSelectedStyle;
    public GUIStyle titleStyle;

    public zzGUILibTreeElement selectedElement;

    public zzGUIStyle ContentAndStyle = new zzGUIStyle();

    public zzGUILibListView.TitleInfo[] titleList;
    Vector2 viewScroll;

    void Start()
    {
        if(!eventObject)
        {
            eventObject = GetComponent<zzGUILibTreeElementEvent>();
            if(!eventObject)
            {
                eventObject = gameObject.AddComponent<zzGUILibTreeElementEvent>();
            }
        }
    }

    public override void impGUI(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            GUILayout.BeginArea(rect, ContentAndStyle.Content);
            drawList();
            GUILayout.EndArea();
            return;
        }

        GUILayout.BeginArea(rect, ContentAndStyle.Content, ContentAndStyle.Style);
        drawList();
        GUILayout.EndArea();
    }

    void drawList()
    {
        viewScroll = GUILayout.BeginScrollView(viewScroll);
        {
            listView.notSelectedStyle = notSelectedStyle;
            listView.selectedStyle = selectedStyle;
            listView.titleStyle = titleStyle;
            listView.titleList = titleList;
            listView.setTreeNode(treeInfo.treeInfo);
            if(listView.drawGUI(ref selectedElement))
            {
                eventObject.callElementClickedEvent(selectedElement.stringData);
                eventObject.callElementClickedObjectEvent(selectedElement.objectData);
            }
        }
        GUILayout.EndScrollView();
    }
}