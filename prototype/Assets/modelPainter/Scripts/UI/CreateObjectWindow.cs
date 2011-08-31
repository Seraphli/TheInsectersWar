using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameSystemInfoNodeExtensionMethods
{
    public static zzGUILibTreeNode getGUITreeNode(this GameSystem.InfoNode pInfoNode,
        Texture2D pDefaultCollapsedNodeImg, Texture2D pDefaultExpandedNodeImg)
    {
        var elements = pInfoNode.elements;
        var image = pInfoNode.image;
        var nodes = pInfoNode.nodes;
        var showName = pInfoNode.showName;
        zzGUILibTreeNode lOut = new zzGUILibTreeNode();
        lOut.name = showName;
        lOut.image = image ? image : pDefaultCollapsedNodeImg;
        lOut.expandedImage = pInfoNode.expandedImage ? pInfoNode.expandedImage : pDefaultExpandedNodeImg;

        var lGUIElements = new List<zzGUILibTreeElement>(elements.Length);
        for (int i = 0; i < elements.Length; ++i)
        {
            var lInfoElement = elements[i];
            if (!lInfoElement.isShow)
                continue;
            var lGUIElement = new zzGUILibTreeElement();
            lGUIElement.name = lInfoElement.showName;
            lGUIElement.image = lInfoElement.image;
            lGUIElement.stringData = lInfoElement.name;
            lGUIElement.objectData = lInfoElement.data;
            //lGUIElements[i] = lGUIElement;
            lGUIElements.Add(lGUIElement);
        }
        lOut.elements = lGUIElements.ToArray();

        var lGUINode = new zzGUILibTreeNode[nodes.Length];
        for (int i = 0; i < nodes.Length; ++i)
        {
            lGUINode[i] = nodes[i].getGUITreeNode(pDefaultCollapsedNodeImg, pDefaultExpandedNodeImg);
        }
        lOut.nodes = lGUINode;

        return lOut;
    }

}
public class CreateObjectWindow : MonoBehaviour
{
    public Texture2D defaultCollapsedNodeImage;
    public Texture2D defaultExpandedNodeImage;

    public zzGUILibTreeInfo prefabTreeUIInfo;
    public zzGUILibTreeInfo settingTreeUIInfo;
    public delegate void AddObjectEvent(GameObject pObject);

    static void nullAddObjectEvent(GameObject pObject)
    {

    }

    AddObjectEvent addObjectEvent;

    public void addAddObjectEventReceiver(AddObjectEvent pFunc)
    {
        addObjectEvent += pFunc;
    }

    void Start()
    {
        prefabTreeUIInfo.treeInfo.setData( 
                GameSystem.Singleton.prefabInfoTree.getGUITreeNode(
                    defaultCollapsedNodeImage,defaultExpandedNodeImage
                    )
            );

        settingTreeUIInfo.treeInfo.setData(
                GameSystem.Singleton.settingInfoTree.getGUITreeNode(
                    defaultCollapsedNodeImage, defaultExpandedNodeImage
                    )
            );

        if (addObjectEvent == null)
            addObjectEvent = nullAddObjectEvent;
        //if(getCreateInfoFromSystem)
        //{
        //    var lPrefabInfos = GameSystem.Singleton.PrefabInfoList;
        //    objectCreateInfos = new ObjectCreateInfo[lPrefabInfos.Length];
        //    for (int i = 0; i < lPrefabInfos.Length; ++i)
        //    {
        //        var lObjectCreateInfo = new ObjectCreateInfo();
        //        var lPrefabInfo = lPrefabInfos[i];
        //        lObjectCreateInfo.showName = lPrefabInfo.showName;
        //        lObjectCreateInfo.name = lPrefabInfo.name;
        //        //lObjectCreateInfo.prefab = lPrefabInfo.prefab;
        //        objectCreateInfos[i] = lObjectCreateInfo;
        //    }

        //}

    }

    [System.Serializable]
    public class ObjectCreateInfo
    {
        public string name;
        public string showName;
        public GameObject prefab;
    }

    //public float createObjectDistance = 5f;

    public Transform createObjectTransform;

    //public bool getCreateInfoFromSystem = false;

    //public ObjectCreateInfo[] objectCreateInfos = new ObjectCreateInfo[0];

    Vector3 getCreatePos()
    {
        //Transform lTransform = Camera.main.transform;
        //return lTransform.position + lTransform.forward * createObjectDistance;
        return createObjectTransform.position;
    }

    public bool showCreatePosition = false;

    void OnDrawGizmosSelected()
    {
        if (showCreatePosition)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(getCreatePos(), 0.2f);
        }
    }

    public void createObjectAutoTransform(string pTypeName)
    {
        var lObject = GameSystem.Singleton.createObject(pTypeName, 
            getCreatePos(), Quaternion.identity);
        addObjectEvent(lObject);
    }

    public zzGUILink GUILink;

    public void systemSetting(Object pSetting)
    {
        GUILink.link = (zzInterfaceGUI)pSetting;
    }

    //public override void impWindow(int windowID)
    //{
    //    GUILayout.BeginVertical();
    //    foreach (var lInfo in objectCreateInfos)
    //    {
    //        if (GUILayout.Button(lInfo.showName, GUILayout.ExpandWidth(false)))
    //        {
    //            GameObject lObject;
    //            if(lInfo.prefab)
    //                lObject = (GameObject)Instantiate(lInfo.prefab,
    //                    getCreatePos(), Quaternion.identity);
    //            else
    //                lObject = GameSystem.Singleton.createObject(lInfo.name,
    //                    getCreatePos(), Quaternion.identity);

    //            addObjectEvent(lObject);
    //        }
    //    }
    //    GUILayout.EndVertical();
    //}
}