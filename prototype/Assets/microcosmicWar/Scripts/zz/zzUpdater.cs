using UnityEngine;
using System.Collections;
using System.Json;
using System.IO;

public class zzUpdater:MonoBehaviour
{
    public string updaterPath;
    public string updateInfoUrl;
    public string changeInfoUrl;
    //相对于updaterPath路径的
    public string _7zPath;
    public string tempPath = "temp";
    public string srcPathInArchive = "bin";
    public string runPathAfterSetup;
    int _nowVersion;

    public string[] downloadList;

    [SerializeField]
    string _newVersonName;

    public string newVersonName
    {
        get { return _newVersonName; }
    }

    public string downloadFileName;

    public int nowVersion
    {
        set { _nowVersion = value; }
    }

    System.Action haveUpdateEvent;
    public void addHaveUpdateReceiver(System.Action pReceiver)
    {
        haveUpdateEvent += pReceiver;
    }

    System.Action haveNoUpdateEvent;
    public void addHaveNoUpdateReceiver(System.Action pReceiver)
    {
        haveNoUpdateEvent += pReceiver;
    }

    System.Action checkUpdateFailEvent;
    public void addCheckUpdateFailReceiver(System.Action pReceiver)
    {
        checkUpdateFailEvent += pReceiver;
    }

    System.Action checkEndEvent;
    public void addCheckEndReceiver(System.Action pReceiver)
    {
        checkEndEvent += pReceiver;
    }

    static void nullReceiver(){}

    void Start()
    {
        if (haveUpdateEvent == null)
            haveUpdateEvent = nullReceiver;
        if (haveNoUpdateEvent == null)
            haveNoUpdateEvent = nullReceiver;
        if (checkUpdateFailEvent == null)
            checkUpdateFailEvent = nullReceiver;
        if (checkEndEvent == null)
            checkEndEvent = nullReceiver;

        getUpdateInfo();
    }

    public void startUpdateProcess()
    {
        var lFilePath = updaterPath + ".exe";
        if (File.Exists(lFilePath))
            File.Delete(lFilePath);
        File.Copy(updaterPath, lFilePath);
        print("UpdaterFilePath:" + lFilePath + " " + getAbsolutePath(lFilePath));
        var lUpdateProcess = new System.Diagnostics.Process()
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = getAbsolutePath(lFilePath),
                Arguments = updateCommand,
                CreateNoWindow = false,
            },
        };
        lUpdateProcess.Start();
        Application.Quit();
    }

    void checkEnd()
    {
        downloadingUpdateInfo = false;
        checkEndEvent();
    }

    [SerializeField]
    bool downloadingUpdateInfo = false;

    IEnumerator downloadUpdateInfo()
    {
        downloadingUpdateInfo = true;

        var lRequest = new WWW(updateInfoUrl);
        yield return lRequest;
        if (lRequest.error != null)
        {
            Debug.LogError(updateInfoUrl + " downloadUpdateInfo failed:" + lRequest.error);
            checkUpdateFailEvent();
            lRequest.Dispose();
            downloadingUpdateInfo = false;
            yield break;
        }
        print(lRequest.error);
        var lUpdateInfo = lRequest.text.Trim();
        lRequest.Dispose();
        downloadingUpdateInfo = false;
        bool lFailed = false;
        try
        {
            parseUpdateInfo(lUpdateInfo);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("Parse Update Info Error:" + lUpdateInfo);
            lFailed = true;
        }
        if (lFailed)
            checkUpdateFailEvent();
//        string lTemp = @"
//            {
//                ""Name""     : ""Thomas More"",
//                ""Age""      : 57,
//                ""Birthday"" : ""02/07/1478 00:00:00""
//            }";
    }

    public string updateCommand;

    void addUpdateCommand(string pKey,string pString)
    {
        print(pKey + ":" + pString);
        updateCommand += "\"" + pKey + "=" + pString + "\" ";
    }

    string getAbsolutePath(string pPath)
    {
        return Path.Combine(Application.dataPath + "/../", pPath);
    }

    void createUpdateCommand()
    {
        if (tempPath != null)
            addUpdateCommand("TempPath", getAbsolutePath(tempPath));
        else
            addUpdateCommand("TempPath", Application.temporaryCachePath);
        addUpdateCommand("FileName", downloadFileName);
        addUpdateCommand("7zPath", _7zPath);
        addUpdateCommand("SrcPathInArchive", srcPathInArchive);
        addUpdateCommand("UpdatePath", Application.dataPath + "/../");
        if (!string.IsNullOrEmpty(runPathAfterSetup))
            addUpdateCommand("RunPathAfterSetup", getAbsolutePath(runPathAfterSetup));
        foreach (var lDownloadUrl in downloadList)
        {
            addUpdateCommand("DownloadUrl", lDownloadUrl);
        }
        print("updateCommand:"+updateCommand);
    }

    void parseUpdateInfo(string pInfo)
    {
        checkEnd();
        var lUpdateInfo = (System.Json.JsonObject)System.Json.JsonObject.Parse(pInfo);
        int lNewVersion;
        bool lSucceed = true;
        lSucceed |= lUpdateInfo.TryGetValue("NewVersion", out lNewVersion);
        lSucceed |= lUpdateInfo.TryGetValue("NewVersionName", out _newVersonName);
        lSucceed |= lUpdateInfo.TryGetValue("DownloadList", out downloadList);
        lSucceed |= lUpdateInfo.TryGetValue("FileName", out downloadFileName);

        //安装后运行的程序,没有则用默认 
        lUpdateInfo.TryGetValue("Run", out runPathAfterSetup);
        if (!lSucceed)
        {
            Debug.LogError("parseUpdateInfo fail");
            checkUpdateFailEvent();
        }
        if (lNewVersion > _nowVersion)
            haveUpdateEvent();
        else
            haveNoUpdateEvent();
        //print(lNewVersion);
        //var  lUpdateInfo[]
        createUpdateCommand();
    }

    [ContextMenu("Update")]
    public void getUpdateInfo()
    {
        if (!downloadingUpdateInfo)
            StartCoroutine(downloadUpdateInfo());
    }

    public void getChangeInfo()
    {

    }
}

public static class zzJsonExtensionMethods
{
    public static bool TryGetValue(this JsonObject pJsonObject, string key, out int value)
    {
        JsonValue lJsonValue;
        if (pJsonObject.TryGetValue(key, out lJsonValue)
            && lJsonValue.JsonType == JsonType.Number)
        {
            value = (int)lJsonValue;
            return true;
        }
        value = default(int);
        return false;
    }

    public static bool TryGetValue(this JsonObject pJsonObject, string key, out string value)
    {
        JsonValue lJsonValue;
        if (pJsonObject.TryGetValue(key, out lJsonValue)
            && lJsonValue.JsonType == JsonType.String)
        {
            value = (string)lJsonValue;
            return true;
        }
        value = default(string);
        return false;
    }

    public static bool TryGetValue(this JsonObject pJsonObject, string key,
        out string[] value)
    {
        JsonValue lJsonValue;
        if (pJsonObject.TryGetValue(key, out lJsonValue)
            && lJsonValue.JsonType == JsonType.Array)
        {
            var lJsonArray = (JsonArray)lJsonValue;
            string[] lOut = new string[lJsonArray.Count];
            for (int i = 0; i < lOut.Length;++i )
            {
                var lElement = lJsonArray[i];
                if (lElement.JsonType == JsonType.String)
                {
                    lOut[i] = (string)lElement;
                }
                else
                {
                    value = default(string[]);
                    return false;
                }
            }
            value = lOut;
            return true;
        }
        value = default(string[]);
        return false;
    }

}