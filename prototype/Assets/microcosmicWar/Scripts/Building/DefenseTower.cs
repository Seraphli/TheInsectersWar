﻿using UnityEngine;
using System.Collections;

public class DefenseTower : MonoBehaviour
{
    protected Race race;

    //是否被限制转角,否则可以任意旋转,为true时, maxUpAngle , maxDownAngle 才有用
    public bool limitedAngle = true;

    public float maxUpAngle = 50.0f;

    public float maxDownAngle = 50.0f;

    public float angularVelocity = 20.0f;

    //会在update中同步到物体,以原始角度为0度
    public float nowAngle = 0.0f;

    //剩余要移动的角度,用于平滑的移动
    //float wantToTurn;


    //const float NULL_aimAngular = 1000.0f;
    ////设置一个不会用到的值,作为不是用时的值
    public float aimAngle = 0f;

    public Transform turnObject;

    public Transform gunPivot;

    public Emitter emitter;

    public bool _fire = false;

    public bool netSendFireSwitchAction = false;

    public bool fire
    {
        get { return _fire; }
        set
        {
            if(_fire!=value)
            {
                fireSwitchAction(value);
                if (netSendFireSwitchAction
                    &&Network.peerType != NetworkPeerType.Disconnected)
                    networkView.RPC("fireSwitchAction", RPCMode.Others, value);
            }
        }
    }

    [RPC]
    void fireSwitchAction(bool pFire)
    {
        _fire = pFire;
        if (_fire)
            fireOnEventReceiver();
        else
            fireOffEventReceiver();
    }

    System.Action fireOnEventReceiver;
    public void addFireOnEventReceiver(System.Action pReceiver)
    {
        fireOnEventReceiver += pReceiver;
    }
    System.Action fireOffEventReceiver;
    public void addFireOffEventReceiver(System.Action pReceiver)
    {
        fireOffEventReceiver += pReceiver;
    }

    public bool initDefenseTower = true;

    //public void setFire(bool pNeedFire)
    //{
    //    fire = pNeedFire;
    //}

    //public bool inFiring()
    //{
    //    return fire;
    //}

    public virtual void Start()
    {
        if (!limitedAngle)
        {
            maxUpAngle = 360;
            maxDownAngle = 360;
        }

        if (!gunPivot)
            gunPivot = transform.Find("turn/gunPivot");

        maxDownAngle = -maxDownAngle;


        if (initDefenseTower&&zzCreatorUtility.isHost())
        {
            zzCreatorUtility.sendMessage(gameObject, "initLayer", gameObject.layer );
            //int lIntType = invert.getFace();
            //zzCreatorUtility.sendMessage(gameObject,"initFace", lIntType);
            initWhenHost();
        }
    }

    protected virtual void initWhenHost()
    {
    }

    /*
    info["face"]
    info["layer"]
    info["adversaryLayerValue"]
    */
    public virtual void init(Hashtable info)
    {
        //print(info["face"]);
        //print(invert);
        //invert.face = info["face"];
        //gameObject.layer = (int)info["layer"];
        race = (Race)info["race"];

        int lLayer = PlayerInfo.getBuildingLayer(race);
        gameObject.layer = lLayer;
        var lShapes =  transform.FindChild("shape");
        lShapes.gameObject.layer = lLayer;
        foreach (Transform lShape in lShapes)
        {
            lShape.gameObject.layer = lLayer;
        }
        //AiMachineGunAI lAi = GetComponentInChildren<AiMachineGunAI>();

        //if(zzCreatorUtility.isHost())
        //	lAi.adversaryLayer=info["adversaryLayerValue"];

        //setAdversaryLayerMask((int)info["adversaryLayerValue"]);
        setAdversaryLayerMask(PlayerInfo.getAdversaryObjectValue(race));
    }

    public virtual void setAdversaryLayerMask(LayerMask pLayer)
    {
    }

    [RPC]
    public void initLayer(int pLayer)
    {
        //探测器 扳机 的设置在UI脚本中

        gameObject.layer = pLayer;
        var lShape = transform.Find("shape");
        if(lShape)
            foreach (Transform i in lShape)
            {
                i.gameObject.layer = pLayer;
            }
        //collisionLayer.addCollider(gameObject);
        emitter.setBulletLayer(getBulletLayer());
    }

    public virtual int getBulletLayer()
    {
        //print( LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" ));
        //子弹所在层名字为:种族名字+Bullet
        return PlayerInfo.getBulletLayer(gameObject.layer);
    }

    public virtual void Update()
    {

        //执行平滑旋转,计算 nowAngular
        impSmoothTurn(Time.deltaTime);
        setAngle(nowAngle);
    }

