using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class zzApplication:MonoBehaviour
{
    public bool changeRunInBackground = false;
    public bool newRunInBackground = false;

    public string startLevelName;
    public string nextLevelName;

    public float nextLevelDelayLoad = 0f;

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
    #region Level
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

    IEnumerator delayLoadLevelNet(string pName, float pDelay)
    {
        Network.SetSendingEnabled(0, false);

        Network.isMessageQueueRunning = false;

        yield return new WaitForSeconds(pDelay);
        LoadLevel(pName);

        Network.isMessageQueueRunning = true;
        Network.SetSendingEnabled(0, true);
    }

    IEnumerator delayLoadLevel(string pName, float pDelay)
    {
        yield return new WaitForSeconds(pDelay);
        LoadLevel(pName);
    }

    public void LoadLevel(string pName)
    {
        pushLevel();
        Application.LoadLevel(pName);
    }

    public void LoadNextLevel()
    {
        if (nextLevelDelayLoad == 0f)
            LoadLevel(nextLevelName);
        else
            StartCoroutine(delayLoadLevel(nextLevelName, nextLevelDelayLoad));
    }

    public void LoadNextLevelNetState()
    {
        if (nextLevelDelayLoad == 0f)
            LoadLevelNetState(nextLevelName);
        else
            StartCoroutine(delayLoadLevelNet(nextLevelName, nextLevelDelayLoad));
    }
    #endregion//end Level

    #region InputLanguage

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool SystemParametersInfo(int nAction, int nParam, [In, Out] System.IntPtr[] rc, int nUpdate);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern System.IntPtr GetKeyboardLayout(int dwLayout);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern System.IntPtr ActivateKeyboardLayout(System.IntPtr hkl, int uFlags);
 
    public int DefaultInputLanguageID
    {
        get
        {
            System.IntPtr[] rc = new System.IntPtr[1];
            SystemParametersInfo(0x59, 0, rc, 0);
            return (int)rc[0];

        }
    }

    public int inputLanguageID
    {
        get
        {
            return (int)GetKeyboardLayout(0);
        }
        set
        {
            int lInputLanguageID = 0;
            if (value == 0)
            {
                lInputLanguageID = DefaultInputLanguageID;
            }
            else
                lInputLanguageID = value;
            if (ActivateKeyboardLayout((System.IntPtr)lInputLanguageID, 0) == System.IntPtr.Zero)
                Debug.LogError("unavailable inputLanguageID:" + lInputLanguageID);
        }
    }

    #endregion//InputLanguage End

    public void OpenURL(string pURL)
    {
        Application.OpenURL(pURL);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

//public class Ime
//{
//    [DllImport("user32 ")]
//    private static extern uint ActivateKeyboardLayout(uint hkl, uint Flags);
//    [DllImport("user32 ")]
//    private static extern uint LoadKeyboardLayout(string pwszKLID, uint Flags);
//    [DllImport("user32 ")]
//    private static extern uint GetKeyboardLayoutList(int nBuff, uint[] List);

//    private static Dictionary<string, uint> FImes;
//    public static uint KLF_ACTIVATE = 1;

//    public Ime()
//    {
//        //  
//        //   TODO:   在此处添加构造函数逻辑  
//        //  
//    }

//    //设定当前Ime,使用方法Ime.SetImeName( "中文   (简体)   -   拼音加加 ");  
//    public static void SetImeName(string ImeName)
//    {
//        //字符串形式  
//        if (FImes == null)
//            GetImes();
//        uint id = System.Convert.ToUInt32(FImes[ImeName]);
//        SetIme(id);
//    }

//    public static void SetIme(uint ImeId)
//    {
//        //Id样式  
//        if (ImeId > 0)
//            ActivateKeyboardLayout(ImeId, KLF_ACTIVATE);
//    }

//    //获得所有的Ime列表  
//    public static Dictionary<string, uint> GetImes()
//    {
//        if (FImes == null)
//            FImes = new Dictionary<string, uint>();
//        else
//            return FImes;
//        uint[] KbList = new uint[64];
//        uint TotalKbLayout = GetKeyboardLayoutList(64, KbList);
//        Debug.Log("TotalKbLayout:" + TotalKbLayout);
//        for (int i = 0; i < TotalKbLayout; i++)
//        {
//            string RegKey = string.Format("System\\CurrentControlSet\\Control\\Keyboard Layouts\\{0:X8} ", KbList[i]);
//            RegistryKey rk = Registry.LocalMachine.OpenSubKey(RegKey);
//            Debug.Log(RegKey+":" + (rk == null));
//            if (rk == null)
//                continue;
//            string ImeName = (string)rk.GetValue("layout text");
//            Debug.Log("ImeName == null:" + (ImeName == null));
//            if (ImeName == null)
//                continue;
//            FImes.Add(ImeName, KbList[i]);
//            rk.Close();
//        }
//        return FImes;
//    }
//}