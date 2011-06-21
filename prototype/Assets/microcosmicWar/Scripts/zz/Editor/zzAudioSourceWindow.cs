using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;


public class zzAudioSourceWindow : EditorWindow
{
    [MenuItem("Window/Audio Source")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(zzAudioSourceWindow), false, "AudioSource");
    }

    AudioSource audioSource;
    GameObject sourceObject;

    string sourcePath;
    int sourceCount = 0;

    public void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            audioSource = (AudioSource)EditorGUILayout.ObjectField(audioSource, typeof(AudioSource));
            if (Application.isPlaying && audioSource && GUILayout.Button("play"))
            {
                audioSource.Play();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //EditorGUILayout.LabelField("", sourcePath);
            //if (GUILayout.Button("打开目录"))
            //{
            //    sourcePath = EditorUtility.OpenFolderPanel("音源文件目录", sourcePath, "");
            //    sourcePath.LastIndexOf()
            //}

            sourceObject = (GameObject)EditorGUILayout.ObjectField(sourceObject, typeof(GameObject));
            EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("", sourceCount.ToString());
            if (GUILayout.Button("处理"))
            {
                string lPath = AssetDatabase.GetAssetPath(sourceObject);
                lPath= lPath.Substring(0, lPath.LastIndexOf("/") + 1);
                var lAssets = getAssetAtPath<AudioSource>(lPath);
                foreach (var lDest in lAssets)
                {
                    lDest.minDistance = audioSource.minDistance;
                    lDest.maxDistance = audioSource.maxDistance;
                    lDest.rolloffMode = audioSource.rolloffMode;
                }
            }
            EditorGUILayout.EndHorizontal();

        }
    }

    public static T[] GetComponentsInChildren<T>(Transform pObject) where T : Component
    {
        List<T> lOut = new List<T>();
        lOut.AddRange(pObject.GetComponents<T>());
        foreach (Transform lChildren in pObject)
        {
            lOut.AddRange(GetComponentsInChildren<T>(lChildren));
        }
        return lOut.ToArray();
    }

    public static T[] getAssetAtPath<T>(string pPath)where T:Component
    {
        Debug.Log(pPath);
        List<T> lOut = new List<T>();
        string lPath = pPath;
        //if (File.Exists(pPath))
        //    lPath = Directory.GetParent(pPath).FullName;
        if (lPath.LastIndexOf("/") != lPath.Length - 1)
            lPath += "/";
        var lfiles = Directory.GetFiles(lPath);
        //if (typeof(Component).IsSubclassOf(typeof(T)))
        foreach (var lFileFullName in lfiles)
        {
            var lFile = lPath + Path.GetFileName(lFileFullName);
            var lObject = (GameObject)AssetDatabase.LoadMainAssetAtPath(lFile);
            if (lObject)
            {
                //Debug.Log(lObject.transform.childCount);
                //Debug.Log(lObject.audio == null);
                //Debug.Log(lObject.GetComponents<T>().Length);
                //Debug.Log(lObject.GetComponentsInChildren<T>().Length);
                var lSources = GetComponentsInChildren<T>(lObject.transform);
                if (lSources.Length>0)
                {
                    Debug.Log(lFile);
                    lOut.AddRange(lSources);
                    EditorUtility.SetDirty(lObject);
                    Debug.Log(lOut.Count);
                }
            }
        }
        //else
        //    foreach (var lFile in lfiles)
        //    {
        //        lOut += (T)Resources.LoadAssetAtPath(lFile, typeof(T));
        //    }
        //var lDirectories = Directory.GetDirectories(path);
        //foreach (var lDirectory in lfiles)
        //{
        //    lOut.AddRange( getAssetAtPath<T>(lDirectory) );
        //}
        return lOut.ToArray();
    }

    //public static T[] getAssetAtPath<T>(string path)
    //{

    //    ArrayList al = new ArrayList();
    //    string[] fileEntries = Directory.GetFiles(path);
    //    foreach (string fileName in fileEntries)
    //    {
    //        int index = fileName.LastIndexOf("/");
    //        string localPath = "Assets/" + path;

    //        if (index > 0)
    //            localPath += fileName.Substring(index);

    //        Object t = Resources.LoadAssetAtPath(localPath, typeof(T));

    //        if (t != null)
    //            al.Add(t);
    //    }
    //    T[] result = new T[al.Count];
    //    for (int i = 0; i < al.Count; i++)
    //        result[i] = (T)al[i];

    //    return result;
    //}
}