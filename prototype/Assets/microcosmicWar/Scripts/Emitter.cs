using UnityEngine;
using System.Collections;


public class Emitter : MonoBehaviour
{


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
            zzCreatorUtility.sendMessage(gameObject, "_SetBulletAliveTime", value);
        }
    }

    [RPC]
    void _SetBulletAliveTime(float pBulletAliveTime)
    {
        _bulletAliveTime = pBulletAliveTime;
    }

    protected float _bulletAliveTime;

    public virtual void setInjureInfo(Hashtable pInjureInfo)
    {
        injureInfo = pInjureInfo;
    }

    void Start()
    {
        _bulletAliveTime = shootRange / bulletSpeed;
    }

    //void Update()
    //{
    //}

    public virtual GameObject[] EmitBullet()
    {
        GameObject lOut = null;
        if (zzCreatorUtility.isHost())
        {
            var lPosition = transform.position;
            lPosition.z=0f;
            lOut = zzCreatorUtility.Instantiate(bulletPrefab, lPosition, Quaternion.identity, 0);

            Bullet pBullet = lOut.GetComponentInChildren<Bullet>();
            pBullet.setLayer(bulletLayer);
            pBullet.setAliveTime(_bulletAliveTime);

            pBullet.setForwardVelocity(getForward() * bulletSpeed);
            if (injureInfo!=null)
            {
                pBullet.setInjureInfo(injureInfo);
            }

        }
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
            clone = (GameObject)Instantiate(fireSpark, transform.position, transform.rotation);
            FireSpark lFireSpark = clone.GetComponent<FireSpark>();
            lFireSpark.setForward(getForward());
        }
    }

    public virtual void setBulletLayer(int pBulletLayer)
    {
        //print("Emit.setBulletLayer");
        bulletLayer = pBulletLayer;
    }

    public virtual Vector3 getForward()
    {
        //return transform.right;
        return transform.localToWorldMatrix.MultiplyVector(Vector3.right);
    }

    public virtual Ray getFireRay()
    {
        return new Ray(transform.position, getForward());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + getForward() * shootRange);
    }

    public bool showGizmo = false;

    void OnDrawGizmos()
    {
        if (showGizmo)
            Gizmos.DrawLine(transform.position, transform.position + getForward() * shootRange);
    }
}