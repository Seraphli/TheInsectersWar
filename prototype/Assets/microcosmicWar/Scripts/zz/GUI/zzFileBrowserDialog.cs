
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

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
    public bool SelectList(string[] list,ref int selected)//, GUIStyle defaultStyle, GUIStyle selectedStyle)
    {
        if (selected >= list.Length)
            selected = -1;
        int i = 0;
        bool lIsChoose = false;
        foreach (string item in list)
        {
            var lStyle = new GUIStyle();
            lStyle.normal.textColor = (selected == i) ? Color.black : Color.white;

            if (GUILayout.Button(item, lStyle) )//, (selected == item) ? selectedStyle : defaultStyle))
            {
                //if(  Event.current.clickCount>1)
                selected = i;
                if (doubleClick)
                    lIsChoose = true;
            }
            ++i;
        }

        return lIsChoose;
    }

    static string[] getFilesName(FileInfo[] pFiles)
    {
        string[] lOut = new string[pFiles.Length];
        for (int i = 0; i < pFiles.Length;++i )
        {
            lOut[i] = Path.GetFileName(pFiles[i].ToString());
        }
        return lOut;
    }

    static string[] getDirectoriesName(DirectoryInfo[] pDirectories)
    {
        string[] lOut = new string[pDirectories.Length];
        for (int i = 0; i < pDirectories.Length; ++i)
        {
            lOut[i] = Path.GetDirectoryName(pDirectories[i].ToString());
        }
        return lOut;
    }

    static string[] getFileSystemsName(FileSystemInfo[] pFileSystem)
    {
        string[] lOut = new string[pFileSystem.Length];
        for (int i = 0; i < pFileSystem.Length; ++i)
        {
            lOut[i] = pFileSystem[i].Name;
        }
        return lOut;
    }

    public bool FileBrowser()
    {
        UpdateIsDoubleClick();
        //print(Event.current.type);
        //if (Event.current.clickCount>1)
        // print(Event.current.clickCount);
        bool complete;
        DirectoryInfo directoryInfo;
        //DirectoryInfo directorySelection;
        FileInfo fileSelection;
        int contentWidth;


        // Our return state - altered by the "Select" button
        complete = false;

        // Get the directory info of the current location
        //fileSelection = new FileInfo(location);
        if (location is DirectoryInfo)
        {
            directoryInfo = (DirectoryInfo)location;
        }
        else
        {
            directoryInfo = ((FileInfo)location).Directory;
        }

        GUILayout.BeginVertical();
        {
            var lParentDirectories = new List<DirectoryInfo>();
            {
                var lDirectory = directoryInfo;
                lParentDirectories.Add(lDirectory);
                while (lDirectory.Parent != null) 
                {
                    lDirectory = lDirectory.Parent;
                    lParentDirectories.Add(lDirectory);
                } 

            }

            bool lIsChangeDirectory = false;

            //创建父路径按钮
            GUILayout.BeginHorizontal();
            {
                for (int i = lParentDirectories.Count-1; i >= 0;--i )
                {
                    if (GUILayout.Button(lParentDirectories[i].Name)
                        && Event.current.type != EventType.Layout
                        && Event.current.type != EventType.Repaint )
                    {
                        location = lParentDirectories[i];
                        lIsChangeDirectory = true;
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (lIsChangeDirectory)
                return false;

            //if (directoryInfo.Parent!=null && GUILayout.Button("Up one level"))
            //{
            //    directoryInfo = directoryInfo.Parent;
            //    location = directoryInfo;
            //}

            GUILayout.BeginHorizontal();
            {
                // Handle the directories list
                //GUILayout.BeginArea(new Rect(10, 40, 200, 300));
                //GUILayout.BeginVertical();
                //{
                //    GUILayout.Label("Directories:");
                //    directoryScroll = GUILayout.BeginScrollView(directoryScroll);
                //    directorySelection = SelectList(directoryInfo.GetDirectories(), null) as DirectoryInfo;
                //    GUILayout.EndScrollView();

                //}
                //GUILayout.EndVertical();

                //if (directorySelection != null)
                //// If a directory was selected, jump there
                //{
                //    location = directorySelection.FullName;
                //}

                GUILayout.Space(10);

                var lDirectories = directoryInfo.GetDirectories();
                var lFiles = directoryInfo.GetFiles();
                var lPathFile = new List<FileSystemInfo>(lDirectories);
                lPathFile.AddRange(lFiles);
                pathFiles = lPathFile.ToArray();
                //pathFiles = (FileSystemInfo[])lDirectories + (FileSystemInfo[])lFiles;
                bool isChoose;
                // Handle the files list
                GUILayout.BeginVertical();
                {
                    //GUILayout.Label("Files:");
                    fileScroll = GUILayout.BeginScrollView(fileScroll);
                    isChoose = SelectList(getFileSystemsName(pathFiles), ref fileSelectedIndex);
                    //fileSelectedIndex = SelectList(new string[]{"aaa","bbbb"}, fileSelectedIndex);

                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();


                if (
                    isChoose
                    && Event.current.type != EventType.Layout
                    && Event.current.type != EventType.Repaint)
                // If a file was selected, update our location to it
                {
                    location = pathFiles[fileSelectedIndex];
                }



            }
            GUILayout.EndHorizontal();

            // The manual location box and the select button
            GUILayout.BeginHorizontal();
            {
                selectedLocation = GUILayout.TextArea(selectedLocation);

                contentWidth = (int)GUI.skin.GetStyle("Button").CalcSize(new GUIContent("Select")).x;
                if (GUILayout.Button("Select", GUILayout.Width(contentWidth)))
                {
                    complete = true;
                }
            }
            GUILayout.EndHorizontal();


        }

        GUILayout.EndVertical();


        return complete;
    }

    FileSystemInfo _location = new DirectoryInfo(System.Environment.CurrentDirectory);

    [SerializeField]
    string locationName;

    public FileSystemInfo location
    {
        set
        {
            _location = value;
            locationName = value.FullName;
        }

        get { return _location; }
    }

    //public Vector2 directoryScroll;
    public int fileSelectedIndex=-1;
    public Vector2 fileScroll;

    public GUICallFunc endBrowseCallFunc = nullGUICallback;

    public string selectedLocation
    {
        set 
        {
            if (File.Exists(value))
                location = new FileInfo(value);
            else if (Directory.Exists(value))
                location = new DirectoryInfo(value);
        }

        get
        {
            return location.ToString();
        }
    }

    //[SerializeField]
    FileSystemInfo[] pathFiles = new FileSystemInfo[0];

    public override void impWindow(int windowID)
    {
        if (FileBrowser())
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


    //float firstClick = 0.0f;
    //float secondClick = 0.0f;
    float lastDoubleClickTime = 0.0f;

    bool doubleClick;

    void UpdateIsDoubleClick()
    {
        if ((Time.time - lastDoubleClickTime) > 0.3 )
        {
            if (Event.current.clickCount ==2)
            {
                doubleClick = true;
                lastDoubleClickTime = Time.time;

            }
            else
                doubleClick = false;
        }
    }


}