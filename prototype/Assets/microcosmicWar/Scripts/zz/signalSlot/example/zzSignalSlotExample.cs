using UnityEngine;
using System.Collections;

/// <summary>
/// A class show how to use zzSignalSlot.
/// For more info about delegate in C#, please see
/// http://msdn.microsoft.com/en-us/library/ms173171%28v=VS.80%29.aspx
/// </summary>
public class zzSignalSlotExample:MonoBehaviour
{
    //define a delegate type
    public delegate string TestDelegate();

    //default value for TestDelegate
    static string nullTestDelegate() { return ""; }

    //create a public delegate as signal
    public TestDelegate testDelegate;

    //create a private delegate as signal,will set it in setTestDelegate function
    TestDelegate setByfunction;


    //use delegate type as only one parameter,the function also can been "signal"
    //will use slot's function such as testFunction1 and testFunction2
    //as argument of setTestDelegate,when connect
    public void setTestDelegate(TestDelegate pTest)
    {
        setByfunction = 
            () => "set by function:" + pTest();
    }

    //a function of TestDelegate type,as slot
    public string testFunction1()
    {
        return " testFunction1 ";
    }

    //a function of TestDelegate type,as slot
    public string testFunction2()
    {
        return " testFunction2 ";
    }

    void Start()
    {
        //use default when not been set value in Awake
        if (testDelegate == null)
            testDelegate = nullTestDelegate;
        if (setByfunction == null)
            setByfunction = nullTestDelegate;
    }

    //save the return of implementing signal
    string info="";

    void OnGUI()
    {
        if (GUILayout.Button("testDelegate", GUILayout.ExpandWidth(false)))
            info = testDelegate();
        if (GUILayout.Button("setByfunction", GUILayout.ExpandWidth(false)))
            info = setByfunction();

        GUILayout.Label(info);
    }
}