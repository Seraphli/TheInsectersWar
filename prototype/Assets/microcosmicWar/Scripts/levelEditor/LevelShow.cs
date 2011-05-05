using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LevelShow:MonoBehaviour
{
    public string levelRootFolder;
    public string mainFileName = "main.zzScene";
    public Texture2D defaultLevelImage;
    public zzGUILibTreeInfo treeUIInfo;

    public bool checkMapAvailable(string pMap)
    {
        if (Directory.Exists(levelRootFolder + "/" + pMap))
        {
            return true;
        }
        return false;
    }

    zzGUILibTreeElement[] getLevelInFolder(string pLevelRootFolder)
    {
        DirectoryInfo lDirectory = new DirectoryInfo(pLevelRootFolder);
        var lLevelDirectorise =  lDirectory.GetDirectories();
        var lLevelElement = new List<zzGUILibTreeElement>(lLevelDirectorise.Length);
        foreach (var lLevelDir in lLevelDirectorise)
        {
            var lMainFileName = lLevelDir.FullName + "/" + mainFileName;
            if (File.Exists(lMainFileName))
            {
                var lGUIElement = new zzGUILibTreeElement();
                lGUIElement.name = lLevelDir.Name;
                lGUIElement.image = defaultLevelImage;
                lGUIElement.stringData = lLevelDir.Name;
                //lGUIElement.objectData = lInfoElement.data;
                lLevelElement.Add( lGUIElement );
            }
        }
        return lLevelElement.ToArray();
    }

    void Start()
    {
        zzGUILibTreeNode lTreeNode = new zzGUILibTreeNode();
        if (Directory.Exists(levelRootFolder))
            lTreeNode.elements = getLevelInFolder(levelRootFolder);
        treeUIInfo.treeInfo.setData(lTreeNode);
    }
}