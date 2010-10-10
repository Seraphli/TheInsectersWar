
using UnityEngine;
using System.Collections;

class zzWindow : zzGUIContainer
{

    //FIXME_VAR_TYPE windowRect= new Rect(20, 20, 120, 50);
    public GUIContent title;
    public bool useDefaultStyle = false;
    public GUIStyle style;
    public int ID = 0;

    public bool enableDrag = false;

    //zzGUI[] subElements;


    public override void impGUI()
    {
        if (useDefaultStyle)
        {
            //print("useDefaultStyle");
            position = GUI.Window(ID, getPosition(), DoMyWindow, title);
            return;
        }
        //print("not useDefaultStyle");
        position = GUI.Window(ID, getPosition(), DoMyWindow, title, style);
    }

    public virtual void DoMyWindow(int windowID)
    {
        impSubs();
        //print("enableDrag:"+enableDrag);
        if (enableDrag)
            GUI.DragWindow();
    }


    // Make the contents of the window
    //function DoMyWindow ( int windowID  ) 
    //	{
    //if (GUI.Button ( new Rect(10,20,100,20), "Hello World"))
    //print ("Got a click");
    //foreach(zzGUI i in subElements)
    //	i.impGUI();
    //print("DoMyWindow begin");

    /*
foreach(Transform i in transform)
{
    //print(i.name);
    zzGUI impGUI = i.GetComponent<zzGUI>();
    if(impGUI)
        impGUI.impGUI();
}
*/
    //print("DoMyWindow end");
    //	}

}