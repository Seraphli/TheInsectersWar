
using UnityEngine;
using System.Collections;

public class SphereAreaHarm : MonoBehaviour
{

    //用于爆炸的破坏(去血效果)

    //public bool onlyOnce = true;
    public float harmRadius = 5.0f;
    public float harmValueInCentre = 10.0f;

    public float minHarmRate = 0f;

    // 符合 此 mask 的物体,执行伤害
    public LayerMask harmLayerMask = -1;

    //protected Hashtable lInjuredLifeMinDistance = new Hashtable();

    public bool isExplodeType = false;

    public Life.HarmType harmType = Life.HarmType.none;
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
        if (isExplodeType)
        {
            harmType = Life.HarmType.explode;
        }
        if (enabled)
            impSphereAreaHarm(transform.position, harmRadius, minHarmRate, harmValueInCentre,
                harmLayerMask, harmType);
        //if (onlyOnce)
        //{
        enabled = false;
        Destroy(gameObject);
        //}

    }

    public delegate bool canHarmFunc(Life p);

    static bool canHarmAll(Life p)
    {
        return true;

    }

    public static void impSphereAreaHarm(Vector3 pCenterPos, float pHarmRadius,
        float pHarmValueInCentre, int pHarmLayerMask)
    {
        impSphereAreaHarm(pCenterPos, pHarmRadius,0f, pHarmValueInCentre,
            pHarmLayerMask, canHarmAll, Life.HarmType.none);
    }

    public static void impSphereAreaHarm(Vector3 pCenterPos, float pHarmRadius,float pMinRate,
        float pHarmValueInCentre, int pHarmLayerMask, Life.HarmType pHarmType)
    {
        impSphereAreaHarm(pCenterPos, pHarmRadius,pMinRate, pHarmValueInCentre,
            pHarmLayerMask, canHarmAll, pHarmType);
    }

    //执行圆球范围的伤害,伤害随离中心距离增加而递减
    public static void impSphereAreaHarm(Vector3 pCenterPos, float pHarmRadius,float pMinRate,
        float pHarmValueInCentre, int pHarmLayerMask, canHarmFunc pCanHarmFunc,
        Life.HarmType pHarmType)
    {
        //现在用于存储有生命的物体,离爆炸点的最近距离
        Hashtable lInjuredLifeMinDistance = new Hashtable();
        Collider[] lColliderList = Physics.OverlapSphere(pCenterPos, pHarmRadius, pHarmLayerMask);
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
                //获得物体离中心pCenterPos的最近距离
                Vector3 lClosestPoint = i.ClosestPointOnBounds(pCenterPos);
                float lDistance = Vector3.Distance(lClosestPoint, pCenterPos);

                //因为一个有Life的物体上,可能有多个Collider
                if (lInjuredLifeMinDistance.ContainsKey(lLife))
                {
                    float lObjectDistance = (float)lInjuredLifeMinDistance[lLife];
                    if (lObjectDistance > lDistance)
                    {
                        //相同Life时,若距离比之前获得的Collider物体小,则替换
                        //print(""+lInjuredLifeMinDistance[lLife]+">"+lDistance);
                        lInjuredLifeMinDistance[lLife] = lDistance;
                    }
                }
                else
                    lInjuredLifeMinDistance[lLife] = lDistance;
            }

        }

        //执行伤害
        foreach (System.Collections.DictionaryEntry lifeToDistance in lInjuredLifeMinDistance)
        {
            // The hit points we apply fall decrease with distance from the explosion point
            Life lLifeImp = (Life)lifeToDistance.Key;

            //判断是否可以执行伤害
            if(pCanHarmFunc(lLifeImp))
            {
                float lDistanceImp = (float)lifeToDistance.Value;
                float lHarmRange = Mathf.Lerp(pMinRate, 1f, 1.0f - Mathf.Clamp01(lDistanceImp / pHarmRadius));
                //print(lDistanceImp);
                //print(lHarmRange);
                //print(lHarmRange *harmValueInCentre );
                lLifeImp.injure((int)(lHarmRange * pHarmValueInCentre), pHarmType);

            }
        }

        //lInjuredLifeMinDistance.Clear();

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, harmRadius);
    }
}