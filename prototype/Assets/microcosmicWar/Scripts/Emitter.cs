using UnityEngine;
using System.Collections;


public class Emitter : MonoBehaviour
{
    public Transform emitterTransform;

    public GameObject bulletPrefab;
    public float bulletSpeed = 20.0f;

    //射程
    public float shootRange = 10.0f;

    public int bulletLayer = 0;

    public AudioSource fireSound;
    public GameObject fireSpark;

    protected Hashtable injureInfo;

    public float bulletAliveTime
    {
        get { return _bulletAliveTime; }
        set 
        {
            //_bulletAliveTime = value;
            //zzCreatorUtility.sendMessage(gameObject, "_SetBulletAliveTime", value);
            _bulletAliveTime = value;
        }
    }

    //[RPC]
    //void _SetBulletAliveTime(float pBulletAliveTime)
    //{
    //    _bulletAliveTime = pBulletAliveTime;
    //}

    protected float _bulletAliveTime;

    public virtual void setInjureInfo(Hashtable pInjureInfo)
    {
        injureInfo = pInjureInfo;
    }

    public void Reset()
    {
        emitterTransform = transform;
    }

    void Start()
    {
        _bulletAliveTime = shootRange / bulletSpeed;
    }

    //void Update()
    //{
    //}
    GameObject createBullet(Vector3 pPos, Vector3 ForwardVelocity)
    {
        var lBullet = (GameObject)Instantiate(bulletPrefab);
        setBullet(lBullet.GetComponent<Bullet>(), pPos, ForwardVelocity);
        return lBullet;
    }

    [RPC]
    GameObject EmitterSetBullet(NetworkViewID pViewID, Vector3 pPos, Vector3 ForwardVelocity)
    {
        var lBullet = (GameObject)Instantiate(bulletPrefab);
        lBullet.networkView.viewID = pViewID;
        setBullet(lBullet.GetComponent<Bullet>(), pPos, ForwardVelocity);
        return lBullet;
    }

    void setBullet(Bullet pBullet,Vector3 pPos, Vector3 ForwardVelocity)
    {
        pPos.z = 0;
        pBullet.transform.position = pPos;
        pBullet.setLayer(bulletLayer);
        pBullet.setAliveTime(_bulletAliveTime);
        pBullet.setForwardVelocity(ForwardVelocity);
        if (injureInfo != null)
        {
            pBullet.setInjureInfo(injureInfo);
        }
    }

    public virtual void getBulletInfo(out Vector3 lPosition,out Vector3 pForwardVelocity)
    {
        lPosition = emitterTransform.position;
        pForwardVelocity = getForward() * bulletSpeed;
    }

    public virtual GameObject[] EmitBullet()
    {
        GameObject lOut = null;
        Vector3 lPos;
        Vector3 lForwardVelocity;
        getBulletInfo(out lPos, out lForwardVelocity);
        if (!bulletPrefab.networkView
            || Network.peerType == NetworkPeerType.Disconnected)
        {
            lOut = createBullet(lPos, lForwardVelocity);
        }
        else if (Network.isServer)
        {
            var lID = Network.AllocateViewID();
            lOut = EmitterSetBullet(lID, lPos, lForwardVelocity);
            networkView.RPC("EmitterSetBullet", RPCMode.Others, lID, lPos, lForwardVelocity);
        }
        //if (zzCreatorUtility.isHost())
        //{
        //    var lPosition = emitterTransform.position;
        //    lPosition.z = 0f;
        //    lOut = zzCreatorUtility.Instantiate(bulletPrefab, lPosition, Quaternion.identity, 0);

        //    Bullet pBullet = lOut.GetComponentInChildren<Bullet>();
        //    pBullet.setLayer(bulletLayer);
        //    pBullet.setAliveTime(_bulletAliveTime);

        //    pBullet.setForwardVelocity(getForward() * bulletSpeed);

        //}
        if (fireSound)
        {
            fireSound.Play();
        }
        playFireSpark();

        return new GameObject[]{lOut};
    }

    protected void playFireSpark()
    {
        if (fireSpark)
        {
            GameObject clone;
            clone = (GameObject)Instantiate(fireSpark, emitterTransform.position, emitterTransform.rotation);
            FireSpark lFireSpark = clone.GetComponent<FireSpark>();
            lFireSpark.setForward(getForward());
        }
    }

    public virtual void setBulletLayer(int pBulletLayer)
    {
        //print("Emit.setBulletLayer");
        bulletLayer = pBulletLayer;
    }

    public Vector3 getForward()
    {
        //return emitterTransform.right;
        return emitterTransform.localToWorldMatrix.MultiplyVector(Vector3.right);
    }

    public Ray getFireRay()
    {
        return new Ray(emitterTransform.position, getForward());
    }

    void OnDrawGizmosSelected()
    {
        if (emitterTransform)
            Gizmos.DrawLine(emitterTransform.position, emitterTransform.position + getForward() * shootRange);
    }

    public bool showGizmo = false;

    void OnDrawGizmos()
    {
        if (showGizmo && emitterTransform)
            Gizmos.DrawLine(emitterTransform.position, emitterTransform.position + getForward() * shootRange);
    }
}