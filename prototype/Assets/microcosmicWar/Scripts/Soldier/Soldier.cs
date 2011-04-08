
using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour
{
    public Animation characterAnimation;

    public zzCharacter character = new zzCharacter();

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

        if (!characterAnimation)
            characterAnimation = GetComponentInChildren<Animation>();

        character.characterController = GetComponentInChildren<CharacterController>();

        emitter = GetComponentInChildren<Emitter>();
        life = GetComponentInChildren<Life>();

        if (!actionCommandControl)
            actionCommandControl = GetComponentInChildren<ActionCommandControl>();

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
        characterAnimation.CrossFade("dead", 0.3f);
        gameObject.layer = layers.deadObject;
        //transform.Find("CubeReact").gameObject.layer = layers.deadObject;

        //collisionLayer.updateCollider(gameObject);

    }

    //public void disappear()
    //{
    //    //zzCreatorUtility.Destroy(gameObject);
    //    Destroy(gameObject);
    //}

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
        if (life.isDead())
        {
            return;
        }

        if (actionCommandControl.updateFace())
            UpdateFaceShow();

        UnitActionCommand lActionCommand = actionCommandControl.getCommand();

        //设置动画 动作
        if (lActionCommand.Fire)
        {
            characterAnimation.CrossFade("fire", 0.2f);
        }
        else
        {
            if (lActionCommand.GoForward)
            {
                characterAnimation.CrossFade("run", 0.1f);
                //moveV.x=face;
                //print("run");
            }
            else
            {
                characterAnimation.CrossFade("stand", 0.2f);
                //print(gameObject.name);
                //print("stand");
                //print(lActionCommand);
            }

        }

        if (lActionCommand.Jump && lActionCommand.FaceDown)
        {
            boardDetector.down();
        }
        else
            boardDetector.recover();
    }

    //更新characterController
    void FixedUpdate()
    {
        character.update(actionCommandControl.getCommand(), actionCommandControl.getFaceValue(), life.isAlive());
    }

}