using UnityEngine;

public class SoldierAction:MonoBehaviour
{
    public ActionCommandControl actionCommandControl;
    public Character2D character;
    public CharacterController characterController;
    public bool canChangeFace;
    public bool canMove;
    public int commandValue;

    public Animation characterAnimation;
    public string animationName;
    public AnimationState animationState;
    public float animationFadeLength = 0.2f;
    public bool endActionWhenAinmationEnd = true;
    public zzTimer timer;

    public virtual bool inActing
    {
        get { return _inActing; }

        set
        {
            if (_inActing == value)
                return;
            if (value)
            {
                OnActionStart();
            }
            else
            {
                OnActionEnd();
            }
        }
    }

    public virtual void OnActionStart() 
    {
        _inActing = true;
        if (characterAnimation)
        {
            if (endActionWhenAinmationEnd)
            {
                animationState.time = animationState.time % animationState.length;
                float lTime;
                if (animationState.enabled)
                    lTime = (animationState.length * 2 - animationState.time)
                        / animationState.speed - 0.2f;
                else
                    lTime = animationState.length / animationState.speed - 0.2f;
                timer.enabled = true;
                timer.timePos = 0f;
                timer.setInterval(lTime);
                //print("animationState:" + animationState.time
                //    + " length:" + animationState.length
                //    + " speed:" + animationState.speed
                //    + " lTime:" + lTime);
            }
            characterAnimation.CrossFade(animationName, animationFadeLength);
        }
    }

    //自行终止 和 强制终止
    public virtual void OnActionEnd()
    {
        //print("OnActionEnd");
        _inActing = false;
        timer.enabled = false;
        actionCommandControl.commandValue &= ~commandValue;
    }

    protected void Awake()
    {
        characterController = character.characterController;
        timer = gameObject.AddComponent<zzTimer>();
        timer.addImpFunction(OnActionEnd);
        timer.enabled = false;
        _inActing = false;
        if(characterAnimation)
            animationState = characterAnimation[animationName];
    }
    //public virtual bool canInterrupt
    //{
    //    get { return true; }
    //}

    //public virtual bool canJump
    //{
    //    get { return false; }
    //}

    //public virtual bool canXMove
    //{
    //    get { return false; }
    //}

    [SerializeField]
    protected bool _inActing = false;

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <returns>是否成功开启动作</returns>
    //public virtual bool beginAction()
    //{
    //    _inActing = true;
    //    return true;
    //}

    public virtual void processCommand( UnitActionCommand pUnitActionCommand){}
    //public virtual void interruptAction() { }

}