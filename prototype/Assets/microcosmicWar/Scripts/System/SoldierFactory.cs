
using UnityEngine;
using System.Collections;

public class SoldierFactory : MonoBehaviour
{
    public interface SoldierFactoryListener
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

    }
    //public string adversaryName = "";

    //public Transform finalAim;

    //protected int adversaryLayerValue;

    //public Transform produceTransform;

    public float produceInterval = 1.0f;

    public GameObject soldierToProduce;

    public float firstTimeOffset = 0f;

    protected float timePos = 0.0f;

    //Component.SendMessage ("dieCallFunction")
    //Component dieCallFunction;
    //public IobjectListener objectListener;

    public SoldierFactoryListener listener;

    zzRandomPath randomPath;
    Transform produceTransform;

    public float prepareTime = 0.5f;

    void Start()
    {
        randomPath = gameObject.GetComponent<zzRandomPath>();

        timePos = firstTimeOffset;

        if (!zzCreatorUtility.isHost())
            Destroy(this);

        
    }

    void Update()
    {
        //if(zzCreatorUtility.isHost())
        //{
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

            GameSceneManager.Singleton.addSoldier(lClone);

            timePos = 0.0f;
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
}