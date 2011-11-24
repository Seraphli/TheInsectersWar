using UnityEngine;

public class zzFollow:MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}