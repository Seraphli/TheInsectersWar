

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

    public Transform particleEmitter;

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


    public void setLayer(int pLayer)
    {
        _setLayer(pLayer);
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            networkView.RPC("_setLayer", RPCMode.Others, pLayer);
        }
    }

    [RPC]
    void _setLayer(int pLayer)
    {
        gameObject.layer = pLayer;
        if (shape)
            shape.layer = pLayer;
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
        _touch(collision.transform);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.isTrigger)
            _touch(collider.transform);
    }

    void _touch(Transform pOther)
    {

        //有可能在一次运算中 同时碰到多个物体,所以判断之前是否碰撞过;判断子弹的生命值
        if (bulletLife.getBloodValue() <= 0 || !zzCreatorUtility.isHost())
            return;

        Life lLife = Life.getLifeFromTransform(pOther);

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
        if(!particleEmitter)
            particleEmitter = transform.Find("particleEmitter");
        //Transform lTransform = transform.Find("particleEmitter");
        if (particleEmitter)
        {
            //防止同时执行Update 和 OnCollisionEnter的情况
            ParticleEmitter lParticleEmitter = particleEmitter.gameObject.GetComponent<ParticleEmitter>();
            lParticleEmitter.emit = false;
            particleEmitter.parent = null;
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
        var lRotation = new Quaternion();
        lRotation.SetFromToRotation(Vector3.right, pVelocity);
        transform.rotation = lRotation;
        bulletRigidbody.velocity = pVelocity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + getForward() * 3);
    }
}