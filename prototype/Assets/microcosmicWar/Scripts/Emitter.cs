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

    //public delegate void initBulletNullFunc(Bullet pBullet);

    //static void nullInitBulletNullFunc(Bullet pBullet){}
    //protected void  initBulletNullFunc ( Bullet pBullet  ){
    //}

    //protected initBulletNullFunc initBulletFunc = nullInitBulletNullFunc;

    //public void setInitBulletFunc(initBulletNullFunc pFunc)
    //{
    //    initBulletFunc = pFunc;
    //}

    public virtual void setInjureInfo(Hashtable pInjureInfo)
    {
        injureInfo = pInjureInfo;
    }

    void Start()
    {
        _bulletAliveTime = shootRange / bulletSpeed;
        //print(""+bulletAliveTime+"="+shootRange"//"+bulletSpeed);
    }

    void Update()
    {
    }

    public virtual GameObject[] EmitBullet()
    {
        GameObject lOut = null;
        if (zzCreatorUtility.isHost())
        {
            lOut = zzCreatorUtility.Instantiate(bulletPrefab, transform.position, new Quaternion(), 0);
            //clone.layer = bulletLayer;

            //print(transform.localToWorldMatrix.MultiplyVector(Vector3(1,0,0)) );
            //Rigidbody lRigidbody = clone.GetComponentInChildren<Rigidbody>();
            //lRigidbody.velocity=transform.localToWorldMatrix.MultiplyVector(Vector3(1,0,0))*bulletSpeed;
            //clone.velocity=transform.forward;
            Bullet pBullet = lOut.GetComponentInChildren<Bullet>();
            pBullet.setLayer(bulletLayer);
            pBullet.setAliveTime(_bulletAliveTime);
            //pBullet.setForward(getForward());
            //pBullet.setSpeed(bulletSpeed);
            pBullet.setForwardVelocity(getForward() * bulletSpeed);
            if (injureInfo!=null)
            {
                pBullet.setInjureInfo(injureInfo);
            }

            //执行外部的初始化子弹的函数
            //initBulletFunc(pBullet);
        }
        if (fireSound)
        {
            fireSound.Play();
        }

        if(fireSpark)
        {
            GameObject clone;
            clone = (GameObject)Instantiate(fireSpark, transform.position, transform.rotation);
            FireSpark lFireSpark = clone.GetComponent<FireSpark>();
            lFireSpark.setForward(getForward());
        }
        return new GameObject[]{lOut};
    }

    public virtual void setBulletLayer(int pBulletLayer)
    {
        //print("Emit.setBulletLayer");
        bulletLayer = pBulletLayer;
    }

    public virtual Vector3 getForward()
    {
        //return transform.right;
        return transform.localToWorldMatrix.MultiplyVector(new Vector3(1, 0, 0));
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