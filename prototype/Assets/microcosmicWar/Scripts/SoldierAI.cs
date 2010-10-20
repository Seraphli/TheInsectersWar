
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierAI : MonoBehaviour
{

    public string adversaryName;

    //执行频率/每秒执行的次数
    //FIXME_VAR_TYPE frequencyOfImplement=3.0f;
    public float frequencyOfImplement = 3.0f;
    //public zzFrequencyTimer fequencyTimer = new zzFrequencyTimer();
    zzCoroutineTimer timer;

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

    protected int adversaryLayerValue = -1;

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
        if((pAim.position - transform.position).sqrMagnitude<100.0f)
            return true;
        return false;
    }

    public Transform getNowAimTransform()
    {
        Transform lOut = runtimeAim.getAim();

        //获取没有超出追赶范围的目标
        while( lOut && (!needPursuit(lOut)) )
        {
            runtimeAim.removeNowAim();
            lOut = runtimeAim.getAim();
        }

        if(!lOut)
        {
            lOut = fixedAim.getAim();
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
        fixedAim.initAimList();
        runtimeAim.initAimList();
        //initAimList();
        //客户端的AI 在 SoldierNetView 中去除
        //if(!zzCreatorUtility.isHost())
        //	Debug.LogError("AI not removed in not host");
        //{
        //	Destroy(this);
        //	timeToWait=100.0f;
        //	return;
        //}
        //timeToWait=1.0f/frequencyOfImplement;
        //timePos=timeToWait+0.1f;
        //if (finalAim)
        //    pAimPos = finalAim.position;
        if (adversaryLayerValue == -1)
            adversaryLayerValue = 1 << LayerMask.NameToLayer(adversaryName);
        //print(LayerMask.NameToLayer(adversaryName));
        //print(adversaryLayerValue);
        if (!actionCommandControl)
            actionCommandControl = gameObject.GetComponentInChildren<ActionCommandControl>();
        //fequencyTimer.setImpFunction(AiUpdate);

        fireDistanceMin = Random.Range(fireDistanceMinRandMin, fireDistanceMinRandMax);
        fireDistanceMax = Random.Range(fireDistanceMaxRandMin, fireDistanceMaxRandMax);

        //产生第一个命令
        if (enable)
            AiUpdate();
        timer = gameObject.AddComponent<zzCoroutineTimer>();
        timer.setInterval(1.0f / frequencyOfImplement);
        timer.setImpFunction(AiUpdate);
    }

    public void AddFinalAim(Transform pFinalAim)
    {
        //finalAim = pFinalAim;
        //pAimPos = new Vector3();
        //if (pFinalAim)
        //    pAimPos = finalAim.position;
        fixedAim.checkAndAddAim(pFinalAim);
    }

    //public void SetSoldier(Soldier pSoldier)
    //{
    //    //print("SetSoldier");
    //    //print(pSoldier);
    //    soldier = pSoldier;
    //}

    public void SetAdversaryLayerValue(int pLayerValue)
    {
        adversaryLayerValue = pLayerValue;
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

            if(Mathf.Abs(pAimPos.x - transform.position.x) < 1.0f)
            {
                if (pAimPos.y > transform.position.y + 0.5f)
                    lActionCommand.Jump = true;
                else if(pAimPos.y < transform.position.y - 0.5f)
                {
                    lActionCommand.Jump = true;
                    lActionCommand.FaceDown = true;
                }

            }

            if (Mathf.Abs(pAimPos.x - transform.position.x) < 0.3)
                lActionCommand.GoForward = false;
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
            out lHit, Random.Range(fireDistanceMin, fireDistanceMax), adversaryLayerValue))
        //if (Physics.Raycast (transform.position, lFwd , lHit, 4.0f,adversaryLayerValue)) 
        {
            //fireTarget = lHit.transform;
            runtimeAim.checkAndAddAim(lHit.transform);
            return lFaceValue;
        }
        if (Physics.Raycast(transform.position, new Vector3(-lFaceValue, 0, 0),
            out lHit, fireDistanceMin, adversaryLayerValue))
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

    public void calculate()
    {
        Transform lAim = getNowAimTransform();
        if (lAim)
        {
            pathSeeker.StartPath(transform.position, lAim.position );
            int lFireTaget = needFire();
            if (lFireTaget != 0)
            {
                actionCommand.clear();
                actionCommand.Fire = true;
                setFaceCommand(actionCommand, lFireTaget);
            }
            else
                actionCommand = moveToAim(aimPosition);

        }
    }

    Vector3 aimPosition;
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
            aimPosition = newPoints[0];
            //print(newPoints[0]);

        }
        //haveAim = true;
        //pAimPos = newPoints[0];
        //print(newPoints[0]);
    }
    public Vector3[] wayPoints;
    public void OnDrawGizmos()
    {
        //if (wayPoints)
        //{
        Gizmos.color = Color.red;

        for (int i = 0; i < wayPoints.Length; i++)
        {
            Gizmos.DrawSphere(wayPoints[i], 1.0f);
        }
        //}
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

    void AiUpdate()
    {
        if (enable)
        {
            calculate();
            actionCommandControl.setCommand(getCommand());
        }
    }
}