using UnityEngine;
using System.Collections.Generic;

public class zzApplication:MonoBehaviour
{
    public bool changeRunInBackground = false;
    public bool newRunInBackground = false;

    public string startLevelName;
    public string nextLevelName;

    void Start()
    {
        //_lastLevelName = sLastLevelName;
        if (changeRunInBackground)
            Application.runInBackground = newRunInBackground;
    }

    //[SerializeField]
    //string _lastLevelName;

    //public string lastLevelName
    //{
    //    get { return sLastLevelName; }
    //    set { sLastLevelName = value; }
    //}

    static Stack<string> sLastLevelName = new Stack<string>();

    public void LoadStartLevel()
    {
        sLastLevelName.Clear();
        Application.LoadLevel(startLevelName);
    }

    void pushLevel()
    {
        sLastLevelName.Push( Application.loadedLevelName );
    }

    string popLevel()
    {
        return sLastLevelName.Pop();
    }

    public void LoadLastLevelNetState()
    {
        Network.SetSendingEnabled(0, false);

        Network.isMessageQueueRunning = false;

        LoadLastLevel();

        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);
    }

    public void LoadLastLevel()
    {
        Application.LoadLevel(popLevel());
    }

    public void LoadLevelNetState(string pName)
    {
        Network.SetSendingEnabled(0, false);

        Network.isMessageQueueRunning = false;

        LoadLevel(pName);

        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);
    }

    public void LoadLevel(string pName)
    {
        pushLevel();
        Application.LoadLevel(pName);
    }

    public void LoadNextLevel()
    {
        LoadLevel(nextLevelName);
    }

    public void LoadNextLevelNetState()
    {
        LoadLevelNetState(nextLevelName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}