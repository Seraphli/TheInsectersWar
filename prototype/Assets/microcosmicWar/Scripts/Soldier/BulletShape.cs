using UnityEngine;

public class BulletShape:MonoBehaviour
{
    public Bullet bullet;

    void OnCollisionEnter(Collision pCollision)
    {
        bullet.OnCollisionEnter(pCollision);
    }

    void OnTriggerEnter(Collider pCollider)
    {
        bullet.OnTriggerEnter(pCollider);
    }
}