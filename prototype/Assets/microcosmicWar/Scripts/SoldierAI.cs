
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierAI : MonoBehaviour
{
    public CharacterController character;
    //public string adversaryName;

    //执行频率/每秒执行的次数
    //FIXME_VAR_TYPE frequencyOfImplement=3.0f;
    public float frequencyOfImplement = 3.0f;
    //public zzFrequencyTimer fequencyTimer = new zzFrequencyTimer();

    //寻路计时器,在这些时间后执行寻路命令
    zzCoroutineTimer pathTimer;

    //动作技术器,计算并发送动作命令
    zzTimer actionCommandTimer;

    //protected float timeToWait;//=1.0f/frequencyOfImplement

    //protected FIXME_VAR_TYPE timePos=0.0f;

    public ActionCommandControl actionCommandControl;
    //public Transform finalAim;
    //bool haveAim = false;


    public Seeker pathSeeker;
    //int enemyLayer;
    public bool enable = true;

    //protected Vector3 pAimPos;
    protected UnitActionCommand actionCommand = new UnitActionCommand();

    public LayerMask adversaryLayerMask = -1;

    //public Transform fireTarget;

    //射击的检测距离将在介于与以下值
    //protected
    public float fireDistanceMin;
    //protected
    public float fireDistanceMax;

    //为了使每个兵的行为不同,射击的范围也取随机值
    public float fireDistanceMinRandMin = 4.0f;
    public float fireDistanceMinRandMax = 8.0f;

    public float fireDistanceMaxRandMin = 8.0f;
    public float fireDistanceMaxRandMax = 12.0f;

    //预设的一定要消灭的目标
    public zzAimTranformList fixedAim = new zzAimTranformList();

    //运动中设置的目标,可视情况忽略
    public zzAimTranformList runtimeAim = new zzAimTranformList();

    //目标是否在可追赶的范围内
    public bool needPursuit(Transform pAim)
    {
        if((pAim.position - transform.position).sqrMagnitude<400.0f)
            return true;
        return false;
    }

    public Transform getNowAimTransform()
    {
        Transform lOut = runtimeAim.getAim(transform);
        pathTimer.setInterval(2.0f);

        //获取没有超出追赶范围的目标
        while( lOut && (!needPursuit(lOut)) )
        {
            //删除超出范围的
            runtimeAim.removeNowAim();
            lOut = runtimeAim.getAim(transform);
        }

        if(!lOut)
        {
            lOut = fixedAim.getAim(transform);
        }
        else if (lOut.GetComponent<Hero>() != null)
        {
            //如果遇到英雄,则减小寻路间隔
            pathTimer.setInterval(0.5f);
        }
        return lOut;
    }
    
    //[SerializeField]
    ////现在使用的 目标列表
    //zzAimTranformList nowAimList;

    //void updateAimList()
    //{
   
    //}

    //public bool getAim(out Vector3 aimPos)
    //{
    //    aimPos = aimList[0].position;
    //    return true;
    //}

    //public Vector3 getAim()
    //{
    //    return aimList[0].position;
    //}

    void Start()
    {
        //客户端的AI 在 SoldierNetView 中去除
        if(!zzCreatorUtility.isHost())
        {
        	Destroy(this);
        	return;
        }

        //寻路的计时器
        pathTimer = gameObject.AddComponent<zzCoroutineTimer>();
        pathTimer.setInterval(2.0f);
        pathTimer.setImpFunction(pathUpdate);

        //行为更新的计时器
        actionCommandTimer = gameObject.AddComponent<zzTimer>();
        actionCommandTimer.setInterval(1.0f / frequencyOfImplement);
        actionCommandTimer.setImpFunction(actionCommandUpdate);

        //广范围探测敌人的计时器
        zzCoroutineTimer lEnemyDetectorTimer = gameObject.AddComponent<zzCoroutineTimer>();
        lEnemyDetectorTimer.setInterval(0.5f);
        lEnemyDetectorTimer.setImpFunction(detectEnemy);

        //fixedAim.initAimList();
        //runtimeAim.initAimList();
        //initAimList();

        //timeToWait=1.0f/frequencyOfImplement;
        //timePos=timeToWait+0.1f;
        //if (finalAim)
        //    pAimPos = finalAim.position;

        //if (adversaryLayerMask == -1)
        //    adversaryLayerMask = 1 << LayerMask.NameToLayer(adversaryName);

        //print(LayerMask.NameToLayer(adversaryName));
        //print(adversaryLayerValue);
        if (!actionCommandControl)
            actionCommandControl = gameObject.GetComponentInChildren<ActionCommandControl>();
        //fequencyTimer.setImpFunction(AiUpdate);

        fireDistanceMin = UnityEngine.Random.Range(fireDistanceMinRandMin, fireDistanceMinRandMax);
        fireDistanceMax = UnityEngine.Random.Range(fireDistanceMaxRandMin, fireDistanceMaxRandMax);

        //产生第一个命令
        if (enable)
        {
            //AiUpdate();
            pathUpdate();
            actionCommandUpdate();
        }
    }

    //enum AimType = zzAimTranformList.AimType;

    public void AddFinalAim(Transform pFinalAim, zzAimTranformList.AimType pAimType)
    {
        //finalAim = pFinalAim;
        //pAimPos = new Vector3();
        //if (pFinalAim)
        //    pAimPos = finalAim.position;
        //fixedAim.checkAndAddAim(pFinalAim, pAimType);
        fixedAim.checkAndAddAim(new zzAimTranformList.AimInfo(pFinalAim, pAimType));
    }

    //public void SetSoldier(Soldier pSoldier)
    //{
    //    //print("SetSoldier");
    //    //print(pSoldier);
    //    soldier = pSoldier;
    //}

    public void SetAdversaryLayerMask(int pLayerMask)
    {
        adversaryLayerMask = pLayerMask;
    }

    //判断前进的方向上是否有可站立的地方
    public zzDetectorBase forwardBoardDetector;

    public zzDetectorBase enemyDetector;

    void detectEnemy()
    {
        if(runtimeAim.getAim(transform)==null)
        {
            Collider[] lEnemyes = enemyDetector.detect(1, adversaryLayerMask.value);
            if (lEnemyes.Length > 0)
            {
                runtimeAim.checkAndAddAim(lEnemyes[0].transform);
                //print(adversaryLayerValue);
                //print("detectEnemy:" + lEnemyes[0].transform.name);
                //if (lEnemyes[0].transform.parent)
                //    print("detectEnemy:" + lEnemyes[0].transform.parent.name);
                //print("Layer:" + lEnemyes[0].gameObject.layer);
                //print("LayerValue:" + (1 << lEnemyes[0].gameObject.layer));
                pathUpdate();
            }
        }
    }

    //public UnitActionCommand moveToAim(Transform pAim)
    public UnitActionCommand moveToAim(Vector3 pAimPos)
    {
        UnitActionCommand lActionCommand = new UnitActionCommand();
        //if (finalAim)
        //if (haveAim)
        //if(pAim)
        //{
        //    Vector3 pAimPos = pAim.position;
            float lT = pAimPos.x - transform.position.x;
            //FIXME_VAR_TYPE lActionCommand=UnitActionCommand();
            lActionCommand.GoForward = true;

            if (character.isGrounded)
            {
                //目标x值 在一定范围内时,可跳跃
                if (Mathf.Abs(pAimPos.x - transform.position.x) < 1.0f)
                {
                    if (pAimPos.y > transform.position.y + 0.8f)
                    {
                        lActionCommand.Jump = true;
                        //Debug.Log("jump1");
                    }
                    else if (pAimPos.y < transform.position.y - 0.5f)
                    {
                        lActionCommand.Jump = true;
                        lActionCommand.FaceDown = true;
                    }

                }
            }

            //目标x值 在一定范围内时,停止前进
            if (Mathf.Abs(pAimPos.x - transform.position.x) < 0.3)
                lActionCommand.GoForward = false;
            else if (
                character.isGrounded
                && pAimPos.y >= transform.position.y
                && !lActionCommand.Jump
                && forwardBoardDetector.detect(1, layers.standPlaceValue).Length == 0
                )//判断前进的方向上是否有可站立的地方,没有 就跳跃
            {
                lActionCommand.Jump = true;
                //Debug.Log("jump2");
            }
            /*
            if(lT<0)
            {
                lActionCommand.FaceLeft=true;
            }
            else
            {
                lActionCommand.FaceRight=true;
            }*/
            //soldier.setCommand(lActionCommand);
            setFaceCommand(lActionCommand, (int)lT);
            return lActionCommand;
        //}
        //return lActionCommand;
    }

    public void setFaceCommand(UnitActionCommand pActionCommand, int face)
    {
        if (face < 0)
        {
            pActionCommand.FaceLeft = true;
        }
        else if (face > 0)
        {
            pActionCommand.FaceRight = true;
        }
    }

    //判断是否需要射击,并返回要射击的x轴上的朝向.不需射击则返回0
    public int needFire()
    {
        //FIXME_VAR_TYPE lFwd= transform.TransformDirection(Vector3.forward);
        RaycastHit lHit = new RaycastHit();
        //print(transform.position);
        //print(soldier);
        //print(adversaryLayerValue);
        //FIXME_VAR_TYPE lFaceDirection= 0;
        int lFaceValue = actionCommandControl.getFaceValue();

        if (Physics.Raycast(transform.position, new Vector3(lFaceValue, 0, 0),
            out lHit, Random.Range(fireDistanceMin, fireDistanceMax), adversaryLayerMask.value))
        //if (Physics.Raycast (transform.position, lFwd , lHit, 4.0f,adversaryLayerValue)) 
        {
            //fireTarget = lHit.transform;
            runtimeAim.checkAndAddAim(lHit.transform);
            return lFaceValue;
        }
        if (Physics.Raycast(transform.position, new Vector3(-lFaceValue, 0, 0),
            out lHit, fireDistanceMin, adversaryLayerMask.value))
        {
            runtimeAim.checkAndAddAim(lHit.transform);
            return -lFaceValue;
        }
        return 0;
    }

    public UnitActionCommand getCommand()
    {
        return actionCommand;
    }

    //public void calculate()
    //{
    //    Transform lAim = getNowAimTransform();
    //    if (lAim)
    //    {
    //        pathSeeker.StartPath(transform.position, lAim.position );
    //        int lFireTaget = needFire();
    //        if (lFireTaget != 0)
    //        {
    //            actionCommand.clear();
    //            actionCommand.Fire = true;
    //            setFaceCommand(actionCommand, lFireTaget);
    //        }
    //        else
    //            actionCommand = moveToAim(aimPosition);

    //    }
    //}

    public Vector3 aimPosition;
    public void PathComplete(Vector3[] newPoints)
    {
        wayPoints = newPoints;
        if (newPoints.Length > 1)
        {
            //haveAim = true;
            aimPosition = newPoints[1];
            //pAimPos = (newPoints[0] + newPoints[1]) / 2;
            //print(newPoints[0]);
            //print(pAimPos);

        }
        else
        {
            //haveAim = false;
            aimPosition = (newPoints[0] + getNowAimTransform().position)/2.0f;
            //print(newPoints[0]);

        }
        //haveAim = true;
        //pAimPos = newPoints[0];
        //print(newPoints[0]);
    }
    public Vector3[] wayPoints;
    public bool showWayPoints = false;
    public void OnDrawGizmos()
    {
        if (showWayPoints)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < wayPoints.Length; i++)
            {
                Gizmos.DrawSphere(wayPoints[i], 0.6f);
            }

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(aimPosition, 1.0f);
        }
    }
    //void Update()
    //{
    //        //print("AI Update");
    //        //if(zzCreatorUtility.isHost())
    //        //{
    //        /*timePos+=Time.deltaTime;
    //        //FIXME_VAR_TYPE lActionCommand=UnitActionCommand();
    //        if(timePos>timeToWait)
    //        {
    //            //lActionCommand=moveToFinalAim();
    //            calculate();
    //            timePos=0.0f;
    //        }
    //        //soldier.setCommand(lActionCommand);
    //        soldier.setCommand(getCommand());*/
    //        //}
    //        //fequencyTimer.Update();

    //}

    //void AiUpdate()
    //{
    //    if (enable)
    //    {
    //        calculate();
    //        actionCommandControl.setCommand(getCommand());
    //    }
    //}

    void pathUpdate()
    {
        Transform lAim = getNowAimTransform();
        if (enable && lAim)
        {
            pathSeeker.StartPath(transform.position, lAim.position);
        }

    }

    void actionCommandUpdate()
    {
        Transform lAim = getNowAimTransform();
        if (enable && lAim)
        {
            int lFireTaget = needFire();
            if (lFireTaget != 0)
            {
                actionCommand.clear();
                actionCommand.Fire = true;
                setFaceCommand(actionCommand, lFireTaget);
            }
            else
                actionCommand = moveToAim(aimPosition);

            actionCommandControl.setCommand(getCommand());
        }

    }
}