using UnityEngine;
using System.IO;

public class zzLogFile:MonoBehaviour
{
    public string logFileName;
    public StreamWriter writer;
    public FileStream logFile;
    public bool append = false;

    static protected zzLogFile singletonInstance;

    void Awake()
    {
        if (singletonInstance)
            return;

        FileMode lFileMode;
        if (append)
            lFileMode = FileMode.Append;
        else
            lFileMode = FileMode.Create;
        logFile = new FileStream(logFileName, lFileMode);
        writer = new StreamWriter(logFile);
        writer.AutoFlush = true;

        writer.WriteLine(System.DateTime.Now);

        Object.DontDestroyOnLoad(gameObject);
        //放在最后,以免IO发生异常
        Application.RegisterLogCallback(HandleLog);
        singletonInstance = this;
    }

    void OnDestroy()
    {
        //防止销毁的是后载入的场景中的物体的情况
        if (singletonInstance == this)
        {
            logFile.Close();
            Application.RegisterLogCallback(null);
            singletonInstance = null;
        }
    }

    void HandleLog(string logString, string stackTrace, LogType pLogType)
    {
        string lLogInfo = pLogType.ToString() + ":" 
            + logString 
            + writer.NewLine 
            + stackTrace
            + writer.NewLine
            + writer.NewLine;
        writer.Write(lLogInfo);
    }
}