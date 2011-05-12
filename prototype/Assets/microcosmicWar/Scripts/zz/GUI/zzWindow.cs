
using UnityEngine;
using System.Collections;

public class zzWindow : zzGUIContainer
{

    public zzGUIStyle ContentAndStyle = new zzGUIStyle();
    public int ID = 0;

    public bool enableDrag = false;

    public bool alwayFront = false;

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

    public override Vector2 originOfCoordForSub
    {
        get
        {
            var lScreenPosition = screenPosition;
            return new Vector2(lScreenPosition.x, lScreenPosition.y);
        }
    }


    public override void impGUI(Rect rect)
    {
        if (ContentAndStyle.UseDefaultStyle)
        {
            position = GUI.Window(GetInstanceID(), rect, DoMyWindow, ContentAndStyle.Content);
            return;
        }
        //print("not _useDefaultStyle");
        position = GUI.Window(GetInstanceID(), rect, DoMyWindow, ContentAndStyle.Content, ContentAndStyle.Style);
    }

    public void DoMyWindow(int windowID)
    {
        impWindow(windowID);
        //print("enableDrag:"+enableDrag);
        if (enableDrag)
            GUI.DragWindow();
        if (alwayFront)
            GUI.BringWindowToFront(GetInstanceID());
    }

    
    public virtual void impWindow(int windowID)
    {
        impSubs();
    }

}