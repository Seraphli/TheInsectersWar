
using UnityEngine;
using System.Collections;

public class zzGUIProgressBar : zzGUIGroup
{
    public zzGUIDirection direction = zzGUIDirection.horizontal;
    public float rate;

    //public override void impSubs()

    public override void impGUI(Rect rect)
    {
        zzImageGUI[] lImages = gameObject.GetComponentsInChildren<zzImageGUI>();
        foreach (var lImage in lImages)
        {
            Rect lRelativePosition = lImage.relativePosition;
            if (direction == zzGUIDirection.horizontal )
                lRelativePosition.width = rate;
            else
                lRelativePosition.height = rate;
            lImage.relativePosition = lRelativePosition;
        }
        base.impGUI(rect);
    }
}