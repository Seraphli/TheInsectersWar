
using UnityEngine;
using System.Collections;

public class SoldierFactory : MonoBehaviour
{
    [System.Serializable]
    public class CheckPointPath
    {
        public Transform[] CheckPointList;

        //会选择此条路径的概率 weight/权重总和
        public int weight;
    }

    public string adversaryName = "";

    public Transform finalAim;

    public CheckPointPath[] checkPointPaths;

    public float produceInterval = 1.0f;

    public GameObject soldierToProduce;

    protected float timePos = 0.0f;

    protected int adversaryLayerValue;

    public Transform produceTransform;

    //Component.SendMessage ("dieCallFunction")
    //Component dieCallFunction;
    public IobjectListener objectListener;

    CheckPointPath[] checkPointPathWeightList;


    void Start()
    {
        if (!produceTransform)
            produceTransform = transform;

        collisionLayer.addCollider(gameObject);

        adversaryLayerValue = 1 << LayerMask.NameToLayer(adversaryName);

        Life lLife = gameObject.GetComponent<Life>();
        //lLife.setDieCallback(dieCall);
        lLife.addDieCallback(dieCall);

        if (!zzCreatorUtility.isHost())
            Destroy(this);

        {
            //初始化随机路径
            int lTotalWeigth = 0;
            foreach (CheckPointPath lCheckPointPath in checkPointPaths)
            {
                //若权重为0，改为1
                if (lCheckPointPath.weight == 0)
                    lCheckPointPath.weight = 1;
                lTotalWeigth += lCheckPointPath.weight;
            }

            checkPointPathWeightList = new CheckPointPath[lTotalWeigth];
            int lIndex = 0;

            //按权重比例将路径填充进查询表checkPointPathWeightList
            foreach (CheckPointPath lCheckPointPath in checkPointPaths)
            {
                int lBeginIndex = lIndex;
                int lEndIndex = lBeginIndex + lCheckPointPath.weight;
                for (; lIndex < lEndIndex; ++lIndex)
                    checkPointPathWeightList[lIndex] = lCheckPointPath;
            }

        }
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
                produceTransform.position,new Quaternion(), 0);
            timePos = 0.0f;
            SoldierAI soldierAI = lClone.GetComponent<SoldierAI>();
            //soldierAI.AddFinalAim(finalAim);

            //foreach (CheckPointPath lCheckPointPath in checkPointPaths)
            //{
            soldierAI.AddFinalAim(finalAim, zzAimTranformList.AimType.aliveAim);

            if(checkPointPathWeightList.Length>0)
            {
                CheckPointPath lCheckPointPath = checkPointPathWeightList[Random.Range(0, checkPointPathWeightList.Length)];

                for (int i = lCheckPointPath.CheckPointList.Length - 1; i >= 0; --i)
                {
                    Transform lNowPoint = lCheckPointPath.CheckPointList[i];
                    soldierAI.AddFinalAim(lNowPoint, zzAimTranformList.AimType.checkPoint);
                }

            }


            soldierAI.SetAdversaryLayerValue(adversaryLayerValue);
            //lClone.GetComponent<SoldierAI>().SetSoldier(lClone.GetComponent<Soldier>());
            //lClone.GetComponent<SoldierAI>().SetAdversaryLayerValue(adversaryLayerValue);
        }
        //}
    }

    public void dieCall(Life p)
    {
        //if(dieCallFunction)
        //	dieCallFunction.SendMessage ("dieCallFunction");
        //else
        if (objectListener!=null)
            objectListener.removedCall();
        Destroy(gameObject);
        //GameScene.getSingleton().gameResult(adversaryName);
    }

    void OnDrawGizmosSelected()
    {

        foreach (CheckPointPath lCheckPointPath in checkPointPaths)
        {
            if(lCheckPointPath.CheckPointList.Length>0)
            {
                Transform lLastPoint = lCheckPointPath.CheckPointList[0];
                for(int i=1;i<lCheckPointPath.CheckPointList.Length;++i)
                {
                    Transform lNowPoint = lCheckPointPath.CheckPointList[i];
                    Gizmos.DrawLine(lLastPoint.position, lNowPoint.position);
                    lLastPoint = lNowPoint;
                }
                Gizmos.DrawLine(lLastPoint.position, finalAim.position);
            }
        }
    }
}