    public void EmitBullet()
    {
        //print("EmitBullet");
        emitter.EmitBullet();
    }

    //以角度转动
    //function smoothTurn()
    //{
    //}

    protected void impSmoothTurn(float pElapseTime)
    {
        if (aimAngle != nowAngle)
        {
            float lRemainAngular = aimAngle - nowAngle;
            //print("b:"+lRemainAngular);
            //转过一周后的处理办法,虽然和目标角度离得很近,但是值却差很多
            if (Mathf.Abs(lRemainAngular) > 180)
                lRemainAngular = lRemainAngular - zzUtilities.normalize(lRemainAngular) * 360;
            //print("a:"+lRemainAngular);
            float lRemainAngularAbs = Mathf.Abs(lRemainAngular);
            float lTurnAngular = angularVelocity * pElapseTime;
            //print("lTurnAngular:"+lTurnAngular+"lRemainAngularAbs:"+lRemainAngularAbs);
            if (lTurnAngular < lRemainAngularAbs)
                nowAngle += lTurnAngular * (lRemainAngular / lRemainAngularAbs);
            else
            {
                //到达目标位置,并停止转动
                //nowAngle += lRemainAngular;
                nowAngle = aimAngle;
                //print("final:"+nowAngular);
                //aimAngle = NULL_aimAngular;
            }

            nowAngle = nowAngle % 360;
        }
    }

    public float _getSmoothAngle()
    {
        return aimAngle;
    }

    public void _setSmoothAngle(float pAimAngular)
    {
        aimAngle = pAimAngular;
    }

    //以转速转到此角度
    public void smoothTurnToAngle(float pAimAngular)
    {
        //print("smoothTurnToAngle"+pAimAngular);
        //if (pAimAngular == NULL_aimAngular)
        //    return;
        pAimAngular = pAimAngular % 360.0f;
        //print("pAimAngular%360.0f:"+pAimAngular);

        if (pAimAngular > maxUpAngle)
        {
            aimAngle = maxUpAngle;
        }
        else if (pAimAngular < maxDownAngle)
        {
            aimAngle = maxDownAngle;
        }
        else
            aimAngle = pAimAngular;
        //print("aimAngular:"+aimAngular);
    }

    //瞄准到指定位置,如果已在枪口的deviation角度范围内,则不动
    public void takeAim(Vector3 pAimPos, float deviation)
    {
        //print(pAimPos);
        Ray lFireRay = emitter.getFireRay();
        Vector3 lEmitterToAim = pAimPos - lFireRay.origin;
        lEmitterToAim.Normalize();

        float lAngle = Vector3.Angle(lFireRay.direction, lEmitterToAim);

        if (lAngle > deviation)
        {
            Vector3 lCross = Vector3.Cross(lFireRay.direction, lEmitterToAim);

            //如果有xz面的转向物体,则叉乘结果转向,否则 lCross.z 不管用
            if (turnObject)
                lCross = turnObject.rotation * lCross;
            //FIXME_VAR_TYPE lToRight= Quaternion();
            //lToRight.SetFromToRotation(lFireRay.direction,Vector3.right);
            //lToRight.SetFromToRotation(Vector3.right,lFireRay.direction);
            //print("lCrossB:"+lCross);
            //lCross = lToRight * lCross;
            //print("lToRight:"+lToRight);
            //print("lCrossA:"+lCross);
            /*
                print("pAimPos:"+pAimPos);
                print("lFireRay:"+lFireRay);
                print("lEmitterToAim:"+lEmitterToAim);
                print("lAngle:"+lAngle);
                print("deviation:"+deviation);
            */
            //if(lEmitterToAim.y>lFireRay.direction.y)

            //因为射击口射线不一定穿过转轴,所以采取 nowAngular+/-lAngle的方式 设置角度
            if (lCross.z > 0)
            {
                //print("lEmitterToAim.y>lFireRay.direction.y");
                smoothTurnToAngle(nowAngle + lAngle);
            }
            else
            {
                //print("lEmitterToAim.y<=lFireRay.direction.y");
                smoothTurnToAngle(nowAngle - lAngle);
            }
        }
        //else
        //    smoothTurnToAngle(NULL_aimAngular);
    }


    public void setAngle(float pAngle)
    {
        gunPivot.localEulerAngles = new Vector3(0, 0, pAngle);
    }



    public virtual UnitFaceDirection getFaceDirection()
    {
        return UnitFaceDirection.left;
    }


    //角度向上转为正
    public float getAngle()
    {
        return nowAngle;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        //stream.Serialize(ref nowAngular);
        stream.Serialize(ref aimAngle);
    }

}