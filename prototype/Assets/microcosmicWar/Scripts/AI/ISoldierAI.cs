
using UnityEngine;
using System.Collections;

public abstract class ISoldierAI:MonoBehaviour
{

    public void AddPresetAim(Transform pFinalAim, zzAimTranformList.AimType pAimType)
    {
        presetAim.checkAndAddAim(new zzAimTranformList.AimInfo(pFinalAim, pAimType));
    }

    public CharacterController character;


    //寻路计时器,在这些时间后执行寻路命令
    zzCoroutineTimer pathTimer;

    //动作技术器,计算并发送动作命令
    zzTimer actionCommandTimer;

    public ActionCommandControl actionCommandControl;

    public Seeker pathSeeker;

    public bool enable = true;

    protected UnitActionCommand actionCommand = new UnitActionCommand();

    public LayerMask adversaryLayerMask = -1;


    ////射击的检测距离将在介于与以下值
    ////protected
    //public float fireDistanceMin;
    ////protected
    //public float fireDistanceMax;

    ////为了使每个兵的行为不同,射击的范围也取随机值
    //public float fireDistanceMinRandMin = 4.0f;
    //public float fireDistanceMinRandMax = 8.0f;

    //public float fireDistanceMaxRandMin = 8.0f;
    //public float fireDistanceMaxRandMax = 12.0f;

    [SerializeField]
    protected Transform home;

    public void setHome(Transform pHome)
    {
        home = pHome;
    }

    //预设的一定要消灭的目标
    public zzAimTranformList presetAim = new zzAimTranformList();

    //运动中设置的目标,可视情况忽略
    public zzAimTranformList runtimeAim = new zzAimTranformList();

    //目标是否在可追赶的范围内
    public bool needPursuit(Transform pAim)
    {
        if ((pAim.position - transform.position).sqrMagnitude < 400.0f)
            return true;
        return false;
    }

    public virtual Transform getNowAimTransform()
    {
        Transform lOut = runtimeAim.getAim(transform);
        pathTimer.setInterval(pathSearchInterval);

        //获取没有超出追赶范围的目标
        while (lOut && (!needPursuit(lOut)))
        {
            //删除超出范围的
            runtimeAim.removeNowAim();
            lOut = runtimeAim.getAim(transform);
        }

        if (!lOut)
        {
            lOut = presetAim.getAim(transform);
        }
        else if (lOut.GetComponent<Hero>() != null)
        {
            //如果遇到英雄,则减小寻路间隔
            pathTimer.setInterval(0.5f);
        }
        return lOut;
    }

    public float pathSearchInterval = 2.0f;
    public float actionCommandUpdateInterval = 0.25f;
    public float followDetectIterval = 0.5f;

    void Start()
    {

        if (!zzCreatorUtility.isHost())
        {
            Destroy(this);
            return;
        }

        //寻路的计时器
        pathTimer = gameObject.AddComponent<zzCoroutineTimer>();
        pathTimer.setInterval(pathSearchInterval);
        pathTimer.setImpFunction(this.pathUpdate);

        //行为更新的计时器
        actionCommandTimer = gameObject.AddComponent<zzTimer>();
        actionCommandTimer.setInterval(actionCommandUpdateInterval);
        actionCommandTimer.setImpFunction(this.actionCommandUpdate);

        //广范围探测追踪(敌人)的计时器
        zzCoroutineTimer lFollowDetectorTimer = gameObject.AddComponent<zzCoroutineTimer>();
        lFollowDetectorTimer.setInterval(followDetectIterval);
        lFollowDetectorTimer.setImpFunction(this.detectFollowed);

        if (!actionCommandControl)
            actionCommandControl = gameObject.GetComponentInChildren<ActionCommandControl>();

        AIStart();

        //产生第一个命令
        if (enable)
        {
            pathUpdate();
            actionCommandUpdate();
        }

        //fireDistanceMin = UnityEngine.Random.Range(fireDistanceMinRandMin, fireDistanceMinRandMax);
        //fireDistanceMax = UnityEngine.Random.Range(fireDistanceMaxRandMin, fireDistanceMaxRandMax);
    }

    protected virtual void AIStart()
    {

    }

    //enum AimType = zzAimTranformList.AimType;

    public void AddFinalAim(Transform pFinalAim, zzAimTranformList.AimType pAimType)
    {
        presetAim.checkAndAddAim(new zzAimTranformList.AimInfo(pFinalAim, pAimType));
    }

    public void SetAdversaryLayerMask(int pLayerMask)
    {
        adversaryLayerMask = pLayerMask;
    }

    //判断前进的方向上是否有可站立的地方
    public zzDetectorBase forwardBoardDetector;

    public zzDetectorBase followDetector;

    protected virtual void detectFollowed()
    {
        if (runtimeAim.getAim(transform) == null)
        {
            Collider[] lEnemyes = followDetector.detect(1, adversaryLayerMask.value);
            if (lEnemyes.Length > 0)
            {
                runtimeAim.checkAndAddAim(lEnemyes[0].transform);

                pathUpdate();
            }
        }
    }

    //public UnitActionCommand moveToAim(Transform pAim)
    public UnitActionCommand moveToAim(Vector3 pAimPos)
    {
        UnitActionCommand lActionCommand = new UnitActionCommand();

        float lT = pAimPos.x - transform.position.x;

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
    //protected virtual int needFire()
    //{
    //    //FIXME_VAR_TYPE lFwd= transform.TransformDirection(Vector3.forward);
    //    RaycastHit lHit = new RaycastHit();
    //    //print(transform.position);
    //    //print(soldier);
    //    //print(adversaryLayerValue);
    //    //FIXME_VAR_TYPE lFaceDirection= 0;
    //    int lFaceValue = actionCommandControl.getFaceValue();

    //    if (Physics.Raycast(transform.position, new Vector3(lFaceValue, 0, 0),
    //        out lHit, Random.Range(fireDistanceMin, fireDistanceMax), adversaryLayerMask.value))
    //    //if (Physics.Raycast (transform.position, lFwd , lHit, 4.0f,adversaryLayerValue)) 
    //    {
    //        //fireTarget = lHit.transform;
    //        runtimeAim.checkAndAddAim(lHit.transform);
    //        return lFaceValue;
    //    }
    //    if (Physics.Raycast(transform.position, new Vector3(-lFaceValue, 0, 0),
    //        out lHit, fireDistanceMin, adversaryLayerMask.value))
    //    {
    //        runtimeAim.checkAndAddAim(lHit.transform);
    //        return -lFaceValue;
    //    }
    //    return 0;
    //}

    public UnitActionCommand getCommand()
    {
        return actionCommand;
    }

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
            aimPosition = (newPoints[0] + getNowAimTransform().position) / 2.0f;
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

    protected void pathUpdate()
    {
        Transform lAim = getNowAimTransform();
        if (enable && lAim)
        {
            pathSeeker.StartPath(transform.position, lAim.position);
        }

    }


    protected abstract void actionCommandUpdate();
    //{
    //    Transform lAim = getNowAimTransform();
    //    if (enable && lAim)
    //    {
    //        int lFireTaget = needFire();
    //        if (lFireTaget != 0)
    //        {
    //            actionCommand.clear();
    //            actionCommand.Fire = true;
    //            setFaceCommand(actionCommand, lFireTaget);
    //        }
    //        else
    //            actionCommand = moveToAim(aimPosition);

    //        actionCommandControl.setCommand(getCommand());
    //    }

    //}

}