
using UnityEngine;
using System.Collections;



[ExecuteInEditMode]
public class zzGUIRender : zzGUIContainer
{
    void OnGUI()
    {
        //impSubs();
        zzGUI.impGUI(this);
    }

    public override float getWidth()
    {
        return Screen.width;
    }

    public override float getHeight()
    {
        return Screen.height;
    }
}