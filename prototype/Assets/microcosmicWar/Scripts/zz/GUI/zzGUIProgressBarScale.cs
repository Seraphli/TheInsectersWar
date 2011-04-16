
using UnityEngine;
using System.Collections;

public class zzGUIProgressBarScale : zzGUIProgressBarBase
{
    public override void impGUI(Rect rect)
    {
        foreach (Transform lSubTransform in transform)
        {
            var lSub = lSubTransform.GetComponent<zzInterfaceGUI>();
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