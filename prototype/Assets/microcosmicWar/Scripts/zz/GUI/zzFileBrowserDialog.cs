
using UnityEngine;
using System.IO;
using System.Collections;

class zzFileBrowserDialog : zzWindow
{

    /// <summary>
    /// from http://www.unifycommunity.com/wiki/index.php?title=SelectList
    /// </summary>
    /// <param name="list"></param>
    /// <param name="selected"></param>
    /// <param name="defaultStyle"></param>
    /// <param name="selectedStyle"></param>
    /// <returns></returns>
    public static object SelectList(ICollection list, object selected)//, GUIStyle defaultStyle, GUIStyle selectedStyle)
    {
        foreach (object item in list)
        {
            var lStyle = new GUIStyle();
            lStyle.normal.textColor = (selected == item) ? Color.black : Color.white;
            if (GUILayout.Button(item.ToString(), lStyle))//, (selected == item) ? selectedStyle : defaultStyle))
            {
                if (selected == item)
                // Clicked an already selected item. Deselect.
                {
                    selected = null;
                }
                else
                {
                    selected = item;
                }
            }
        }

        return selected;
    }

    /// <summary>
    /// from http://www.unifycommunity.com/wiki/index.php?title=FileBrowser
    /// </summary>
    /// <param name="location"></param>
    /// <param name="directoryScroll"></param>
    /// <param name="fileScroll"></param>
    /// <returns></returns>
    public static bool FileBrowser(ref string location, ref Vector2 directoryScroll, ref Vector2 fileScroll)
    {
        bool complete;
        DirectoryInfo directoryInfo;
        DirectoryInfo directorySelection;
        FileInfo fileSelection;
        int contentWidth;


        // Our return state - altered by the "Select" button
        complete = false;

        // Get the directory info of the current location
        fileSelection = new FileInfo(location);
        if ((fileSelection.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
        {
            directoryInfo = new DirectoryInfo(location);
        }
        else
        {
            directoryInfo = fileSelection.Directory;
        }


        if (location != "/" && GUI.Button(new Rect(10, 20, 410, 20), "Up one level"))
        {
            directoryInfo = directoryInfo.Parent;
            location = directoryInfo.FullName;
        }


        // Handle the directories list
        GUILayout.BeginArea(new Rect(10, 40, 200, 300));
        GUILayout.Label("Directories:");
        directoryScroll = GUILayout.BeginScrollView(directoryScroll);
        directorySelection = SelectList(directoryInfo.GetDirectories(), null) as DirectoryInfo;
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (directorySelection != null)
        // If a directory was selected, jump there
        {
            location = directorySelection.FullName;
        }


        // Handle the files list
        GUILayout.BeginArea(new Rect(220, 40, 200, 300));
        GUILayout.Label("Files:");
        fileScroll = GUILayout.BeginScrollView(fileScroll);
        fileSelection = SelectList(directoryInfo.GetFiles(), null) as FileInfo;
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (fileSelection != null)
        // If a file was selected, update our location to it
        {
            location = fileSelection.FullName;
        }


        // The manual location box and the select button
        GUILayout.BeginArea(new Rect(10, 350, 410, 20));
        GUILayout.BeginHorizontal();
        location = GUILayout.TextArea(location);

        contentWidth = (int)GUI.skin.GetStyle("Button").CalcSize(new GUIContent("Select")).x;
        if (GUILayout.Button("Select", GUILayout.Width(contentWidth)))
        {
            complete = true;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();


        return complete;
    }

    public string location = System.Environment.CurrentDirectory;
    public Vector2 directoryScroll;
    public Vector2 fileScroll;

    public GUICallFunc endBrowseCallFunc = nullGUICallback;

    public override void impWindow(int windowID)
    {
        if (FileBrowser(ref location, ref directoryScroll, ref fileScroll))
        {
            endBrowseCallFunc(this);
            Destroy(gameObject);
        }
    }

    public static zzFileBrowserDialog createDialog(Transform pParent)
    {
        var lObject = new GameObject();
        lObject.transform.parent = pParent;
        var lOut = lObject.AddComponent<zzFileBrowserDialog>();
        return lOut;
    }

}