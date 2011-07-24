using UnityEngine;

public class zzGUIScaleToFit:zzGUIGroup
{
    public float aspectRatio = 1f;

    public override Rect calculatePosition()
    {
        Rect lOut = new Rect();

        lOut.width = calculateWidth();
        lOut.height = calculateHeight();
        if ((lOut.height / lOut.width) > aspectRatio)
            lOut.height = lOut.width * aspectRatio;
        else
            lOut.width = lOut.height / aspectRatio;
        calculatePosition(ref lOut);
        return lOut;
    }
}