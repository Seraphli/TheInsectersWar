using UnityEngine;
using System.Collections;

public class zzGUIUseCustomPos:MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        var lGUI = GetComponent<zzInterfaceGUI>();
        lGUI.verticalDockPosition = zzGUIDockPos.custom;
        lGUI.horizontalDockPosition = zzGUIDockPos.custom;

        var lRelativePosInfo = lGUI.useRelativePosition;
        lRelativePosInfo.x = false;
        lRelativePosInfo.y = false;

        lGUI.useRelativePosition = lRelativePosInfo;

        Destroy(this);
    }
}