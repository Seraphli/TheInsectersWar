
using UnityEngine;
using System.Collections;


public class zzRayOneBlockDetector : zzRayDetectorBase
{
    public LayerMask blockMask;

    public static RaycastHit[] rayOneBlockDetect(LayerMask pBlockMask,LayerMask pLayerMask,
        Vector3 pOrigin, Vector3 pDirection, float pDistance)
    {
        RaycastHit lHit;
        if (
            Physics.Raycast(pOrigin, pDirection, out lHit,
                pDistance, pLayerMask | pBlockMask)
            )
        {
            //是否为障碍,是则返回0数组
            if (
                ((1 << lHit.collider.gameObject.layer) & pBlockMask) != 0
                )
                return new RaycastHit[0] { };
            else
                return new RaycastHit[1] { lHit };
        }
        return new RaycastHit[0] { };

    }

    public override RaycastHit[] _impDetect(LayerMask pLayerMask)
    {
        return rayOneBlockDetect(blockMask, pLayerMask, getOrigin(),
            getDirection(), getDistance());
    }

}
