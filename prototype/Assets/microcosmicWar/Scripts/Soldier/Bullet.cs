﻿

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

    //protected Hashtable injureInfo;


    public Rigidbody bulletRigidbody;

    public Transform particleEmitter;
    public bool hitObject = false;

    CharacterInfo _characterInfo;

    [ContextMenu("SetCharacterInfo")]
    void setCharacterInfo()
    {
        CharacterInfoObject lCharacterInfoObject
            = gameObject.GetComponent<CharacterInfoObject>();
        if (!lCharacterInfoObject)
            lCharacterInfoObject = gameObject.AddComponent<CharacterInfoObject>();
        characterInfoObject = lCharacterInfoObject;
    }

    public CharacterInfoObject characterInfoObject;
    public CharacterInfo characterInfo
    {
        set
        {
            _characterInfo = value;
            characterInfoObject.characterInfo = value;
        }

        get { return _characterInfo; }
    }

    void Awake()
    {
        bulletRigidbody = gameObject.GetComponent<Rigidbody>();
        bulletLife = gameObject.GetComponent<Life>();
        bulletLife.addDieCallback(lifeEndImp);
    }

    //public void setInjureInfo(Hashtable pInjureInfo)
    //{
    //    injureInfo = pInjureInfo;
    //}


    public void setLayer(int pLayer)
    {
        _setLayer(pLayer);
        //if (Network.isServer && networkView)
        //{
        //    networkView.RPC("_setLayer", RPCMode.Others, pLayer);
        //}
    }

    //[RPC]
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

    //防止其在Start前执行
    System.Action hitObjectEvent = nullEventReceiver;
    public void addHitObjectReceiver(System.Action pReceiver)
    {
        hitObjectEvent = pReceiver;
    }

    System.Action notHitObjectEvent = nullEventReceiver;
    public void addNotHitObjectReceiver(System.Action pReceiver)
    {
        notHitObjectEvent = pReceiver;
    }

    static void nullEventReceiver(){}

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
        if (!(Network.isClient&&networkView))
        {
            aliveTime -= Time.deltaTime;
            if (aliveTime < 0)
                //zzCreatorUtility.Destroy (gameObject);
                lifeEnd();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        foreach (var lContacts in collision.contacts)
        {
            _touch(lContacts.otherCollider.transform);
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (!collider.isTrigger)
            _touch(collider.transform);
    }

    protected virtual void _touch(Transform pOther)
    {
        hitObject = true;
        //Network.isClient的判断,在Life中进行
        //if(Network.isClient)
        //{
        //    if(!networkView
        //        || networkView.viewID == NetworkViewID.unassigned)
        //        lifeEnd();
        //    return;
        //}

        //有可能在一次运算中 同时碰到多个物体,所以判断之前是否碰撞过;判断子弹的生命值
        if (bulletLife.getBloodValue() <= 0)
            return;
        Life lLife = Life.getLifeFromTransform(pOther);
        if (lLife && harmVale!=0)
            lLife.injure(harmVale, characterInfo);
        //print(networkView.viewID);
        //if(zzCreatorUtility.isHost())
        lifeEnd();
    }

    protected void lifeEnd()
    {
        //	zzCreatorUtility.sendMessage(gameObject,"lifeEndImp");
        bulletLife.makeDead();
    }

    public bool changeColliderWhenEnd = false;

    //[RPC]
    void lifeEndImp(Life p)
    {
        if (changeColliderWhenEnd)
        {
            foreach (var lCollider in GetComponentsInChildren<Collider>())
            {
                lCollider.gameObject.layer = layers.deadObject;
            }
            if (hitObject)
            {
                rigidbody.isKinematic = true;
                hitObjectEvent();
            }
            else
            {
                notHitObjectEvent();
            }
        }

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
        //setBulletForwardVelocity(lVelocity, lRotation, pRPCCall);
    }

    //void setBulletForwardVelocity(Vector3 pVelocity,Quaternion pRotation,bool pRPCCall)
    //{
    //    if (pRPCCall && Network.peerType == NetworkPeerType.Server && networkView)
    //    {
    //        networkView.RPC("RPCSetBulletForwardVelocity", RPCMode.Others, pVelocity, pRotation);
    //    }
    //}

    //[RPC]
    //void RPCSetBulletForwardVelocity(Vector3 pVelocity,Quaternion pRotation)
    //{
    //    transform.rotation = pRotation;
    //    bulletRigidbody.velocity = pVelocity;
    //}

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
        //setBulletForwardVelocity(pVelocity, lRotation, pRPCCall);
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