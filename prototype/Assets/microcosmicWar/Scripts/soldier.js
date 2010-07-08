
var runSpeed=2.0;
var userControl=false;
var gravity = 10.0;
var jumpSpeed = 8.0;

var actionCommand:ActionCommand;

var clearCommandEveryFrame=true;

var emitter:Emitter;

var actionImpDuringAnimation=AnimationImpInTime();

protected var turnObjectTransform:Transform;
protected var reverseObjectTransform:Transform;

protected var Xscale:float;
protected var mZZSprite:ZZSprite;
protected var characterController:CharacterController;

private var moveV = Vector3.zero;
private var grounded : boolean = false;

class ActionCommand
{
	//None,
	var MoveLeft=false;
	var MoveRight=false;
	var Fire=false;
	var Jump=false;
	
	function clear()
	{
		MoveLeft=false;
		MoveRight=false;
		Fire=false;
		Jump=false;
	}
}

function Start()
{
	mZZSprite = GetComponentInChildren(ZZSprite);
	characterController = GetComponentInChildren(CharacterController);
	emitter = GetComponentInChildren(Emitter);
	
	characterController .detectCollisions=false;
	
	collisionLayer.addCollider(gameObject);

	Xscale=transform.localScale.x;
	
	turnObjectTransform= transform.Find("turn").transform;
	reverseObjectTransform= transform.Find("reverse").transform;
	
	actionImpDuringAnimation.ImpFunction=EmitBullet;
	mZZSprite.setListener("fire",actionImpDuringAnimation);
}

function EmitBullet()
{
	emitter.EmitBullet();
}

function GetActionCommandFromInput()
{
	var lActionCommand=ActionCommand();
	lActionCommand.MoveLeft=Input.GetButton ("left");
	lActionCommand.MoveRight=Input.GetButton ("right");
	lActionCommand.Fire=Input.GetButton ("fire");
	lActionCommand.Jump=Input.GetButton ("jump");
	return lActionCommand;
}

function FixedUpdate() 
{	
	moveV.x=0;
	moveV.z=0;
	if(userControl)
		actionCommand=GetActionCommandFromInput();

	if(actionCommand.Fire)
	{
		mZZSprite.playAnimation("fire");
	}
	else
	{
		var lMove = 0;
		if(actionCommand.MoveLeft)
		{
			lMove+=-1;
		}
		if(actionCommand.MoveRight)
		{
			lMove+=1;
		}
		
		if(lMove!=0)
		{
			mZZSprite.playAnimation("run");
			reverseObjectTransform.localScale.x=lMove*Xscale;
			moveV.x=lMove;
			if(lMove==1)
				turnObjectTransform.rotation=Quaternion(0,0,0,1);
			else
				turnObjectTransform.rotation=Quaternion(0,1,0,0);
			//characterController.Move(Vector3(lMove,0,0)*runSpeed * Time.deltaTime);
		}
		else
		{
			mZZSprite.playAnimation("stand");
		}
	}
	if (characterController.isGrounded && actionCommand.Jump)
	{
		moveV.y = jumpSpeed;
		//controller.animation
	}
	
	moveV.y -= gravity* Time.deltaTime;
	// Move the controller
	var flags = characterController.Move(Vector3(moveV.x * runSpeed,moveV.y,0)* Time.deltaTime);
	grounded = (flags & CollisionFlags.CollidedBelow) != 0;
	if(userControl || clearCommandEveryFrame)
		actionCommand.clear();
}