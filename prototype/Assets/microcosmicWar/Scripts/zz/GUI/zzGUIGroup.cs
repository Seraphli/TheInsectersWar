
using UnityEngine;
using System.Collections;


public class zzGUIGroup : zzGUIContainer
{

    //FIXME_VAR_TYPE windowRect= new Rect(20, 20, 120, 50);
    //public GUIContent _content = new GUIContent();
    //public bool _useDefaultStyle = false;
    //public GUIStyle _style = new GUIStyle();

    //zzGUI[] subElements;
    public zzGUIStyle ContentAndStyle = new zzGUIStyle();

    public override void impGUI(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            //print("_useDefaultStyle");
            GUI.BeginGroup(rect, ContentAndStyle.Content);
            impSubs();
            GUI.EndGroup();
            return;
        }
        //print("not _useDefaultStyle");
        GUI.BeginGroup(rect, ContentAndStyle.Content, ContentAndStyle.Style);
        impSubs();
        GUI.EndGroup();
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