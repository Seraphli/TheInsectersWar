
using UnityEngine;
using System.Collections;

public class SoldierFactory : MonoBehaviour
{

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

    ArmyBase armyBase;

    zzRandomPath randomPath;


    void Start()
    {
        randomPath = gameObject.GetComponent<zzRandomPath>();
        armyBase = gameObject.GetComponent<ArmyBase>();

        timePos = firstTimeOffset;

        if (!zzCreatorUtility.isHost())
            Destroy(this);

        
    }

    void Update()
    {
        //if(zzCreatorUtility.isHost())
        //{
        timePos += Time.deltaTime;
        if (timePos > produceInterval)
        {
            //FIXME_VAR_TYPE lClone= Network.Instantiate(soldierToProduce, transform.position+Vector3(0,2.5f,0), Quaternion(), 0);
            GameObject lClone = zzCreatorUtility.Instantiate(soldierToProduce, 
                armyBase.produceTransform.position,new Quaternion(), 0);

            GameSceneManager.Singleton.addSoldier(lClone);

            timePos = 0.0f;
            var soldierAI = lClone.GetComponent<ISoldierAI>();
            //soldierAI.AddFinalAim(finalAim);

            //foreach (CheckPointPath lCheckPointPath in checkPointPaths)
            //{
            soldierAI.setHome(transform);
            soldierAI.AddPresetAim(armyBase.finalAim, zzAimTranformList.AimType.aliveAim);
            soldierAI.AddPresetAim(armyBase.finalAims, zzAimTranformList.AimType.aliveAim);

            if (randomPath && randomPath.totalWeigth>0)
            {
                zzRandomPath.CheckPointPath lCheckPointPath = randomPath.randomPath();

                for (int i = lCheckPointPath.CheckPointList.Length - 1; i >= 0; --i)
                {
                    Transform lNowPoint = lCheckPointPath.CheckPointList[i];
                    soldierAI.AddPresetAim(lNowPoint, zzAimTranformList.AimType.checkPoint);
                }

            }

            soldierAI.adversaryLayerMask = armyBase.adversaryLayerMask;
        }
        //}
    }
}