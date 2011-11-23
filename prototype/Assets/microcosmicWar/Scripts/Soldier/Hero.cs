
using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour
{

    //public zzCharacter character = new zzCharacter();
    public Character2D character2D;

    public Emitter emitter;
    public Emitter subEmitter;

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

    //[SerializeField]
    //WMPurse _purse;
    public WMPurse purse
    {
        get { return characterInfoObject.purse; }
        //set
        //{
        //    _purse = value;
        //    emitter.attackerPurse = value;
        //    if (setPurseEvent != null)
        //        setPurseEvent(value);
        //}
    }
    System.Action<WMPurse> setPurseEvent;

    public void addSetPurseEventReceiver(System.Action<WMPurse> pReceiver)
    {
        setPurseEvent += pReceiver;
    }

    public WMPriceList priceList
    {
        get { return characterInfoObject.priceList; }
    }
    public WMItemBag itemBag
    {
        get { return characterInfoObject.itemBag; }
    }

    public CharacterInfoObject characterInfoObject;

    public CharacterInfo characterInfo
    {
        set 
        {
            characterInfoObject.characterInfo = value;
            emitter.characterInfo = value;
            if (subEmitter)
                subEmitter.characterInfo = value;
            if (setPurseEvent != null)
                setPurseEvent(value.purse);
        }
        get { return characterInfoObject.characterInfo; }
    }

    public LifeFlash lifeFlash;

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

    static void nullValueChangedReceiver(float p) { }

    void Start()
    {

        if (!myAnimation)
            myAnimation = GetComponentInChildren<Animation>();
        if (!_actionEnergyValue)
        {
            var lActionEnergyValue = gameObject.AddComponent<ActionEnergyValue>();
            lActionEnergyValue.addValueChangedReceiver(nullValueChangedReceiver);
            actionEnergyValue = lActionEnergyValue;
        }

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
            if (nowAction && !nowAction.inActing)
            {
                nowAction = null;
            }
            if (nowAction)
            {
                nowAction.processCommand(lActionCommand);
            }

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
        if (character2D.characterController.isGrounded
            && pCommand.Jump
            && !pCommand.FaceDown
            && !_actionEnergyValue.tryUse(jumpCost))
        {
            pCommand.Jump = false;
        }
        processAction(pCommand);
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
    
    [SerializeField]
    ActionEnergyValue _actionEnergyValue;

    public ActionEnergyValue actionEnergyValue
    {
        set
        {
            _actionEnergyValue = value;
            _actionEnergyValue.fullValue = 1f;
            _actionEnergyValue.nowValue = 1f;
            _actionEnergyValue.recoverSpeed = recoverSpeed;
        }
    }

    public float fireCost;
    public float action1Cost;
    public float action2Cost;
    public float jumpCost;
    public float recoverSpeed;

    void processAction(UnitActionCommand lActionCommand)
    {

        //if (nowAction && !nowAction.inActing)
        //{
        //    nowAction = null;
        //}
        //if (nowAction)
        //{
        //    nowAction.processCommand(lActionCommand);
        //}
        //else 
        if (!nowAction)
        {
            if (lActionCommand.Fire && _actionEnergyValue.tryUse(fireCost))
            {
                nowAction = fireAction;
            }
            else if (lActionCommand.Action1 && _actionEnergyValue.tryUse(action1Cost))
            {
                nowAction = action1;
            }
            else if (lActionCommand.Action2 && _actionEnergyValue.tryUse(action2Cost))
            {
                nowAction = action2;
            }
        }
        lActionCommand.Action1 = action1.inActing;
        lActionCommand.Action2 = action2.inActing;
        lActionCommand.Fire = fireAction.inActing;
    }

}