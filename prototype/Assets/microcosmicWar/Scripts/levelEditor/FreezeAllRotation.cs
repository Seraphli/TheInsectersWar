using UnityEngine;
using System.Collections;


public class FreezeAllRotation:MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}