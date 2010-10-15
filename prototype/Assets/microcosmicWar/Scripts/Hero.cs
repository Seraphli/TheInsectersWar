
using UnityEngine;
using System.Collections;
[RequireComponent(typeof(destroyWhenDie))]
public class Hero : MonoBehaviour
{

    public float runSpeed = 2.0f;
    //FIXME_VAR_TYPE userControl=false;
    //FIXME_VAR_TYPE controlByOther=false;
    public float gravity = 10.0f;
    public float jumpSpeed = 8.0f;
    public zzCharacter character = new zzCharacter();

    public Emitter emitter;

    //被打死后小时的时间
    public float deadDisappearTimePos = 4.0f;

    //AudioSource fireSound;

    //在播放死亡动画时,会执行的动作
    protected AnimationImpInTimeList actionImpDuringDeadAnimation = new AnimationImpInTimeList();

    //protected Transform turnObjectTransform;
    public Transform reverseObjectTransform;

    protected float Xscale;
    //protected ZZSprite mZZSprite;
    //protected CharacterController characterController;

    //private FIXME_VAR_TYPE moveV= Vector3.zero;
    //private bool grounded = false;

    private Life life;

    private Animation myAnimation;

    public BoardDetector boardDetector;

    public bool inFiring;

    public BodyActionInfo upBodyActionInfo = new BodyActionInfo();
    public BodyActionInfo downBodyActionInfo = new BodyActionInfo();


    public BodyAction upBodyAction = new BodyAction();
    public BodyAction downBodyAction = new BodyAction();

    public IobjectListener objectListener;


    //原始的朝向
    public UnitFaceDirection originalFace = UnitFaceDirection.left;

    //Component.SendMessage ("dieCallFunction")
    //Component dieCallFunction;

    //var upBodyAction

    //角色的朝向
    //protected FIXME_VAR_TYPE face= -1;
    public ActionCommandControl actionCommandControl;
    /*
    void  getVelocity (){
        return moveV;
    }

    void  setVelocity ( Vector3 pVelocity  ){
        moveV=pVelocity;
    }
    */

    public zzCharacter getCharacter()
    {
        return character;
    }

    public int getFaceDirection()
    {
        return actionCommandControl.getFaceValue();
    }

    public UnitFaceDirection getFace()
    {
        return actionCommandControl.getFace();
    }

    //Transform upBody;

    void Start()
    {

        if (!myAnimation)
            myAnimation = GetComponentInChildren<Animation>();
        if (!actionCommandControl)
            actionCommandControl = GetComponent<ActionCommandControl>();

        upBodyAction.init(upBodyActionInfo, myAnimation);
        downBodyAction.init(downBodyActionInfo, myAnimation);

        //characterController = GetComponentInChildren<CharacterController>();
        character.characterController = GetComponentInChildren<CharacterController>();
        /*
        character.runSpeed=runSpeed;
        character.gravity = gravity;
        character.jumpSpeed = jumpSpeed;
        */
        //characterController.Move(Vector3(0,0,0));
        emitter = GetComponentInChildren<Emitter>();


        life = GetComponentInChildren<Life>();
        //life.setDieCallback(deadAction);
        life.addDieCallback(deadAction);

        //?
        //characterController .detectCollisions=false;

        collisionLayer.addCollider(gameObject);

        if (!reverseObjectTransform)
            reverseObjectTransform = transform;

        //Xscale=transform.localScale.x;

        Xscale = Mathf.Abs(reverseObjectTransform.localScale.x);

        //}
        //死亡的后的动作
        //actionImpDuringDeadAnimation.setImpInfoList(
        //[AnimationImpTimeListInfo(deadDisappearTimePos,disappear)]
        //);
        //mZZSprite.setListener("dead",actionImpDuringDeadAnimation);
        //}

        emitter.setBulletLayer(getBulletLayer());
        UpdateFaceShow();
    }

