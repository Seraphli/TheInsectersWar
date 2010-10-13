
using UnityEngine;
using System.Collections;

public class SonicAttack : MonoBehaviour
{
    public float harmRadius = 5.0f;
    public float harmValueInCentre = 400.0f;
    int mylayer;
    public int harmLayerMask;

    //protected FIXME_VAR_TYPE injuredLifeInTheFrame = Hashtable();


    public void setHarmLayerMask(int pMark)
    {
        harmLayerMask = pMark;
    }

    //void Start()
    //{
    //    harmLayerMask = PlayerInfo.getAdversaryRaceLayer(gameObject.layer);
    //}

    //判断是否可以伤害
    bool canHarm(Life pLife)
    {
        ObjectProperty AcousticTowerTemp = pLife.gameObject.GetComponent<ObjectProperty>();
        print(pLife.gameObject.name);
        if (AcousticTowerTemp
            && AcousticTowerTemp.identity == Identitys.Tower)
        {
            return false;
        }
        return true;

    }

    public void Attack()
    {
        SphereAreaHarm.impSphereAreaHarm(transform.position, harmRadius, harmValueInCentre, harmLayerMask, canHarm);

        //Collider[] lColliderList = Physics.OverlapSphere(transform.position, harmRadius, 1 << harmLayerMask);
        ////print("asd");
        ////搜寻范围内的Life,并寻找最短距离
        //foreach (Collider i in lColliderList)
        //{

        //    //print(i);
        //    //Trigger 也会被探测到
        //    if (i.isTrigger)
        //        continue;
        //    Life lLife = Life.getLifeFromTransform(i.transform);
        //    if (lLife)
        //    {
        //        //lLife.injure(harmValueInCentre);
        //        FIXME_VAR_TYPE lClosestPoint = i.ClosestPointOnBounds(transform.position);
        //        FIXME_VAR_TYPE lDistance = Vector3.Distance(lClosestPoint, transform.position);

        //        //因为一个有Life的物体上,可能有多个Collider
        //        if (injuredLifeInTheFrame.ContainsKey(lLife))
        //        {
        //            if (injuredLifeInTheFrame[lLife] > lDistance)
        //            {
        //                //print(""+injuredLifeInTheFrame[lLife]+">"+lDistance);
        //                injuredLifeInTheFrame[lLife] = lDistance;
        //            }
        //        }
        //        else
        //            injuredLifeInTheFrame[lLife] = lDistance;
        //    }

        //}

        ////执行伤害
        //foreach (System.Collections.DictionaryEntry lifeToDistance in injuredLifeInTheFrame)
        //{
        //    // The hit points we apply fall decrease with distance from the explosion point
        //    Life lLifeImp = lifeToDistance.Key;
        //    ObjectProperty AcousticTowerTemp = lLifeImp.gameObject.GetComponent<ObjectProperty>();
        //    if (lLifeImp.gameObject.layer == gameObject.layer)
        //        continue;
        //    if (AcousticTowerTemp)
        //    {
        //        if (AcousticTowerTemp.identity == Identitys.Tower)
        //        {
        //            //print("aa");
        //            continue;
        //        }
        //    }
        //    float lDistanceImp = lifeToDistance.Value;
        //    float lHarmRange = 1.0f - Mathf.Clamp01(lDistanceImp / harmRadius);
        //    //print(lDistanceImp);
        //    //print(lHarmRange);
        //    //print(lHarmRange *harmValueInCentre );
        //    lLifeImp.injure(lHarmRange * harmValueInCentre);
        //}

        //injuredLifeInTheFrame.Clear();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, harmRadius);
    }

}