
using UnityEngine;
using System.Collections;
public class zzGUIRenderDebuger : zzGUIContainer
{
    public void drawGUI()
    {
        zzGUI._setRoot(this);
        renderGUI();
    }

    public override Rect calculatePosition()
    {
        return position;
    }

}