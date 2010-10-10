
using UnityEngine;
using System.Collections;


public class zzGUIGroup : zzGUIContainer
{

    //FIXME_VAR_TYPE windowRect= new Rect(20, 20, 120, 50);
    public GUIContent content = new GUIContent();
    public bool useDefaultStyle = false;
    public GUIStyle style = new GUIStyle();

    //zzGUI[] subElements;


    public override void impGUI()
    {
        if (useDefaultStyle)
        {
            //print("useDefaultStyle");
            GUI.BeginGroup(getPosition(), content);
            impSubs();
            GUI.EndGroup();
            return;
        }
        //print("not useDefaultStyle");
        GUI.BeginGroup(getPosition(), content, style);
        impSubs();
        GUI.EndGroup();
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