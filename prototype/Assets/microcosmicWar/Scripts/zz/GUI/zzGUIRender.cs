
using UnityEngine;
using System.Collections;



[ExecuteInEditMode]
public class zzGUIRender : zzGUIContainer
{
    void Start()
    {
        zzGUI._setRoot(this);
    }

    void OnGUI()
    {
        //impSubs();
        //zzGUI.renderGUI(this);
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