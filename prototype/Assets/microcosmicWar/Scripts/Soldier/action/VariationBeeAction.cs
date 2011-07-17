using UnityEngine;

public class VariationBeeAction : SoldierAction
{
    //public Animation characterAnimation;

    public float delayAfterAniEnd = 0.5f;

    //public string animationName = "action1";

    public float speedInAir = 4f;

    public float downJumpSpeed = 8f;

    public zzDetectorBase landingPointDetector;

    //public LayerMask groundMask;

    public float speedOnGround;

    public float originalJumpSpeed;

    public float halfHeight;

    //public override bool inActing
    //{
    //    set 
    //    {
    //        if (_inActing==value)
    //            return;
    //        _inActing = value;
    //        if (_inActing)
    //        {
    //            OnActionStart();
    //        }
    //        else
    //        {
    //            OnActionEnd();
    //        }
    //    }
    //}

    void Awake()
    {
        //enabled = false;
        //timer.setInterval(delayAfterAniEnd);
        //timer.addImpFunction(OnActionEnd);
        //characterController = character.characterController;
        base.Awake();
        speedOnGround = character.runSpeed;
        originalJumpSpeed = character.jumpSpeed;
        halfHeight = characterController.height
            * characterController.transform.localScale.y / 2f;
    }

    public override void OnActionStart()
    {
        base.OnActionStart();
        var lHit = landingPointDetector._impDetect(layers.standPlaceValue);
        //print(lHit.Length);
        if(lHit.Length>0)
        {
            //print(lHit[0].transform.name);
            var landingPointY = lHit[0].point.y;
            if (transform.position.y - halfHeight >= landingPointY)
                character.jumpSpeed = downJumpSpeed;
        }
        else
            character.jumpSpeed = downJumpSpeed;
    }

    public override void processCommand(UnitActionCommand pUnitActionCommand)
    {
        //pUnitActionCommand.face = actionFace;
        pUnitActionCommand.GoForward = !characterController.isGrounded;
        if(actionJump)
        {
            pUnitActionCommand.Jump = actionJump;
            actionJump = false;
        }
    }

    //[SerializeField]
    //UnitFaceDirection actionFace;

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
        character.runSpeed = speedInAir;
    }

    //void OnActionTouchdown()
    //{
    //    actionInAir = false;
    //}

    public void OnActionAttack()
    {
        character.runSpeed = speedOnGround;
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

    //public override void OnActionStart()
    //{
    //    //_inActing = true;
    //    //actionFace = actionCommandControl.face;
    //    characterAnimation.CrossFade(animationName, 0.2f);
    //}

    //自行终止 和 强制终止
    public override void OnActionEnd()
    {
        //_inActing = false;
        //actionCommandControl.commandValue &= ~commandValue;
        //enabled = false;
        //timer.timePos = 0f;
        base.OnActionEnd();
        character.runSpeed = speedOnGround;
        character.jumpSpeed = originalJumpSpeed;
    }

    //zzTimerClass timer = new zzTimerClass();

    //void Update()
    //{
    //    timer.Update();
    //}
}