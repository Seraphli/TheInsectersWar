

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
        if (Network.isServer && networkView)
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

    public GameObject renderObject;

    public float hideTime = 0.05f;

    zzTimer showRenderObjectTimer;

    void hideObject()
    {
        renderObject.SetActiveRecursively(false);
        showRenderObjectTimer = gameObject.AddComponent<zzTimer>();
        showRenderObjectTimer.addImpFunction(showObject);
        showRenderObjectTimer.setInterval(hideTime);
    }

    void showObject()
    {
        renderObject.SetActiveRecursively(true);
        Destroy(showRenderObjectTimer);
    }

    void Start()
    {
        //if (renderObject && zzCreatorUtility.isHost())
        //{
        //    hideObject();
        //}

        if (shape)
        {
            shape.layer = gameObject.layer;
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

    protected virtual void _touch(Transform pOther)
    {

        //有可能在一次运算中 同时碰到多个物体,所以判断之前是否碰撞过;判断子弹的生命值
        if (bulletLife.getBloodValue() <= 0 || Network.isClient)
            return;

        Life lLife = Life.getLifeFromTransform(pOther);

        if (lLife && harmVale!=0)
            lLife.injure(harmVale, injureInfo);
        //print(networkView.viewID);
        //if(zzCreatorUtility.isHost())
        lifeEnd();
    }

    protected void lifeEnd()
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
        setForward(pForward, true);
    }

    //设置子弹飞的方向
    public void setForward(Vector3 pForward,bool pRPCCall)
    {
        pForward.z = 0;

        float lSign = pForward.y > 0f ? 1f : -1f;
        var lRotation = Quaternion.Euler(0f, 0f, lSign*Vector3.Angle(Vector3.right, pForward));
        transform.rotation = lRotation;
        float lSpeed = bulletRigidbody.velocity.magnitude;
        Vector3 lVelocity = pForward.normalized * lSpeed;
        bulletRigidbody.velocity = lVelocity;
        setBulletForwardVelocity(lVelocity, lRotation, pRPCCall);
    }

    void setBulletForwardVelocity(Vector3 pVelocity,Quaternion pRotation,bool pRPCCall)
    {
        if (pRPCCall&&Network.peerType== NetworkPeerType.Server)
        {
            networkView.RPC("RPCSetBulletForwardVelocity", RPCMode.Others, pVelocity, pRotation);
        }
    }

    [RPC]
    void RPCSetBulletForwardVelocity(Vector3 pVelocity,Quaternion pRotation)
    {
        transform.rotation = pRotation;
        bulletRigidbody.velocity = pVelocity;
    }

    //设置子弹飞的方向和速度
    public void setForwardVelocity(Vector3 pVelocity)
    {
        setForwardVelocity(pVelocity,true);
    }


    //设置子弹飞的方向和速度
    public void setForwardVelocity(Vector3 pVelocity, bool pRPCCall)
    {
        pVelocity.z = 0;
        float lSign = pVelocity.y > 0f ? 1f : -1f;
        var lRotation = Quaternion.Euler(0f, 0f, lSign*Vector3.Angle(Vector3.right, pVelocity));
        transform.rotation = lRotation;
        bulletRigidbody.velocity = pVelocity;
        setBulletForwardVelocity(pVelocity, lRotation, pRPCCall);
    }

    //public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    //{
    //    if (stream.isWriting)
    //    {
    //        Vector3 lVelocity = bulletRigidbody.velocity;
    //        stream.Serialize(ref lVelocity);
    //    }
    //    else//(stream.isReading)
    //    {
    //        Vector3 lVelocity = new Vector3();
    //        stream.Serialize(ref lVelocity);
    //        bulletRigidbody.velocity = lVelocity;
    //    }
    //}

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + getForward() * 3);
    }
}