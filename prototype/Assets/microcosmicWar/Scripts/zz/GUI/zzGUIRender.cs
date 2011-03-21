
using UnityEngine;
using System.Collections;



[ExecuteInEditMode]
public class zzGUIRender : zzGUIContainer
{
    void OnGUI()
    {
        zzGUI._setRoot(this);
        renderGUI();
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