
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierAI : ISoldierAI
{
 
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


    protected override void AIStart()
    {
        fireDistanceMin = UnityEngine.Random.Range(fireDistanceMinRandMin, fireDistanceMinRandMax);
        fireDistanceMax = UnityEngine.Random.Range(fireDistanceMaxRandMin, fireDistanceMaxRandMax);
    }

    //void detectEnemy()
    //{
    //    if(runtimeAim.getAim(transform)==null)
    //    {
    //        Collider[] lEnemyes = enemyDetector.detect(1, adversaryLayerMask.value);
    //        if (lEnemyes.Length > 0)
    //        {
    //            runtimeAim.checkAndAddAim(lEnemyes[0].transform);
    //            //print(adversaryLayerValue);
    //            //print("detectEnemy:" + lEnemyes[0].transform.name);
    //            //if (lEnemyes[0].transform.parent)
    //            //    print("detectEnemy:" + lEnemyes[0].transform.parent.name);
    //            //print("Layer:" + lEnemyes[0].gameObject.layer);
    //            //print("LayerValue:" + (1 << lEnemyes[0].gameObject.layer));
    //            pathUpdate();
    //        }
    //    }
    //}

    ////public UnitActionCommand moveToAim(Transform pAim)
    //public UnitActionCommand moveToAim(Vector3 pAimPos)
    //{
    //    UnitActionCommand lActionCommand = new UnitActionCommand();
    //    //if (finalAim)
    //    //if (haveAim)
    //    //if(pAim)
    //    //{
    //    //    Vector3 pAimPos = pAim.position;
    //        float lT = pAimPos.x - transform.position.x;
    //        //FIXME_VAR_TYPE lActionCommand=UnitActionCommand();
    //        lActionCommand.GoForward = true;

    //        if (character.isGrounded)
    //        {
    //            //目标x值 在一定范围内时,可跳跃
    //            if (Mathf.Abs(pAimPos.x - transform.position.x) < 1.0f)
    //            {
    //                if (pAimPos.y > transform.position.y + 0.8f)
    //                {
    //                    lActionCommand.Jump = true;
    //                    //Debug.Log("jump1");
    //                }
    //                else if (pAimPos.y < transform.position.y - 0.5f)
    //                {
    //                    lActionCommand.Jump = true;
    //                    lActionCommand.FaceDown = true;
    //                }

    //            }
    //        }

    //        //目标x值 在一定范围内时,停止前进
    //        if (Mathf.Abs(pAimPos.x - transform.position.x) < 0.3)
    //            lActionCommand.GoForward = false;
    //        else if (
    //            character.isGrounded
    //            && pAimPos.y >= transform.position.y
    //            && !lActionCommand.Jump
    //            && forwardBoardDetector.detect(1, layers.standPlaceValue).Length == 0
    //            )//判断前进的方向上是否有可站立的地方,没有 就跳跃
    //        {
    //            lActionCommand.Jump = true;
    //            //Debug.Log("jump2");
    //        }
    //        /*
    //        if(lT<0)
    //        {
    //            lActionCommand.FaceLeft=true;
    //        }
    //        else
    //        {
    //            lActionCommand.FaceRight=true;
    //        }*/
    //        //soldier.setCommand(lActionCommand);
    //        setFaceCommand(lActionCommand, (int)lT);
    //        return lActionCommand;
    //    //}
    //    //return lActionCommand;
    //}

    //public void setFaceCommand(UnitActionCommand pActionCommand, int face)
    //{
    //    if (face < 0)
    //    {
    //        pActionCommand.FaceLeft = true;
    //    }
    //    else if (face > 0)
    //    {
    //        pActionCommand.FaceRight = true;
    //    }
    //}


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
        var lFireHit = forwardFireDetector.detect(1, adversaryLayerMask);
        if (lFireHit.Length > 0)
        {
            runtimeAim.checkAndAddAim(lFireHit[0].transform);
            return lFaceValue;
        }

        lFireHit = backwardFireDetector.detect(1, adversaryLayerMask);
        if (lFireHit.Length > 0)
        {
            runtimeAim.checkAndAddAim(lFireHit[0].transform);
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

    //public Vector3 aimPosition;
    //public void PathComplete(Vector3[] newPoints)
    //{
    //    wayPoints = newPoints;
    //    if (newPoints.Length > 1)
    //    {
    //        //haveAim = true;
    //        aimPosition = newPoints[1];
    //        //pAimPos = (newPoints[0] + newPoints[1]) / 2;
    //        //print(newPoints[0]);
    //        //print(pAimPos);

    //    }
    //    else
    //    {
    //        //haveAim = false;
    //        aimPosition = (newPoints[0] + getNowAimTransform().position)/2.0f;
    //        //print(newPoints[0]);

    //    }
    //    //haveAim = true;
    //    //pAimPos = newPoints[0];
    //    //print(newPoints[0]);
    //}
    ////public Vector3[] wayPoints;
    //public bool showWayPoints = false;
    //public void OnDrawGizmos()
    //{
    //    if (showWayPoints)
    //    {
    //        Gizmos.color = Color.red;

    //        for (int i = 0; i < wayPoints.Length; i++)
    //        {
    //            Gizmos.DrawSphere(wayPoints[i], 0.6f);
    //        }

    //        Gizmos.color = Color.black;
    //        Gizmos.DrawSphere(aimPosition, 1.0f);
    //    }
    //}
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

    //void pathUpdate()
    //{
    //    Transform lAim = getNowAimTransform();
    //    if (enable && lAim)
    //    {
    //        pathSeeker.StartPath(transform.position, lAim.position);
    //    }

    //}

    [SerializeField]
    bool _moveLock = false;
    zzTimer moveLockTimer;
    public void lockMove(float pTime)
    {
        if (!_moveLock)
        {
            moveLockTimer = gameObject.AddComponent<zzTimer>();
            _moveLock = true;
        }
        moveLockTimer.timePos = 0;
        moveLockTimer.setInterval(pTime);
        moveLockTimer.addImpFunction(unlockMove);
    }

    public void unlockMove()
    {
        if (moveLockTimer)
            Destroy(moveLockTimer);
        moveLockTimer = null;
        _moveLock = false;
    }

    protected override void actionCommandUpdate()
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
                actionCommand = moveToAim(aimPosition, lAim);

            if (_moveLock)
            {
                getCommand().GoForward = false;
            }

            actionCommandControl.setCommand(getCommand());
        }

    }
}