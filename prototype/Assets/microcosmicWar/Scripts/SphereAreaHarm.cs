
using UnityEngine;
using System.Collections;

public class SphereAreaHarm : MonoBehaviour
{

    //用于爆炸的破坏(去血效果)

    public bool onlyOnce = true;
    public float harmRadius = 5.0f;
    public float harmValueInCentre = 10.0f;

    // 符合 此 mask 的物体,执行伤害
    public int harmLayerMask;

    //[Life]=Distance  现在用于存储美工有生命挝?离爆炸点的最近距离
    protected Hashtable injuredLifeInTheFrame = new Hashtable();


    public void setHarmLayerMask(int pMark)
    {
        harmLayerMask = pMark;
        //print(harmLayerMask);
        //print(transform.position);
    }

    void Update()
    {
        /*
            print("harmLayerMask");
            print(transform.position);
            print("harmRadius:"+harmRadius);
            print("harmValueInCentre:"+harmValueInCentre);
        */
        Collider[] lColliderList = Physics.OverlapSphere(transform.position, harmRadius, harmLayerMask);
        //搜寻范围内的Life,并寻找最短距离
        foreach (Collider i in lColliderList)
        {

            //Trigger 也会被探测到
            if (i.isTrigger)
                continue;
            Life lLife = Life.getLifeFromTransform(i.transform);
            if (lLife)
            {
                //lLife.injure(harmValueInCentre);
                Vector3 lClosestPoint = i.ClosestPointOnBounds(transform.position);
                float lDistance = Vector3.Distance(lClosestPoint, transform.position);

                //因为一个有Life的物体上,可能有多个Collider
                if (injuredLifeInTheFrame.ContainsKey(lLife))
                {
                    float lObjectDistance = (float)injuredLifeInTheFrame[lLife];
                    if (lObjectDistance > lDistance)
                    {
                        //print(""+injuredLifeInTheFrame[lLife]+">"+lDistance);
                        injuredLifeInTheFrame[lLife] = lDistance;
                    }
                }
                else
                    injuredLifeInTheFrame[lLife] = lDistance;
            }

        }

        //执行伤害
        foreach (System.Collections.DictionaryEntry lifeToDistance in injuredLifeInTheFrame)
        {
            // The hit points we apply fall decrease with distance from the explosion point
            Life lLifeImp = (Life)lifeToDistance.Key;
            float lDistanceImp = (float)lifeToDistance.Value;
            float lHarmRange = 1.0f - Mathf.Clamp01(lDistanceImp / harmRadius);
            //print(lDistanceImp);
            //print(lHarmRange);
            //print(lHarmRange *harmValueInCentre );
            lLifeImp.injure((int)(lHarmRange * harmValueInCentre));
        }

        injuredLifeInTheFrame.Clear();
        if (onlyOnce)
        {
            Destroy(gameObject);
        }

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, harmRadius);
    }
}