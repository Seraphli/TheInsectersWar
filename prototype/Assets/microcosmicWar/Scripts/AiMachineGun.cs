using UnityEngine;
using System.Collections;

[System.Serializable]
public class _2dInvertDoubleFaceSprite
{
    public UnitFaceDirection face = UnitFaceDirection.left;

    //FIXME_VAR_TYPE preFace=1;

    //FIXME_VAR_TYPE leftFaceValue=1;

    //原始的朝向
    public UnitFaceDirection originalFace = UnitFaceDirection.left;

    //Transform invertObject;

    public Transform turnObject;

    public ZZSprite faceLeftSprite;

    public ZZSprite faceRightSprite;

    public ZZSprite nowSprite;

    //_2dInvertDoubleFaceSprite ()
    // {
    //preFace=face;
    //_UpdateFaceShow();
    //}

    public void init(int pFace)
    {
        face = (UnitFaceDirection)pFace;
        _UpdateFaceShow();
    }

    protected float invertObjectXscale;

    public ZZSprite setFace(int pFace)
    {
        //invertObjectXscale=invertObject.localScale.x;
        UpdateFaceShow(pFace);
        face = (UnitFaceDirection)pFace;
        return nowSprite;
    }

    public UnitFaceDirection getFace()
    {
        return face;
    }

    public ZZSprite getNowSprite()
    {
        return nowSprite;
    }

    //以右为正
    public void setFaceDirection(int pFace)
    {
        //setFace(pFace*leftFaceValue);
        setFace(pFace);
    }

    public void setAnimationListener(string pAnimationName, AnimationListener pListener)
    {
        faceLeftSprite.setListener(pAnimationName, pListener);
        faceRightSprite.setListener(pAnimationName, pListener);
    }

    public ZZSprite UpdateFaceShow(int pFace)
    {
        if (face != (UnitFaceDirection)pFace)
        {
            face = (UnitFaceDirection)pFace;
            _UpdateFaceShow();
        }

        return nowSprite;
    }

    protected void _UpdateFaceShow()
    {
        if (face == originalFace)
            turnObject.rotation = new Quaternion(0, 0, 0, 1);
        else
            turnObject.rotation = new Quaternion(0, 1, 0, 0);

        //if(face==leftFaceValue)
        if (face == UnitFaceDirection.right)
            nowSprite = faceRightSprite;
        else
            nowSprite = faceLeftSprite;
    }
}

class AiMachineGun : DefenseTower
{

    public ZZSprite gunSprite;

    public float fireTime=0.0f;


    //物体朝向
    //FIXME_VAR_TYPE face= 1;

    //protected Transform gunPivot;
    //protected float Xscale;

    public _2dInvertDoubleFaceSprite invert = new _2dInvertDoubleFaceSprite();

    //在播放射击动画时,会执行的动作
    protected AnimationImpInTimeList actionImpDuringFireAnimation = new AnimationImpInTimeList();


    public override void setAdversaryLayer(int pLayer)
    {

        if (zzCreatorUtility.isHost())
        {
            AiMachineGunAI lAi = GetComponentInChildren<AiMachineGunAI>();
            lAi.setAdversaryLayer(pLayer);
        }
    }


    public override void Start()
    {
        base.Start();
        invert.setAnimationListener("fire", actionImpDuringFireAnimation);

        actionImpDuringFireAnimation.setImpInfoList(
                new AnimationImpTimeListInfo[] { new AnimationImpTimeListInfo(fireTime, EmitBullet) }
            );

    }


    protected override void initWhenHost()
    {
        int lIntType = (int)invert.getFace();
        zzCreatorUtility.sendMessage(gameObject, "initFace", lIntType);
    }

    public override void init(Hashtable info)
    {
        //print(info["face"]);
        //print(invert);
        invert.face = (UnitFaceDirection)info["face"];
        base.init(info);
    }

    [RPC]
    public void initFace(int pFace)
    {
        invert.init(pFace);
        gunSprite = invert.getNowSprite();
    }

    public void setFaceDirection(int pFace)
    {
        //print(gunSprite);
        gunSprite = invert.setFace(pFace);
    }


    public override UnitFaceDirection getFaceDirection()
    {
        return invert.getFace();
        //return face;
    }

    public override void Update()
    {
        //print(gunSprite);
        if (inFiring())
            gunSprite.playAnimation("fire");
        else
            gunSprite.playAnimation("wait");
        base.Update();
        //impSmoothTurn(Time.deltaTime );
        //setAngle(nowAngular);
    }


}
