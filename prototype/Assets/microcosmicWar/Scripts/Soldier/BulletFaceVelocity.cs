
using UnityEngine;
using System.Collections;

class BulletFaceVelocity : MonoBehaviour
{
    Bullet bullet;
    //public Rigidbody bulletRigidbody;
    void Start()
    {
        if (!bullet)
            bullet = gameObject.GetComponent<Bullet>();
    }

    void Update()
    {
        bullet.setForward(bullet.bulletRigidbody.velocity);
    }
}