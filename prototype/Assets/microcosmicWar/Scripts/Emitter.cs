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

    protected Hashtable injureInfo;
    protected float bulletAliveTime;

    public delegate void initBulletNullFunc(Bullet pBullet);

    static void nullInitBulletNullFunc(Bullet pBullet){}
    //protected void  initBulletNullFunc ( Bullet pBullet  ){
    //}

    protected initBulletNullFunc initBulletFunc = nullInitBulletNullFunc;

    public void setInitBulletFunc(initBulletNullFunc pFunc)
    {
        initBulletFunc = pFunc;
    }

    public virtual void setInjureInfo(Hashtable pInjureInfo)
    {
        injureInfo = pInjureInfo;
    }

    void Start()
    {
        bulletAliveTime = shootRange / bulletSpeed;
        //print(""+bulletAliveTime+"="+shootRange"//"+bulletSpeed);
    }

    void Update()
    {
    }

    public virtual void EmitBullet()
    {
        if (zzCreatorUtility.isHost())
        {
            GameObject clone;
            clone = zzCreatorUtility.Instantiate(bulletPrefab, transform.position, transform.rotation, 0);
            clone.layer = bulletLayer;

            //print(transform.localToWorldMatrix.MultiplyVector(Vector3(1,0,0)) );
            //Rigidbody lRigidbody = clone.GetComponentInChildren<Rigidbody>();
            //lRigidbody.velocity=transform.localToWorldMatrix.MultiplyVector(Vector3(1,0,0))*bulletSpeed;
            //clone.velocity=transform.forward;
            Bullet pBullet = clone.GetComponentInChildren<Bullet>();
            pBullet.setAliveTime(bulletAliveTime);
            //pBullet.setForward(getForward());
            //pBullet.setSpeed(bulletSpeed);
            pBullet.setForwardVelocity(getForward() * bulletSpeed);
            if (injureInfo!=null)
            {
                pBullet.setInjureInfo(injureInfo);
            }

            //执行外部的初始化子弹的函数
            initBulletFunc(pBullet);
        }
        if (fireSound)
        {
            fireSound.Play();
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
}