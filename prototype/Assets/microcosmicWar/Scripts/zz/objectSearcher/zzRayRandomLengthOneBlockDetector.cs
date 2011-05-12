using UnityEngine;

public class zzRayRandomLengthOneBlockDetector : zzRayRandomLengthDetector
{
    public LayerMask blockMask;

    public override RaycastHit[] _impDetect(LayerMask pLayerMask)
    {
        //RaycastHit lHit;
        //if (
        //    Physics.Raycast(transform.position, worldDirection, out lHit,
        //        getLength(), pLayerMask | blockMask)
        //    )
        //{
        //    //print(lHit.collider.name);
        //    //print(lHit.collider.gameObject.layer);
        //    //print(((1 << lHit.collider.gameObject.layer) & blockMask) != 0);
        //    //是否为障碍,是则返回0数组
        //    if (
        //        ((1 << lHit.collider.gameObject.layer) & blockMask) != 0
        //        )
        //        return new RaycastHit[0] { };
        //    else
        //        return new RaycastHit[1] { lHit };
        //}
        //return new RaycastHit[0] { };


        return zzRayOneBlockDetector.rayOneBlockDetect(blockMask, pLayerMask,
            transform.position,worldDirection, getLength());
    }

}