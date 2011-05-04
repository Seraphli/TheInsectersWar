
using UnityEngine;
using System.Collections.Generic;

public class SoldierFactory : MonoBehaviour
{
    //public struct SoldierCreatedList
    //{
    //    public SoldierCreatedList(HashSet<Soldier> pSoldierList,int pUsedPoint)
    //    {
    //        soldierList = pSoldierList;
    //        usedPoint = pUsedPoint;
    //    }

    //    public HashSet<Soldier> soldierList;
    //    public int usedPoint = 0;

    //}

    static public SoldierFactory addFactory(GameObject factoryObject,
        GameObject armyPrefab, float pProduceInterval, float pfirstTimeOffset)
    {
        SoldierFactory lSoldierFactory = factoryObject.AddComponent<SoldierFactory>();
        lSoldierFactory.soldierToProduce = armyPrefab;
        lSoldierFactory.produceInterval = pProduceInterval;
        lSoldierFactory.firstTimeOffset = pfirstTimeOffset;
        return lSoldierFactory;
    }

    public interface Listener
    {
        //Transform getProduceTransform(int index);

        Transform[] finalAims
        {
            get ;
        }

        LayerMask adversaryLayerMask
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>produceTransform</returns>
        Transform prepareProduce();

        HashSet<Soldier> popSoldierCreatedList();

        HashSet<Soldier> soldierCreatedList
        {
            get;
            set;
        }

    }
    //public string adversaryName = "";

    //public Transform finalAim;

    //protected int adversaryLayerValue;

    //public Transform produceTransform;

    public float produceInterval = 1.0f;

    public GameObject soldierToProduce;

    public int pointCount = 10;

    public int pointCountInSoldier = 1;

    public HashSet<Soldier> soldierList = new HashSet<Soldier>();

    public float firstTimeOffset = 0f;

    [SerializeField]
    protected float timePos = 0.0f;

    //Component.SendMessage ("dieCallFunction")
    //Component dieCallFunction;
    //public IobjectListener objectListener;

    public Listener listener;

    zzRandomPath randomPath;
    Transform produceTransform;

    public float prepareTime = 0.5f;

    void Start()
    {
        GetComponent<Life>().addDieCallback(OnFactoryDead);
        randomPath = GetComponent<zzRandomPath>();

        timePos = firstTimeOffset;

        //if (listener==null)
        //    listener = GetComponent<SoldierFactoryListener>().interfaceObject;

        if (listener != null && listener.soldierCreatedList != null)
        {
            var lSoldierCreatedList = listener.popSoldierCreatedList();
            soldierList = lSoldierCreatedList;
            usedPoint = 0;

            //移除死掉的
            List<Soldier> lRemoveList = new List<Soldier>();
            foreach (var lSoldier in soldierList)
            {
                if (collisionLayer.isAliveFullCheck(lSoldier))
                    usedPoint += lSoldier.pointCount;
                else
                    lRemoveList.Add(lSoldier);
            }
            foreach (var lSoldier in lRemoveList)
                soldierList.Remove(lSoldier);
        }

        if (zzCreatorUtility.isHost())
            canProduceCheck();
        else
            Destroy(this);
        
    }

    public int usedPoint = 0;

    public void changeUsedPoint(int pPointCount)
    {
        usedPoint += pPointCount;
        canProduceCheck();
    }

    public void canProduceCheck()
    {
        if(pointCount-pointCountInSoldier-usedPoint>=0)
        {
            //有剩余点数造兵
            this.enabled = true;
        }
        else
        {
            this.enabled = false;
        }
    }

    public void soldierDeadCall(Life pLife)
    {
        var lSoldier = pLife.GetComponent<Soldier>();
        soldierList.Remove(lSoldier);
        //canProduceCheck();
        changeUsedPoint(-(lSoldier.pointCount));
    }

    void Update()
    {
        //if(zzCreatorUtility.isHost())
        //{
        if (listener==null)
            return;

        timePos += Time.deltaTime;
        if (!produceTransform && timePos > produceInterval - prepareTime)
        {
            produceTransform = listener.prepareProduce();
        }
        if (timePos > produceInterval)
        {
            //FIXME_VAR_TYPE lClone= Network.Instantiate(soldierToProduce, transform.position+Vector3(0,2.5f,0), Quaternion(), 0);
            GameObject lClone = zzCreatorUtility.Instantiate(soldierToProduce,
                produceTransform.position, new Quaternion(), 0);

            //GameSceneManager.Singleton.addSoldier(lClone);

            timePos = 0.0f;
            var lSoldier = lClone.GetComponent<Soldier>();
            lSoldier.pointCount = pointCountInSoldier;
            changeUsedPoint(pointCountInSoldier);
            soldierList.Add(lSoldier);
            lSoldier.GetComponent<Life>().addDieCallback(soldierDeadCall);
            var soldierAI = lClone.GetComponent<ISoldierAI>();
            //soldierAI.AddFinalAim(finalAim);

            //foreach (CheckPointPath lCheckPointPath in checkPointPaths)
            //{
            soldierAI.setHome(transform);
            soldierAI.AddPresetAim(listener.finalAims, zzAimTranformList.AimType.aliveAim);

            if (randomPath && randomPath.totalWeigth>0)
            {
                zzRandomPath.CheckPointPath lCheckPointPath = randomPath.randomPath();

                for (int i = lCheckPointPath.CheckPointList.Length - 1; i >= 0; --i)
                {
                    Transform lNowPoint = lCheckPointPath.CheckPointList[i];
                    soldierAI.AddPresetAim(lNowPoint, zzAimTranformList.AimType.checkPoint);
                }

            }

            soldierAI.adversaryLayerMask = listener.adversaryLayerMask;
            produceTransform = null;
        }
        //}
    }

    void OnFactoryDead(Life pLife)
    {
        if (!zzCreatorUtility.isHost())
            return;

        foreach (var lSoldier in soldierList)
        {
            lSoldier.GetComponent<Life>().removeDieCallback(soldierDeadCall);
        }
        
        listener.soldierCreatedList = soldierList;
    }
}