// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    //@script RequireComponent(Rigidbody)
    //@script RequireComponent(Life)
    //@script RequireComponent(destroyWhenDie)

    public float aliveTime = 5.0f;
    public int harmVale = 1;
    public Life bulletLife;
    public GameObject shape;

    protected Hashtable injureInfo;


    public Rigidbody bulletRigidbody;

    void Awake()
    {
        bulletRigidbody = gameObject.GetComponent<Rigidbody>();
        bulletLife = gameObject.GetComponent<Life>();
        bulletLife.addDieCallback(lifeEndImp);
    }

    public void setInjureInfo(Hashtable pInjureInfo)
    {
        injureInfo = pInjureInfo;
    }

    void Start()
    {
        if (shape)
        {
            //print("collisionLayer.addCollider(shape)");
            shape.layer = gameObject.layer;
            collisionLayer.addCollider(shape);
        }
        else
        {
            //print("collisionLayer.addCollider(gameObject)");
            collisionLayer.addCollider(gameObject);
        }
        //print(gameObject.layer);
        //if(!zzCreatorUtility.isHost())
        //	Destroy(this);
    }

    public void setAliveTime(float pAliveTime)
    {
        //print(pAliveTime);
        aliveTime = pAliveTime;
    }

    void Update()
    {
        if (zzCreatorUtility.isHost())
        {
            aliveTime -= Time.deltaTime;
            if (aliveTime < 0)
                //zzCreatorUtility.Destroy (gameObject);
                lifeEnd();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //print("OnCollisionEnter");
        //Transform lOwner = collision.transform;
        //Life lLife=lOwner.gameObject.GetComponentInChildren<Life>();
        /*
        Life lLife=lOwner.gameObject.GetComponent<Life>();
	
        if(!lLife)
        {
            while(lOwner.parent)
            {
                lOwner=lOwner.parent;
                lLife = lOwner.gameObject.GetComponent<Life>();
                if(lLife)
                    break;
            }
        }
        */
        Life lLife = Life.getLifeFromTransform(collision.transform);

        if (lLife)
            lLife.injure(harmVale, injureInfo);

        //if(zzCreatorUtility.isHost())
        lifeEnd();
    }

    void lifeEnd()
    {
        //	zzCreatorUtility.sendMessage(gameObject,"lifeEndImp");
        bulletLife.setBloodValue(0);
    }

    //[RPC]
    void lifeEndImp(Life p)
    {
        particleEmitterDetach();
        //zzCreatorUtility.sendMessage(gameObject,"particleEmitterDetach");
        //Destroy(gameObject);
    }

    ////@RPC
    void particleEmitterDetach()
    {
        Transform lTransform = transform.Find("particleEmitter");
        if (lTransform)
        {
            //防止同时执行Update 和 OnCollisionEnter的情况
            ParticleEmitter lParticleEmitter = lTransform.gameObject.GetComponent<ParticleEmitter>();
            lParticleEmitter.emit = false;
            lTransform.parent = null;
        }
        //脱离所有子物体 临时
    }

    public Vector3 getForward()
    {
        return transform.rotation * Vector3.right;
    }

    //设置子弹飞的方向
    public void setForward(Vector3 pForward)
    {
        pForward.z = 0;
        Quaternion lRotation = new Quaternion();
        lRotation.SetFromToRotation(Vector3.right, pForward);
        //transform.rotation.SetFromToRotation(Vector3.right,pForward);
        transform.rotation = lRotation;
        float lSpeed = bulletRigidbody.velocity.magnitude;
        bulletRigidbody.velocity = pForward.normalized * lSpeed;
    }


    //设置子弹飞的方向和速度
    public void setForwardVelocity(Vector3 pVelocity)
    {
        pVelocity.z = 0;
        transform.rotation.SetFromToRotation(Vector3.right, pVelocity);
        bulletRigidbody.velocity = pVelocity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + getForward() * 3);
    }
}