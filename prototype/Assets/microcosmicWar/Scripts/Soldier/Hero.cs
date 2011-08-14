
using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{

    //public zzCharacter character = new zzCharacter();
    public Character2D character2D;

    public Emitter emitter;

    //被打死后小时的时间
    //public float deadDisappearTimePos = 4.0f;

    //AudioSource fireSound;

    //在播放死亡动画时,会执行的动作
    //protected AnimationImpInTimeList actionImpDuringDeadAnimation = new AnimationImpInTimeList();

    //protected Transform turnObjectTransform;
    public Transform reverseObjectTransform;

    protected float Xscale;


    private Life life;

    private Animation myAnimation;

    public BoardDetector boardDetector;

    public zzAutoDetect standDetector;

    //public bool inFiring;

    public BodyActionInfo upBodyActionInfo = new BodyActionInfo();
    public BodyActionInfo downBodyActionInfo = new BodyActionInfo();


    public BodyAction upBodyAction;
    public BodyAction downBodyAction;

    public IobjectListener objectListener;


    //原始的朝向
    public UnitFaceDirection originalFace = UnitFaceDirection.left;

    public ActionCommandControl actionCommandControl;

    public Character2D getCharacter()
    {
        return character2D;
    }

    public int getFaceDirection()
    {
        return actionCommandControl.getFaceValue();
    }

    public UnitFaceDirection getFace()
    {
        return actionCommandControl.face;
    }

    //Transform upBody;

    void Start()
    {

        if (!myAnimation)
            myAnimation = GetComponentInChildren<Animation>();

        emitter = GetComponentInChildren<Emitter>();


        life = GetComponentInChildren<Life>();
        //life.setDieCallback(deadAction);
        life.addDieCallback(deadAction);


        if (!reverseObjectTransform)
            reverseObjectTransform = transform;


        Xscale = Mathf.Abs(reverseObjectTransform.localScale.x);


        emitter.setBulletLayer(getBulletLayer());
        UpdateFaceShow();
    }

    public int getBulletLayer()
    {
        //子弹所在层名字为:种族名字+Bullet
        //return LayerMask.NameToLayer(LayerMask.LayerToName(gameObject.layer) + "Bullet");
        return PlayerInfo.getBulletLayer(gameObject.layer);
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
        //transform.Find("CubeReact").gameObject.layer = layers.deadObject;

        //collisionLayer.updateCollider(gameObject);
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
        if (originalFace == actionCommandControl.face)
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

        if (life.isAlive())
        {

            if (actionCommandControl.updateFace())
                UpdateFaceShow();

            lActionCommand = actionCommandControl.getCommand();
            //设置动画 动作
            //if (lActionCommand.Fire)
            //{
            //    upBodyAction.playAction("fire");
            //}
            //else
            //    upBodyAction.playAction("standby");
            processAction(lActionCommand);

            //if (character.isGrounded())
            if (character2D.isGrounded() || standDetector.result.Length != 0)
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
        character2D.update2D(actionCommandControl.getCommand(), actionCommandControl.getFaceValue(), life.isAlive());
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

    SoldierAction _nowAction;

    public SoldierAction nowAction
    {
        get { return _nowAction; }
        set
        {
            //print("_nowAction:" + (_nowAction == null) + " value:" + (value == null));
            if (_nowAction && _nowAction != value)
            {
                _nowAction.inActing = false;
            }
            _nowAction = value;
            if (_nowAction)
                _nowAction.inActing = true;
        }
    }

    void OnCommand(UnitActionCommand pCommand)
    {
        if (nowAction)
        {
            pCommand.Action1 = action1.inActing;
            pCommand.Action2 = action2.inActing;
            pCommand.Fire = fireAction.inActing;
        }
    }

    void Awake()
    {
        fireAction.commandValue = UnitActionCommand.fireCommand;
        action1.commandValue = UnitActionCommand.action1Command;
        action2.commandValue = UnitActionCommand.action2Command;
        actionCommandControl.addCommandChangedReciver(OnCommand);
    }

    public SoldierAction fireAction;

    public SoldierAction action1;

    public SoldierAction action2;

    void processAction(UnitActionCommand lActionCommand)
    {
        if (nowAction && !nowAction.inActing)
        {
            nowAction = null;
        }
        if (nowAction)
        {
            nowAction.processCommand(lActionCommand);
        }
        else if (lActionCommand.Fire)
        {
            nowAction = fireAction;
        }
        else if (lActionCommand.Action1)
        {
            nowAction = action1;
        }
        else if (lActionCommand.Action2)
        {
            nowAction = action2;
        }
    }

}