
using UnityEngine;
using System.Collections;

class zzWindow : zzGUIContainer
{

    //FIXME_VAR_TYPE windowRect= new Rect(20, 20, 120, 50);
    //public GUIContent title;
    //public bool _useDefaultStyle = false;
    //public GUIStyle _style;

    public zzGUIStyle ContentAndStyle;
    public int ID = 0;

    public bool enableDrag = false;

    //zzGUI[] subElements;


    public override void impGUI()
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            //print("_useDefaultStyle");
            position = GUI.Window(ID, getPosition(), DoMyWindow, ContentAndStyle.Content);
            return;
        }
        //print("not _useDefaultStyle");
        position = GUI.Window(ID, getPosition(), DoMyWindow, ContentAndStyle.Content, ContentAndStyle.Style);
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
    //	i.renderGUI();
    //print("DoMyWindow begin");

    /*
foreach(Transform i in transform)
{
    //print(i.name);
    zzGUI renderGUI = i.GetComponent<zzGUI>();
    if(renderGUI)
        renderGUI.renderGUI();
}
*/
    //print("DoMyWindow end");
    //	}

}