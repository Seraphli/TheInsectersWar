
using UnityEngine;
using System.Collections;

public class BulletFollowAI : MonoBehaviour
{


    public Transform target;
    public Bullet bullet;

    void Start()
    {
        //target = gameObject.Find("BulletFollowAITest").transform;s
    }

    public void setTarget(Transform pTarget)
    {
        target = pTarget;
    }

    void Update()
    {
        if (target)
        {
            Vector3 lToAim = target.position - bullet.transform.position;
            bullet.setForward(Vector3.Lerp(bullet.getForward(), lToAim.normalized, 2 * Time.deltaTime));
        }
        //print("getForward"+bullet.getForward());
        //print(lToAim );
        //print(Vector3.Lerp(bullet.getForward(),lToAim,0.6f*Time.deltaTime));
    }
}