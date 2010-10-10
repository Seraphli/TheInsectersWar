
using UnityEngine;
using System.Collections;

public class SoldierAI : MonoBehaviour
{

    public string adversaryName;

    //执行频率/每秒执行的次数
    //FIXME_VAR_TYPE frequencyOfImplement=3.0f;
    public zzFrequencyTimer fequencyTimer = new zzFrequencyTimer();

    //protected float timeToWait;//=1.0f/frequencyOfImplement

    //protected FIXME_VAR_TYPE timePos=0.0f;

    public ActionCommandControl actionCommandControl;
    public Transform finalAim;
    //int enemyLayer;
    public bool enable = true;

    protected Vector3 finalAimPos;
    protected UnitActionCommand actionCommand = new UnitActionCommand();

    protected int adversaryLayerValue = -1;

    public Transform fireTarget;

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

    void Start()
    {
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
        if (finalAim)
            finalAimPos = finalAim.position;
        if (adversaryLayerValue == -1)
            adversaryLayerValue = 1 << LayerMask.NameToLayer(adversaryName);
        //print(LayerMask.NameToLayer(adversaryName));
        //print(adversaryLayerValue);
        if (!actionCommandControl)
            actionCommandControl = gameObject.GetComponentInChildren<ActionCommandControl>();
        fequencyTimer.setImpFunction(AiUpdate);

        fireDistanceMin = Random.Range(fireDistanceMinRandMin, fireDistanceMinRandMax);
        fireDistanceMax = Random.Range(fireDistanceMaxRandMin, fireDistanceMaxRandMax);

        //产生第一个命令
        if (enable)
            AiUpdate();
    }

    public void SetFinalAim(Transform pFinalAim)
    {
        finalAim = pFinalAim;
        finalAimPos = new Vector3();
        if (pFinalAim)
            finalAimPos = finalAim.position;
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

    public UnitActionCommand moveToFinalAim()
    {
        UnitActionCommand lActionCommand = new UnitActionCommand();
        if (finalAim)
        {
            float lT = finalAimPos.x - transform.position.x;
            //FIXME_VAR_TYPE lActionCommand=UnitActionCommand();
            lActionCommand.GoForward = true;
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
        }
        return lActionCommand;
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

    public int needFire()
    {
        //FIXME_VAR_TYPE lFwd= transform.TransformDirection(Vector3.forward);
        RaycastHit lHit = new RaycastHit();
        //print(transform.position);
        //print(soldier);
        //print(adversaryLayerValue);
        //FIXME_VAR_TYPE lFaceDirection= 0;
        int lFaceValue = actionCommandControl.getFaceValue();
        if (Physics.Raycast(transform.position,new Vector3(lFaceValue, 0, 0), 
            out lHit, Random.Range(fireDistanceMin, fireDistanceMax), adversaryLayerValue))
        //if (Physics.Raycast (transform.position, lFwd , lHit, 4.0f,adversaryLayerValue)) 
        {
            fireTarget = lHit.transform;
            return lFaceValue;
        }
        if (Physics.Raycast(transform.position,new Vector3(-lFaceValue, 0, 0),
            out lHit, fireDistanceMin, adversaryLayerValue))
        {
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
        int lFireTaget = needFire();
        if (lFireTaget != 0)
        {
            actionCommand.clear();
            actionCommand.Fire = true;
            setFaceCommand(actionCommand, lFireTaget);
        }
        else
            actionCommand = moveToFinalAim();
    }

    void Update()
    {
        if (enable)
        {
            //print("AI Update");
            //if(zzCreatorUtility.isHost())
            //{
            /*timePos+=Time.deltaTime;
            //FIXME_VAR_TYPE lActionCommand=UnitActionCommand();
            if(timePos>timeToWait)
            {
                //lActionCommand=moveToFinalAim();
                calculate();
                timePos=0.0f;
            }
            //soldier.setCommand(lActionCommand);
            soldier.setCommand(getCommand());*/
            //}
            fequencyTimer.Update();
        }
    }

    void AiUpdate()
    {
        calculate();
        actionCommandControl.setCommand(getCommand());
    }
}