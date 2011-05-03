using UnityEngine;

public class zzIgnoreCollision:MonoBehaviour
{
    public Collider collider1;
    public Collider collider2;

    void Awake()
    {
        Physics.IgnoreCollision(collider1, collider2);
        Destroy(this);
    }
}