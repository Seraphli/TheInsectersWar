
using UnityEngine;
using System.Collections;

class MedicAI:ISoldierAI
{
    //public zzDetectorBase treatedDetector;
    public zzDetectorBase treatDirectionDetector;
    public zzDetectorBase needTreatDetector;
    //public zzDetectorBase frontEnemyDetector;
    //public zzDetectorBase backEnemyDetector;
    public bool needRunAway = false;
    public Life treatAimLife;

    int treatedLayerValue;

    protected override void AIStart()
    {
        treatedLayerValue = 1 << gameObject.layer;
    }

    bool haveTreatAim()
    {
        return treatAimLife && treatAimLife.isAlive() && (!treatAimLife.isFull());
    }

    protected override void detectFollowed()
    {
        if (!haveTreatAim())
        {
            Collider[] lColliders = followDetector.detect(1, treatedLayerValue, detectorFilter);
            if (lColliders.Length > 0)
            {
                treatAimLife = Life.getLifeFromTransform( lColliders[0].transform );
                pathUpdate();
            }
        }
    }

    bool detectorFilter(Collider pCollider)
    {
        return !pCollider.GetComponent<Life>().isFull();
    }

    public override Transform getNowAimTransform()
    {
        if (needRunAway)
            return home;
        if (haveTreatAim())
            return treatAimLife.transform;
        return base.getNowAimTransform();
    }

    //int treatDirectionDetect(int pTreatedLayerValue)
    //{
    //    Collider[] lColliders = treatDirectionDetector.detect(1, pTreatedLayerValue, detectorFilter);
    //    if(lColliders.Length==0)
    //    {
    //        return 0;
    //    }

    //    var lAimX = lColliders[0].transform.position.x;
    //    if (lAimX < transform.position.x)
    //        return -1;

    //    return 1;

    //}

    Transform treatDetect(int pTreatedLayerValue)
    {
        Collider[] lColliders = treatDirectionDetector.detect(1, pTreatedLayerValue, detectorFilter);
        if (lColliders.Length == 0)
        {
            return null;
        }
        return lColliders[0].transform;

    }

    /// <summary>
    /// 判断前方第一个兵,是不是敌人
    /// </summary>
    /// <param name="pCompanionLayerValue"></param>
    /// <param name="pDetector"></param>
    /// <returns></returns>
    bool isFaceEnemy(int pCompanionLayerValue,zzDetectorBase pDetector)
    {
        int lDetectLayerValue = pCompanionLayerValue | adversaryLayerMask.value;
        var ldetected = pDetector.detect(1, lDetectLayerValue);

        return ldetected.Length != 0 && isEnemy(ldetected[0]);
    }

    /// <summary>
    /// 判断前方第一个兵,是不是敌人
    /// </summary>
    /// <param name="pCompanionLayerValue"></param>
    /// <returns></returns>
    bool isFaceEnemy(int pCompanionLayerValue)
    {
        return isFaceEnemy(pCompanionLayerValue, forwardFireDetector)
            || isFaceEnemy(pCompanionLayerValue, backwardFireDetector);
    }

    bool isEnemy(Collider pCollider)
    {
        //print(pCollider.name);
        //print(pCollider.gameObject.layer);
        return (( 1<<(pCollider.gameObject.layer) ) &  adversaryLayerMask.value)!=0;
    }

    bool needTreat(int pCompanionLayerValue)
    {
        var lColliders = needTreatDetector.detect(1, pCompanionLayerValue, detectorFilter);
        if (lColliders.Length != 0)
        {
            treatAimLife = Life.getLifeFromTransform(lColliders[0].transform);
            return true;
        }
        return false;
    }

    protected override void actionCommandUpdate()
    {
        Transform lAim = getNowAimTransform();
        if (enable && lAim)
        {
            bool lPreNeedRunAway = needRunAway;
            needRunAway = false;
            actionCommand.clear();
            //bool lMoveToAim = false;
            if (isFaceEnemy(treatedLayerValue))
            {
                //print("isFaceEnemy(treatedLayerValue");
                if (!lPreNeedRunAway)
                {
                    pathUpdate();
                }
                needRunAway = true;
                actionCommand = moveToAim(aimPosition, lAim);
            }
            else if (needTreat(treatedLayerValue))
            {
                //print("needTreat(treatedLayerValue)");
                actionCommand.Fire = true;
            }
            else
            {
                //探测需救助者
                var lTreat = treatDetect(treatedLayerValue);

                if (lTreat)
                    actionCommand = moveToAim(aimPosition, lTreat);

                actionCommand = moveToAim(aimPosition, lAim);
            }

            actionCommandControl.setCommand(getCommand());
        }
    }

}