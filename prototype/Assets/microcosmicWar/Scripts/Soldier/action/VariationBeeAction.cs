using UnityEngine;

public class VariationBeeAction : SoldierAction
{
    public Animation characterAnimation;

    public float delayAfterAniEnd = 0.5f;

    public string animationName = "action1";

    public override bool inActing
    {
        set 
        {
            if (_inActing==value)
                return;
            _inActing = value;
            if (_inActing)
            {
                OnActionStart();
            }
            else
            {
                OnActionEnd();
            }
        }
    }

    //void Awake()
    //{
    //    //enabled = false;
    //    //timer.setInterval(delayAfterAniEnd);
    //    //timer.addImpFunction(OnActionEnd);
    //    characterController = character.characterController;
    //}

    public override void processCommand(UnitActionCommand pUnitActionCommand)
    {
        pUnitActionCommand.face = actionFace;
        pUnitActionCommand.GoForward = !characterController.isGrounded;
        if(actionJump)
        {
            pUnitActionCommand.Jump = actionJump;
            actionJump = false;
        }
    }

    [SerializeField]
    UnitFaceDirection actionFace;

    //public override bool beginAction()
    //{
    //    if (!characterController.isGrounded)
    //        return false;
    //    base.beginAction();
    //    actionFace = actionCommandControl.face;
    //    //characterAnimation.CrossFade("action1", 0.2f);
    //    return true;
    //}

    public bool actionJump = false;
    //public bool actionInAir = false;

    public void OnActionJump()
    {
        actionJump = true;
    }

    //void OnActionTouchdown()
    //{
    //    actionInAir = false;
    //}

    public void OnActionAttack()
    {

    }

    //public override void interruptAction() 
    //{
    //    OnActionEnd();
    //}

    //void OnAinmationEnd()
    //{
    //    //enabled = true;
    //    //timer.timePos = 0f;
    //    _inActing = false;
    //}

    public void OnActionStart()
    {
        actionFace = actionCommandControl.face;
        characterAnimation.CrossFade(animationName, 0.2f);
    }

    public void OnActionEnd()
    {
        _inActing = false;
        actionCommandControl.commandValue &= ~commandValue;
        //enabled = false;
        //timer.timePos = 0f;
    }

    //zzTimerClass timer = new zzTimerClass();

    //void Update()
    //{
    //    timer.Update();
    //}
}