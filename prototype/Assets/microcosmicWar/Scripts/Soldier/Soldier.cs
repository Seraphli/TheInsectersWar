
using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour
{
    public Animation characterAnimation;

    public zzCharacter character = new zzCharacter();

    public Character2D character2D;

    //UnitActionCommand actionCommand;

    //public bool clearCommandEveryFrame = true;

    public Emitter emitter;

    public int pointCount = 1;

    //改在zzAnimationConfig组件中实现
    //射击在动画的动画,要按从小到大排
    //public float[] fireTimeList;

    //改在destroyWhenDie组件中实现
    //被打死后消失的时间
    //public float deadDisappearTimePos = 4.0f;


    protected Transform turnObjectTransform;
    protected Transform reverseObjectTransform;

    protected float Xscale;


    private Life life;

    public ActionCommandControl actionCommandControl;

    public BoardDetector boardDetector;

    public ActionCommandControl getActionCommandControl()
    {
        return actionCommandControl;
    }

    public Character2D getCharacter()
    {
        return character2D;
    }

    public int getFaceDirection()
    {
        return actionCommandControl.getFaceValue();
    }

    public SoldierAction fireAction;

    public SoldierAction action1;

    public SoldierAction action2;

    [SerializeField]
    SoldierAction _nowAction;

    void Awake()
    {
        if (!fireAction)
        {
            fireAction = gameObject.AddComponent<SoldierAction>();
            fireAction.characterAnimation = characterAnimation;
            fireAction.animationName = "fire";
            fireAction.character = character2D;
            fireAction.actionCommandControl = actionCommandControl;
            fireAction.canChangeFace = true;
            fireAction.canMove = false;
            fireAction.init();
        }
        fireAction.commandValue = UnitActionCommand.fireCommand;
        if (!action1)
            action1 = gameObject.AddComponent<SoldierAction>();
        if (!action2)
            action2 = gameObject.AddComponent<SoldierAction>();
        action1.commandValue = UnitActionCommand.action1Command;
        action2.commandValue = UnitActionCommand.action2Command;
        actionCommandControl.addCommandChangedReciver(OnCommand);
    }


    void Start()
    {

        if (!characterAnimation)
            characterAnimation = GetComponentInChildren<Animation>();

        emitter = GetComponentInChildren<Emitter>();
        life = GetComponentInChildren<Life>();

        //if (!actionCommandControl)
        //    actionCommandControl = GetComponentInChildren<ActionCommandControl>();

        life.addDieCallback(deadAction);


        //collisionLayer.addCollider(gameObject);

        turnObjectTransform = transform.Find("turn").transform;
        reverseObjectTransform = transform.Find("reverse").transform;

        Xscale = reverseObjectTransform.localScale.x;

        if (emitter)
            emitter.setBulletLayer(getBulletLayer());
        UpdateFaceShow();
    }

    public int getBulletLayer()
    {
        return PlayerInfo.getBulletLayer(gameObject.layer);
    }

    public void EmitBullet()
    {
        //print("EmitBullet");
        emitter.EmitBullet();
    }


    //在死亡的回调中使用
    void deadAction(Life p)
    {
        //mZZSprite.playAnimation("dead");
        //characterAnimation.CrossFade("dead", 0.3f);
        gameObject.layer = layers.deadObject;
        //transform.Find("CubeReact").gameObject.layer = layers.deadObject;

        //collisionLayer.updateCollider(gameObject);

    }


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
        //pCommand.Fire = fireAction.inActing;
        if (nowAction)
        {
            pCommand.Action1 = action1.inActing;
            pCommand.Action2 = action2.inActing;
            pCommand.Fire = fireAction.inActing;
            //print(!nowAction.canMove);
            if (!nowAction.canChangeFace)
                pCommand.command &= UnitActionCommand.negFaceCommand;
            if (!nowAction.canMove)
                pCommand.command &= UnitActionCommand.negGoForwardCommand;
        }
        //else if (pCommand.Action1 && pCommand.Action2)
        //    pCommand.Action2 = false;
    }

    /// <summary>
    /// 更新朝向
    /// </summary>
    void UpdateFaceShow()
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
        UnitActionCommand lActionCommand = actionCommandControl.getCommand();
        if (life.isAlive())
        {
            if (actionCommandControl.updateFace())
                UpdateFaceShow();

            //if (nowAction && (lActionCommand.command & nowAction.commandValue) == 0)
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
            else
            {
                ////设置动画 动作
                //if (lActionCommand.Fire)
                //{
                //    characterAnimation.CrossFade("fire", 0.2f);
                //}
                //else
                //{
                if (lActionCommand.GoForward)
                {
                    characterAnimation.CrossFade("run", 0.1f);
                }
                else
                {
                    characterAnimation.CrossFade("stand", 0.2f);
                }

                //}

                if (lActionCommand.Jump && lActionCommand.FaceDown)
                {
                    boardDetector.down();
                }
                else
                    boardDetector.recover();

            }

        }
        else
            nowAction = null;

        character2D.update2D(lActionCommand, actionCommandControl.getFaceValue(), life.isAlive());
    }

}