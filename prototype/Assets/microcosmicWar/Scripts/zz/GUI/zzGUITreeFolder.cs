using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// attention: when System.IO, don't forget
/// change "Player Settings"->"Other Settings"->"API Compatibility Level" to .Net 2.0
/// </summary>

public class zzGUITreeFolder: zzWindow
{
    class TreeFolderDraw 
    {
        public GUIStyle    selectedStyle;
        public GUIStyle    notSelectedStyle;

        int TreeDepth = 0;
        bool _expanded = false;

        bool haveExpanded = false;

        public bool expanded
        {
            get { return _expanded; }
            set 
            {
                if (_expanded == value)
                    return;

                _expanded = value;

                if (!haveExpanded||(_expanded && checkChanged()))
                {
                    if (directoryInfo.Exists)
                        updateData();
                    else
                    {
                        Folders.Clear();
                        files.Clear();
                    }
                    haveExpanded = true;
                }
            }
        }
        DirectoryInfo _directoryInfo;

        public DirectoryInfo directoryInfo
        {
            get { return _directoryInfo; }
            set 
            { 
                _directoryInfo = value;
                //lastChangeTime = _directoryInfo.LastWriteTime;
            }
        }

        //System.DateTime lastChangeTime;

        List<TreeFolderDraw> Folders = new List<TreeFolderDraw>();
        List<FileInfo> files = new List<FileInfo>();

        //是否展开过,判断是否初始化过子内容
        //bool haveExpanded = false;

        bool checkChanged()
        {
            if (!directoryInfo.Exists)
                return true;

            var lDirectoryInfo = new DirectoryInfo(directoryInfo.FullName);
            if (lDirectoryInfo.LastWriteTime != directoryInfo.LastWriteTime)
            {
                directoryInfo = lDirectoryInfo;
                return true;
            }

            return false;
        }

        void updateData()
        {
            files = new List<FileInfo>(directoryInfo.GetFiles());
            var lDirectories = directoryInfo.GetDirectories();
            Folders = new List<TreeFolderDraw>(lDirectories.Length);
            for(int i=0;i<lDirectories.Length;++i)
            {
                var lGUITreeFolder = new TreeFolderDraw();
                lGUITreeFolder.TreeDepth = TreeDepth + 1;
                lGUITreeFolder.directoryInfo = lDirectories[i];
                lGUITreeFolder.selectedStyle = selectedStyle;
                lGUITreeFolder.notSelectedStyle = notSelectedStyle;
                Folders.Add(lGUITreeFolder);
            }
        }

        public void drawGUI(ref string pSelectedName)
        {
            string lNewSelected = "";
            bool lNewExpanded;
            //print("TreeDepth:" + TreeDepth);
            //print(directoryInfo.Name+Folders.Count);
            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(6f * TreeDepth);
                lNewExpanded = GUILayout.Toggle(expanded, directoryInfo.Name);
                GUILayout.EndHorizontal();
                if (expanded)
                {
                    foreach (var lFolder in Folders)
                    {
                        lFolder.drawGUI(ref pSelectedName);
                    }

                    foreach (var lFile in files)
                    {
                        GUIStyle lStyle
                            = lFile.FullName == pSelectedName ? selectedStyle : notSelectedStyle;
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(6f * (TreeDepth+1));
                        if (GUILayout.Button(lFile.Name, lStyle))
                            lNewSelected = lFile.FullName;
                        GUILayout.EndHorizontal();
                    }
                }

            }
            GUILayout.EndVertical();

            if (lNewSelected.Length > 0)
                pSelectedName = lNewSelected;
            expanded = lNewExpanded;
            //if (_expanded && (lastChangeTime != directoryInfo.LastWriteTime))
            //    updateData();
        }

    }
    public bool relativePath = true;
    public string rootFolder;
    public string selectedName;

    TreeFolderDraw rootTreeFolderDraw = new TreeFolderDraw();

    public GUIStyle selectedStyle;
    public GUIStyle notSelectedStyle;

    void Start()
    {
        if (!Application.isPlaying)
            return;

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

    Vector2 fileScroll;

    public override void impWindow(int windowID)
    {
        if (Application.isPlaying)
        {
            fileScroll = GUILayout.BeginScrollView(fileScroll);
            rootTreeFolderDraw.drawGUI(ref selectedName);
            GUILayout.EndScrollView();
        }
    }
}