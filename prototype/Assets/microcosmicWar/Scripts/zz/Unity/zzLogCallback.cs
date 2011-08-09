using UnityEngine;

public class zzLogCallback:MonoBehaviour
{
    [SerializeField]
    int _errorCount = 0;

    public int errorCount
    {
        get { return _errorCount; }
    }

    static void nullEventReceiver(string p){}

    System.Action<string> errorEvent = nullEventReceiver;

    public void addErrorEventVoidReceiver(System.Action pReceiver)
    {
        addErrorEventReceiver((x) => pReceiver());
    }

    public void addErrorEventReceiver(System.Action<string> pReceiver)
    {
        errorEvent -= nullEventReceiver;
        errorEvent += pReceiver;
    }

    void OnEnable()
    {
        Application.RegisterLogCallback(HandleLog);
    }

    void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    void HandleLog(string logString, string stackTrace, LogType pLogType)
    {
        switch(pLogType)
        {
            case LogType.Error:
            case LogType.Assert:
            case LogType.Exception:
                ++_errorCount;
                errorEvent(logString);
                break;
        }
    }
}