    public int getBulletLayer()
    {
        //子弹所在层名字为:种族名字+Bullet
        return LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer) + "Bullet");
    }

    public void EmitBullet()
    {
        //print("EmitBullet");
        if (life.isDead())
        {
            //防止死后开火动画仍然播放,导致开火
            return;
        }
        emitter.EmitBullet();
    }

    //在死亡的回调中使用
    public void deadAction(Life p)
    {
        //mZZSprite.playAnimation("dead");
        gameObject.layer = layers.deadObject;
        transform.Find("CubeReact").gameObject.layer = layers.deadObject;

        collisionLayer.updateCollider(gameObject);
        disappear();
    }

    public void disappear()
    {
        //zzCreatorUtility.Destroy(gameObject);
        //if(dieCallFunction)
        //	dieCallFunction.SendMessage ("dieCallFunction");
        if (objectListener)
            objectListener.removedCall();
        //Destroy(gameObject);
    }

    public void UpdateFaceShow()
    {

        //Xscale=|reverseObjectTransform.localScale.x|,省去判断正负
        //reverseObjectTransform.localScale.x=face*Xscale;
        //reverseObjectTransform.localScale.x=actionCommandControl.getFaceValue()*Xscale;
        //moveV.x=lMove;
        Vector3 lTemp = reverseObjectTransform.localScale;
        if (originalFace == actionCommandControl.getFace())
            lTemp.x = Xscale;
            //reverseObjectTransform.localScale.x = Xscale;
        else
            lTemp.x = -Xscale;
            //reverseObjectTransform.localScale.x = -Xscale;
        reverseObjectTransform.localScale = lTemp;
    }

    //更新动画
    public void Update()
    {

        //moveV.x=0;
        //moveV.z=0;
        UnitActionCommand lActionCommand;

        if (life.isDead())
        {
            return;
        }
        else
        {

            if (actionCommandControl.updateFace())
                UpdateFaceShow();

            lActionCommand = actionCommandControl.getCommand();
            //设置动画 动作
            if (lActionCommand.Fire)
            {
                upBodyAction.playAction("fire");
            }
            else
                upBodyAction.playAction("standby");

            if (character.isGrounded())
                if (lActionCommand.GoForward)
                {
                    downBodyAction.playAction("run");
                }
                else
                {
                    downBodyAction.playAction("stand");
                }
            else
                downBodyAction.playAction("onAir");


            if (lActionCommand.Jump && lActionCommand.FaceDown)
            {
                boardDetector.down();
            }
            else
                boardDetector.recover();

            updatePosture(lActionCommand.FaceUp, lActionCommand.FaceDown, lActionCommand.GoForward);
            //print(""+actionCommand.FaceUp+actionCommand.FaceDown+actionCommand.GoForward);
        }
    }

    public void updatePosture(bool pUp, bool pDwon, bool pForward)
    {
        if (pUp == pDwon)
        {
            upBodyAction.playActionType("angle0");
            return;
        }

        if (pUp)
        {
            if (pForward)
                upBodyAction.playActionType("angle45");
            else
                upBodyAction.playActionType("angle90");
        }
        else
        {
            if (pForward)
                upBodyAction.playActionType("angle-45");
            else
                upBodyAction.playActionType("angle-90");
        }
    }

    //FIXME_VAR_TYPE preIsGrounded= true;

    //function isGrounded()
    //{
    //	return preIsGrounded==true || characterController.isGrounded==true;
    //}

    //更新characterController
    public void FixedUpdate()
    {
        character.update(actionCommandControl.getCommand(), actionCommandControl.getFaceValue(), life.isAlive());
        /*
            FIXME_VAR_TYPE lActionCommand= actionCommandControl.getCommand();
	
            if(life.isAlive() && characterController.isGrounded)
            {
                if( !lActionCommand.FaceDown)
                {
                    if(lActionCommand.Jump)
                        moveV.y = jumpSpeed;
                    else
                        moveV.y = -0.1f;	//以免飞起来
                }
            }
            else
                moveV.y -= gravity* Time.deltaTime;
		
            // Move the controller
            FIXME_VAR_TYPE lVelocity=Vector3(moveV.x * runSpeed,moveV.y,0);

            CollisionFlags flags=characterController.Move(lVelocity* Time.deltaTime);
        */

    }

}