using UnityEngine;
using System.IO;

public class zzLogFile:MonoBehaviour
{
    public string logFileName;
    public StreamWriter writer;
    public FileStream logFile;

    void Awake()
    {
        Application.RegisterLogCallback(HandleLog);

        logFile = new FileStream(logFileName, FileMode.Create);
        writer = new StreamWriter(logFile);
        writer.AutoFlush = true;
        //stringWriter.
        //{
        //    BinaryWriter lWriter = new BinaryWriter(lFile);
        //    lWriter.Write(lScreenshot.EncodeToPNG());
        //}
    }

    void OnDisable()
    {
        logFile.Close();
        Application.RegisterLogCallback(null);
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