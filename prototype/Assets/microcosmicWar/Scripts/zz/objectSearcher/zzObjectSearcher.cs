
using UnityEngine;
using System.Collections;

public class zzObjectSearcher : MonoBehaviour
{


    public zzDetectorBase detector;

    public void setDetector(zzDetectorBase p)
    {
        detector = p;
    }

    public zzDetectorBase getDetector(zzDetectorBase p)
    {
        return detector;
    }
}