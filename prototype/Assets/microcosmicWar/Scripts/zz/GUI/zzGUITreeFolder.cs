﻿using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// attention: when System.IO, don't forget
/// change "Player Settings"->"Other Settings"->"API Compatibility Level" to .Net 2.0
/// </summary>

public class zzGUITreeFolder: zzWindow
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