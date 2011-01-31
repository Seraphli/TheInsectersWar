using UnityEngine;
using System.Collections;

public class RandomScale:MonoBehaviour
{
    public Transform scaleObject;
    public float maxScale;
    public float minScale;

    void Start()
    {
        if (!scaleObject)
            scaleObject = transform;
        var lScale = scaleObject.localScale;
        lScale.x *= Random.Range(minScale, maxScale);
        scaleObject.localScale = lScale;
    }
}