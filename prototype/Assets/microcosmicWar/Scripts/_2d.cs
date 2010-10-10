using UnityEngine;
using System.Collections;

public class _2d : MonoBehaviour
{
    public Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }

    void Update()
    {
        Vector3 lPrePosition = myTransform.position;
        lPrePosition.z = 0.0f;
        myTransform.position = lPrePosition;
    }
}