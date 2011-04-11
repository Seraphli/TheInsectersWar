
using UnityEngine;
using System.Collections;

public class zzWindow : zzGUIContainer
{

    //FIXME_VAR_TYPE windowRect= new Rect(20, 20, 120, 50);
    //public GUIContent title;
    //public bool _useDefaultStyle = false;
    //public GUIStyle _style;

    public zzGUIStyle ContentAndStyle = new zzGUIStyle();
    public int ID = 0;

    public bool enableDrag = false;

    //zzGUI[] subElements;

    void Awake()
    {
        ID = newID();
    }

    /// <summary>
    /// 父UI有Group 时,将返回错误的信息
    /// </summary>
    public override bool isCursorOver
    {
        get 
        {
            var lMousePosition = Input.mousePosition;
            lMousePosition.y = Screen.height - lMousePosition.y;
            return position.Contains(lMousePosition); 
        }
    }

    public override bool isCoordinateReseted
    {
        get
        {
            return true;
        }
    }

    public override void impGUI(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            position = GUI.Window(ID, rect, DoMyWindow, ContentAndStyle.Content);
            return;
        }
        //print("not _useDefaultStyle");
        position = GUI.Window(ID, rect, DoMyWindow, ContentAndStyle.Content, ContentAndStyle.Style);
    }

    public void DoMyWindow(int windowID)
    {
        impWindow(windowID);
        //print("enableDrag:"+enableDrag);
        if (enableDrag)
            GUI.DragWindow();
    }

    
    public virtual void impWindow(int windowID)
    {
        impSubs();
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