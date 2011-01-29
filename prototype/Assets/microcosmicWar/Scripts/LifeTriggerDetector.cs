using UnityEngine;
using System.Collections;

public class LifeTriggerDetector : zzDetectorBase
{
    public Hashtable enemyList = new Hashtable();

    [SerializeField]
    Transform _lockedTarget;

    [SerializeField]
    LayerMask detectLayerMask;

    public Transform lockedTarget
    {
        get { return _lockedTarget; }
    }

    public override Collider[] detect(int pMaxRequired, LayerMask pLayerMask, detectorFilterFunc pNeedDetectedFunc)
    {
        if (!collisionLayer.isAliveFullCheck(_lockedTarget))
            searchFireTargetInList();

        if (_lockedTarget)
        {
            return new Collider[] { _lockedTarget .collider};
        }
        return new Collider[0];
    }


    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & detectLayerMask.value) != 0)
        {
            //如果之前的目标作废,则使用新的
            if (!collisionLayer.isAliveFullCheck(_lockedTarget))
                _lockedTarget = other.transform;
            enemyList[other.transform] = true;
        }
    }


    void OnTriggerExit(Collider other)
    {

        enemyList.Remove(other.transform);

        //移出的是否是当前目标
        if (_lockedTarget == other.transform)
            searchFireTargetInList();
    }

    protected Transform searchFireTargetInList()
    {
        //避免一帧中多次OnTriggerExit和其他的情况,所以用此方法

        _lockedTarget = null;
        foreach (System.Collections.DictionaryEntry i in enemyList)
        {
            //判断物体是否还在场景中
            if (collisionLayer.isAliveFullCheck(i.Key as Transform))
            {
                _lockedTarget = (Transform)i.Key;
                break;
            }
            //若已被消毁,则从表中将其删去
            enemyList.Remove(i);
        }
        return _lockedTarget;
    }


}