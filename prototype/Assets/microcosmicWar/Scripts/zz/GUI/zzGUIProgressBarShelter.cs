
using UnityEngine;
using System.Collections;

public class zzGUIProgressBarShelter : zzGUIProgressBarBase
{
    public override void impGUI(Rect rect)
    {
        //zzImageGUI[] lImages = gameObject.GetComponentsInChildren<zzImageGUI>();
        //foreach (var lImage in lImages)
        //{
        //    Rect lRelativePosition = lImage.relativePosition;
        //    if (direction == zzGUIDirection.horizontal )
        //        lRelativePosition.width = rate;
        //    else
        //        lRelativePosition.height = rate;
        //    lImage.relativePosition = lRelativePosition;
        //}
        if (direction == zzGUIDirection.horizontal)
            rect.width *= rate;
        else
            rect.height *= rate;
        base.impGUI(rect);
    }
}