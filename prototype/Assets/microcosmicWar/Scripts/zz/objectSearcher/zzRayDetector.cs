
using UnityEngine;
using System.Collections;


public class zzRayDetector : zzRayDetectorBase
{
    public override RaycastHit[] _impDetect(LayerMask pLayerMask)
    {
        return Physics.RaycastAll(getOrigin(), getDirection(), getDistance(), pLayerMask);
   
    }

}
