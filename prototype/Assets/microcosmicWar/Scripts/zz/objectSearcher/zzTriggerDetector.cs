using UnityEngine;
using System.Collections.Generic;

public class zzTriggerDetector : zzDetectorBase
{
    HashSet<Collider> detectedList = new HashSet<Collider>();

    [SerializeField]
    LayerMask detectLayerMask;

    static Collider[] nullReturn = new Collider[0]{};

    public override Collider[] detect(int pMaxRequired, LayerMask pLayerMask, detectorFilterFunc pNeedDetectedFunc)
    {
        if(detectedList.Count==0)
            return nullReturn;

        List<Collider> lOut = new List<Collider>(detectedList.Count);
        List<Collider> lRemoved = new List<Collider>();
        foreach (var lCollider in detectedList)
        {
            if (lCollider 
                && ((1 << lCollider.gameObject.layer) & detectLayerMask.value)!=0 
                )
                lOut.Add(lCollider);
            else
                lRemoved.Add(lCollider);
        }
        foreach (var lCollider in lRemoved)
        {
            removeDetectedObject(lCollider);
        }
        return lOut.ToArray();
    }


    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & detectLayerMask.value) != 0)
        {
            addDetectedObject(other);
        }
    }

    protected virtual void addDetectedObject(Collider other)
    {
        detectedList.Add(other);
    }

    protected virtual void removeDetectedObject(Collider other)
    {
        detectedList.Remove(other);
    }


    void OnTriggerExit(Collider other)
    {
        if (detectedList.Contains(other))
            removeDetectedObject(other);
    }


}