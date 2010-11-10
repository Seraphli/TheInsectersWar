
using UnityEngine;
using System.Collections;

public class GuidedMissileLauncherAI : MonoBehaviour
{


    public zzDetectorBase detector;
    public int maxRequired = 3;

    public Transform[] targetList;
    public Emitter emitter;
    //DefenseTower defenseTower;
    public DefenseTowerAim defenseTowerAim;
    //protected 
    public int targetNum = 0;
    //protected 
    public int targetNowIndex = 0;
    //zzTimer pathTimer;
    public int adversaryLayer;

    public SphereAreaHarm sphereAreaHarm;

    public void setAdversaryLayer(int pLayer)
    {
        adversaryLayer = pLayer;
    }

    void Start()
    {
        targetList = new Transform[maxRequired];
        //pathTimer = AddComponent<zzTimer>();
        emitter.setInitBulletFunc(initBullet);
        defenseTowerAim.setAimIsActiveFunc(aimIsActive);
    }

    bool aimIsActive(Transform pTarget)
    {
        return collisionLayer.isAlive(pTarget);
    }

    ////过滤掉死掉的物体
    //bool detectorFilterFunc(Collider pCollider)
    //{
    //    return Life.getLifeIfAlive(pCollider.transform)!=null;
    //}

    public int resetAndSearchEnemy()
    {
        targetNowIndex = 0;
        //线投时,已经根据层 过滤掉了死掉的物体,所以不需设置探测过滤
        Collider[] lHits = detector.detect(maxRequired, 1 << adversaryLayer);
        targetNum = lHits.Length;
        //print("targetNum:"+targetNum);
        if (targetNum > 0)
        {
            for (int i = 0; i < targetNum; ++i)
                targetList[i] = lHits[i].transform;

            //朝第一个目标转动
            //defenseTower.takeAim(targetList[0].position,fireDeviation);
            defenseTowerAim.setTarget(targetList[0]);
        }
        return targetNum;
    }

    //得到目标列表中的下一个目标,如果到尾则返回第一个
    public int getNextTagetIndex()
    {
        int lOut = targetNowIndex + 1;
        if (lOut >= targetNum)
            return 0;
        return lOut;
    }

    public Transform getTargetAndMove()
    {
        int targetIndex = getNextTagetIndex();
        if (
            targetNum > 0 
            && collisionLayer.isAliveFullCheck( targetList[targetIndex] )
            )
        {
            targetNowIndex = targetIndex;
            return targetList[targetIndex];
        }
        if (resetAndSearchEnemy() > 0)
            return targetList[0];
        return null;
    }

    public void createSphereAreaHarm(Life pLife)
    {
        GameObject lAreaHarm = (GameObject)GameObject.Instantiate(sphereAreaHarm.gameObject, pLife.transform.position, pLife.transform.rotation);
        //print("createSphereAreaHarm:"+pLife.transform.position);
        //GameObject lAreaHarm = new GameObject("MissileSphereAreaHarm");

        //SphereAreaHarm lSphereAreaHarm = lAreaHarm.AddComponent<SphereAreaHarm>();
        SphereAreaHarm lSphereAreaHarm = lAreaHarm.GetComponent<SphereAreaHarm>();
        lSphereAreaHarm.setHarmLayerMask(1 << adversaryLayer);
    }

    public void initBullet(Bullet pBullet)
    {
        Transform lTaget = getTargetAndMove();
        if (lTaget)
        {
            BulletFollowAI lBulletFollowAI = pBullet.GetComponent<BulletFollowAI>();
            lBulletFollowAI.setTarget(lTaget);
        }

        Life lBulletLife = pBullet.gameObject.GetComponent<Life>();
        lBulletLife.addDieCallback(createSphereAreaHarm);
    }
}