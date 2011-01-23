
using UnityEngine;
using System.Collections;

public class AiMachineGunAI : MonoBehaviour
{


    //执行频率/每秒执行的次数

    public zzFrequencyTimer fequencyTimer = new zzFrequencyTimer();

    public Hashtable enemyList = new Hashtable();

    public Transform fireTarget;

    public DefenseTower aiMachineGun;

    public string adversaryName;
    public LayerMask adversaryLayerMask = -1;

    //敌人还在这个角度时,枪不动
    public float fireDeviation = 4.0f;

    //FIXME_VAR_TYPE adversaryNumInFireRange= 0;

    public void setAdversaryLayerMask(LayerMask pLayerMask)
    {
        adversaryLayerMask = pLayerMask;
    }

    void OnTriggerEnter(Collider other)
    {
        //print("OnTriggerEnter"+other.gameObject.layer);
        if (((1<<other.gameObject.layer) & adversaryLayerMask.value ) !=0 )
        {
            if (!collisionLayer.isAliveFullCheck(fireTarget))
                fireTarget = other.transform;
            enemyList[other.transform] = true;
        }
    }


    void OnTriggerExit(Collider other)
    {
        //print("OnTriggerExit"+other.gameObject.layer);
        //if(other.gameObject.layer==adversaryLayer)
        //{
        /*
            print("OnTriggerExit");
            if(fireTarget==null)
            {
                print(transform.parent.parent.gameObject.name);
                print(fireTarget==null);
                print(enemyList.Count);
            }
        */

        enemyList.Remove(other.transform);

        //移出的是否是目标兵
        if (fireTarget == other.transform)
            searchFireTargetInList();
        //}
    }

    protected Transform searchFireTargetInList()
    {
        //避免一帧中多次OnTriggerExit和其他的情况,所以用此方法

        //在此情况下重新搜索
        //if(!fireTarget || !enemyList.ContainsKey(fireTarget))
        //{
        fireTarget = null;
        foreach (System.Collections.DictionaryEntry i in enemyList)
        {
            //判断物体是否还在场景中
            if (collisionLayer.isAliveFullCheck(i.Key as Transform))
            {
                fireTarget = (Transform)i.Key;
                break;
            }
            //若已被消毁,则从表中将其删去
            enemyList.Remove(i);
        }

        /*
            if(fireTarget==null)
            {
                print(transform.parent.parent.gameObject.name);
                print(fireTarget==null);
                print(enemyList.Count);
            }
            */
        //}
        return fireTarget;
    }

    //function SetAdversaryLayerValue( int pLayerValue  )
    //{
    //	adversaryLayerValue = pLayerValue;
    //}

    void Start()
    {
        if (!zzCreatorUtility.isHost())
        {
            Destroy(this);
            return;
        }

        if (!aiMachineGun)
            aiMachineGun = transform.parent.GetComponentInChildren<DefenseTower>();
        if (adversaryLayerMask == -1)
            adversaryLayerMask = LayerMask.NameToLayer(adversaryName);
        fequencyTimer.setImpFunction(ImpUpdate);
    }

    void ImpUpdate()
    {
        //searchFireTarget();
        if (fireTarget)
        //if(searchFireTargetInList())
        {
            aiMachineGun.setFire(true);
            aiMachineGun.takeAim(fireTarget.position, fireDeviation);
        }
        else
        {
            aiMachineGun.setFire(false);
            searchFireTargetInList();//有时物体被移除时,没有OnTriggerExit
        }
        //adversaryNumInFireRange = enemyList.Count;
    }

    void Update()
    {
        fequencyTimer.Update();
    }

    //function searchFireTarget()
    //{
    //}
}