
using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour
{

    public zzCharacter character = new zzCharacter();

    //UnitActionCommand actionCommand;

    public bool clearCommandEveryFrame = true;

    public Emitter emitter;

    //射击在动画的动画,要按从小到大排
    public float[] fireTimeList;

    //被打死后小时的时间
    public float deadDisappearTimePos = 4.0f;

    //AudioSource fireSound;

    //在播放射击动画时,会执行的动作
    protected AnimationImpInTimeList actionImpDuringFireAnimation = new AnimationImpInTimeList();
    //在播放死亡动画时,会执行的动作
    protected AnimationImpInTimeList actionImpDuringDeadAnimation = new AnimationImpInTimeList();

    protected Transform turnObjectTransform;
    protected Transform reverseObjectTransform;

    protected float Xscale;
    protected ZZSprite mZZSprite;
    //protected CharacterController characterController;

    //private FIXME_VAR_TYPE moveV= Vector3.zero;
    //private bool grounded = false;

    private Life life;

    public ActionCommandControl actionCommandControl;

    //角色的朝向
    //protected FIXME_VAR_TYPE face= -1;
    /*
    void  getVelocity (){
        return moveV;
    }

    void  setVelocity ( Vector3 pVelocity  ){
        moveV=pVelocity;
    }
    */

    public ActionCommandControl getActionCommandControl()
    {
        return actionCommandControl;
    }

    public zzCharacter getCharacter()
    {
        return character;
    }

    public int getFaceDirection()
    {
        return actionCommandControl.getFaceValue();
    }

    void Start()
    {
        //控制权
        //userControl=userControl&&zzCreatorUtility.isMine(networkView);
        //userControl=userControl&&zzCreatorUtility.isHost();

        mZZSprite = GetComponentInChildren<ZZSprite>();
        //characterController = GetComponentInChildren<CharacterController>();
        character.characterController = GetComponentInChildren<CharacterController>();

        emitter = GetComponentInChildren<Emitter>();
        life = GetComponentInChildren<Life>();

        if (!actionCommandControl)
            actionCommandControl = GetComponentInChildren<ActionCommandControl>();
        //life.setDieCallback(deadAction);
        life.addDieCallback(deadAction);

        //?
        //characterController .detectCollisions=false;

        collisionLayer.addCollider(gameObject);

        Xscale = transform.localScale.x;

        turnObjectTransform = transform.Find("turn").transform;
        reverseObjectTransform = transform.Find("reverse").transform;

        //actionImpDuringFireAnimation.ImpFunction=EmitBullet;


        //当为host 时,允许发射子弹
        //emitter.EmitBullet 也有判断,可去除一处
        //if( zzCreatorUtility.isHost())
        //{//为了客户端有声效,所以都允许发射
        //print("userControl || zzCreatorUtility.isHost()");
        //for(AnimationImpTimeListInfo e in actionImpDuringFireAnimation.getImpInfoList())
        //{
        //	e.ImpFunction=EmitBullet;
        //}
        //FIXME_VAR_TYPE infos=Array();
        foreach (float iTime in fireTimeList)
        {
            //FIXME_VAR_TYPE lEmitBulletImp=AnimationImpTimeListInfo();
            //lEmitBulletImp.ImpTime=iTime;
            //lEmitBulletImp.ImpFunction=EmitBullet;
            //infos.Add(lEmitBulletImp);
            actionImpDuringFireAnimation.addImp(iTime, EmitBullet);

            //FIXME_VAR_TYPE lEmitBulletSoundImp=AnimationImpTimeListInfo();
            //lEmitBulletSoundImp.ImpTime=iTime;
            //lEmitBulletSoundImp.ImpFunction=EmitBulletSound;
            //infos.Add(lEmitBulletSoundImp);
            //actionImpDuringFireAnimation.addImp(iTime,EmitBulletSound);
        }
        //AnimationImpTimeListInfo[] lTemp = infos.ToBuiltin( AnimationImpTimeListInfo );
        //actionImpDuringFireAnimation.setImpInfoList(lTemp);
        //actionImpDuringFireAnimation.ImpFunction=EmitBullet;
        actionImpDuringFireAnimation.endAddImp();
        mZZSprite.setListener("fire", actionImpDuringFireAnimation);
        //}
        //死亡的后的动作
        //{
        //FIXME_VAR_TYPE lDeadInfos=Array();
        //FIXME_VAR_TYPE lDeadImp=AnimationImpTimeListInfo();
        //lDeadImp.ImpTime=deadDisappearTimePos;
        //lDeadImp.ImpFunction=disappear;
        //lDeadInfos.Add(lDeadImp);
        actionImpDuringDeadAnimation.setImpInfoList(
                new AnimationImpTimeListInfo[] { new AnimationImpTimeListInfo(deadDisappearTimePos, disappear) }
            );
        mZZSprite.setListener("dead", actionImpDuringDeadAnimation);
        //}

        emitter.setBulletLayer(getBulletLayer());
        UpdateFaceShow();
    }

    public int getBulletLayer()
    {
        //子弹所在层名字为:种族名字+Bullet
        //return LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" );
        return LayerMask.NameToLayer(LayerMask.LayerToName(transform.Find("CubeReact").gameObject.layer) + "Bullet");
    }

    public void EmitBullet()
    {
        //print("EmitBullet");
        emitter.EmitBullet();
    }

    //function EmitBulletSound()
    //{
    //print("EmitBullet");
    //	fireSound.Play();
    //}

    //在死亡的回调中使用
    public void deadAction(Life p)
    {
        mZZSprite.playAnimation("dead");
        gameObject.layer = layers.deadObject;
        transform.Find("CubeReact").gameObject.layer = layers.deadObject;

        collisionLayer.updateCollider(gameObject);

        /*//奖励英雄
        Hashtable lInjureInfo = life.getInjureInfo();
        if(lInjureInfo && lInjureInfo.ContainsKey("bagControl"))
        {
            zzItemBagControl lBagControl = lInjureInfo["bagControl"];
            lBagControl.addMoney(shootAward);
        }*/
    }

    public void disappear()
    {
        //zzCreatorUtility.Destroy(gameObject);
        Destroy(gameObject);
    }

    public void UpdateFaceShow()
    {
        int lFace = actionCommandControl.getFaceValue();
        //Xscale=|reverseObjectTransform.localScale.x|,省去判断正负
        Vector3 lTemp = reverseObjectTransform.localScale;
        lTemp.x = lFace * Xscale;
        //reverseObjectTransform.localScale.x = lFace * Xscale;
        reverseObjectTransform.localScale = lTemp;
        //moveV.x=lMove;
        if (lFace == 1)
            turnObjectTransform.rotation = new Quaternion(0, 0, 0, 1);
        else
            turnObjectTransform.rotation = new Quaternion(0, 1, 0, 0);
    }

    //更新动画
    void Update()
    {
        //moveV.x=0;
        //moveV.z=0;
        if (life.isDead())
        {
            //mZZSprite.playAnimation("dead");
            return;
        }

        if (actionCommandControl.updateFace())
            UpdateFaceShow();

        UnitActionCommand lActionCommand = actionCommandControl.getCommand();

        //设置动画 动作
        if (lActionCommand.Fire)
        {
            mZZSprite.playAnimation("fire");
        }
        else
        {
            if (lActionCommand.GoForward)
            {
                mZZSprite.playAnimation("run");
                //moveV.x=face;
                //print("run");
            }
            else
            {
                mZZSprite.playAnimation("stand");
                //print(gameObject.name);
                //print("stand");
                //print(lActionCommand);
            }

        }
    }

    //更新characterController
    void FixedUpdate()
    {
        character.update(actionCommandControl.getCommand(), actionCommandControl.getFaceValue(), life.isAlive());
        /*	

            FIXME_VAR_TYPE lActionCommand= actionCommandControl.getCommand();
            if(life.isAlive() && grounded)
            {
                if( !lActionCommand.FaceDown)
                {
                    if(lActionCommand.Jump)
                        moveV.y = jumpSpeed;
                    else
                        moveV.y = -0.1f;		
                }
            }
            else
                moveV.y -= gravity* Time.deltaTime;//死掉后 moveV.y 就不用清零了
		
            // Move the controller
            FIXME_VAR_TYPE flags= characterController.Move(Vector3(moveV.x * runSpeed,moveV.y,0)* Time.deltaTime);
            grounded = (flags & CollisionFlags.CollidedBelow) != 0;
            //if(userControl || clearCommandEveryFrame)
            //	actionCommand.clear();
            */
    }

}