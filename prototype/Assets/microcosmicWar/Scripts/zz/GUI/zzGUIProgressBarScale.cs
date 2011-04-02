
using UnityEngine;
using System.Collections;

public class zzGUIProgressBarScale : zzGUIProgressBarBase
{
    public override void impGUI(Rect rect)
    {
        zzInterfaceGUI[] lSubs = gameObject.GetComponentsInChildren<zzInterfaceGUI>();
        foreach (var lSub in lSubs)
        {
            Rect lRelativePosition = lSub.relativePosition;
            if (direction == zzGUIDirection.horizontal)
                lRelativePosition.width = rate;
            else
                lRelativePosition.height = rate;
            lSub.relativePosition = lRelativePosition;
        }
        base.impGUI(rect);
    }
}