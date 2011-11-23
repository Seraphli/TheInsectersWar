using UnityEngine;
using System.Collections.Generic;

public class SphereHarmByDetector:MonoBehaviour
{
    public zzDetectorBase detector;

    public float harmRadius = 5.0f;
    public float harmValueInCentre = 10.0f;

    public float minHarmRate = 0f;

    public Life.HarmType harmType = Life.HarmType.none;
    public CharacterInfoObject characterInfoObject;

    void Update()
    {
        if (enabled)
            impHarm();
        enabled = false;
        Destroy(gameObject);
    }

    void impHarm()
    {
        //现在用于存储有生命的物体,离爆炸点的最近距离
        var lInjuredLifeMinDistance = new Dictionary<Life, float>();
        Collider[] lColliderList = detector.detect();
        //搜寻范围内的Life,并寻找最短距离
        foreach (Collider i in lColliderList)
        {
            //Trigger 也会被探测到
            if (i.isTrigger)
                continue;
            Life lLife = Life.getLifeFromTransform(i.transform);
            if (lLife)
            {
                //获得物体离中心pCenterPos的最近距离
                var lCenterPos = transform.position;
                Vector3 lClosestPoint = i.ClosestPointOnBounds(lCenterPos);
                float lDistance = Vector3.Distance(lClosestPoint, lCenterPos);

                //因为一个有Life的物体上,可能有多个Collider
                if (lInjuredLifeMinDistance.ContainsKey(lLife))
                {
                    float lObjectDistance = lInjuredLifeMinDistance[lLife];
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
        System.Action<Life,int> lInjureFunc;
        var lCharacterInfoObject = characterInfoObject;
        if (lCharacterInfoObject)
            lInjureFunc = (l,x)=>l.injure(x,
                lCharacterInfoObject.characterInfo, harmType);
        else
            lInjureFunc = (l, x) => l.injure(x, harmType);

        foreach (var lifeToDistance in lInjuredLifeMinDistance)
        {
            Life lLifeImp = (Life)lifeToDistance.Key;

            float lDistanceImp = (float)lifeToDistance.Value;
            float lHarmRange = Mathf.Lerp(minHarmRate, 1f, 1.0f - Mathf.Clamp01(lDistanceImp / harmRadius));

            lInjureFunc(lLifeImp,(int)(lHarmRange * harmValueInCentre));
        }
    }
}