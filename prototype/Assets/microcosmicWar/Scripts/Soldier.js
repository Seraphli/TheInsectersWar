
var runSpeed=2.0;
var userControl=false;
//var controlByOther=false;
var gravity = 10.0;
var jumpSpeed = 8.0;

var actionCommand:UnitActionCommand;

var clearCommandEveryFrame=true;

var emitter:Emitter;

//在播放射击动画时,会执行的动作
var actionImpDuringAnimation=AnimationImpInTime();

protected var turnObjectTransform:Transform;
protected var reverseObjectTransform:Transform;

protected var Xscale:float;
protected var mZZSprite:ZZSprite;
protected var characterController:CharacterController;

private var moveV = Vector3.zero;
private var grounded : boolean = false;

//角色的朝向
protected var face = -1;

function getVelocity()
{
	return moveV;
}

function setVelocity(pVelocity:Vector3)
{
	moveV=pVelocity;
}

function getFaceDirection()
{
	return face;
}

class UnitActionCommand
{
	//None,
	
	var FaceLeft=false;//朝向左
	var FaceRight=false;//朝向右
	var GoForward=false;//前进
	var Fire=false;//开火
	var Jump=false;//跳跃
	
	function clear()
	{
		MoveLeft=false;
		MoveRight=false;
		Fire=false;
		Jump=false;
		FaceLeft=false;
		FaceRight=false;
		GoForward=false;
	}
}

function Start()
{
	//控制权
	//userControl=userControl&&zzCreatorUtility.isMine(networkView);
	//userControl=userControl&&zzCreatorUtility.isHost();

	mZZSprite = GetComponentInChildren(ZZSprite);
	characterController = GetComponentInChildren(CharacterController);
	emitter = GetComponentInChildren(Emitter);
	
	characterController .detectCollisions=false;
	
	collisionLayer.addCollider(gameObject);

	Xscale=transform.localScale.x;
	
	turnObjectTransform= transform.Find("turn").transform;
	reverseObjectTransform= transform.Find("reverse").transform;
	
	actionImpDuringAnimation.ImpFunction=EmitBullet;
	
	//当为host 时,允许发射子弹
	//emitter.EmitBullet 也有判断,可去除一处
	if( zzCreatorUtility.isHost())
	{
		//print("userControl || zzCreatorUtility.isHost()");
		actionImpDuringAnimation.ImpFunction=EmitBullet;
		mZZSprite.setListener("fire",actionImpDuringAnimation);
	}
	
	emitter.setBulletLayer( getBulletLayer() );
	UpdateFaceShow();
}

function getBulletLayer()
{
	//子弹所在层名字为:种族名字+Bullet
	return LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" );
}

function EmitBullet()
{
	//print("EmitBullet");
	emitter.EmitBullet();
}

function GetActionCommandFromInput()
{
	var lActionCommand=UnitActionCommand();
	//lActionCommand.MoveLeft=Input.GetButton ("left");
	if(Input.GetButton ("left"))
	{
		lActionCommand.FaceLeft=true;
		lActionCommand.GoForward=true;
	}
	//lActionCommand.MoveRight=Input.GetButton ("right");
	if(Input.GetButton ("right"))
	{
		lActionCommand.FaceRight=true;
		lActionCommand.GoForward=true;
	}
	lActionCommand.Fire=Input.GetButton ("fire");
	lActionCommand.Jump=Input.GetButton ("jump");
	return lActionCommand;
}

function UpdateFaceShow()
{
	
	//Xscale=|reverseObjectTransform.localScale.x|,省去判断正负
	reverseObjectTransform.localScale.x=face*Xscale;
	//moveV.x=lMove;
	if(face==1)
		turnObjectTransform.rotation=Quaternion(0,0,0,1);
	else
		turnObjectTransform.rotation=Quaternion(0,1,0,0);
}

//更新动画
function Update() 
{	
	moveV.x=0;
	moveV.z=0;
	if(userControl)
		actionCommand=GetActionCommandFromInput();


		//设置角色朝向
	//{
		var lMove = 0;
		if(actionCommand.FaceLeft)
		{
			lMove+=-1;
		}
		if(actionCommand.FaceRight)
		{
			lMove+=1;
		}
		
		if(lMove!=0 && lMove!=face )
		{
			face=lMove;
			UpdateFaceShow();
		}
		
	//}
	
		//设置动画 动作
	if(actionCommand.Fire)
	{
		mZZSprite.playAnimation("fire");
	}
	else
	{
		if(actionCommand.GoForward)
		{
			mZZSprite.playAnimation("run");
			moveV.x=face;
		}
		else
		{
			mZZSprite.playAnimation("stand");
		}
		
	}
}

//更新characterController
function FixedUpdate()
{
	
	//if (characterController.isGrounded && actionCommand.Jump)
	if (grounded && actionCommand.Jump)
	{
		moveV.y = jumpSpeed;
		//controller.animation
	}
	
	moveV.y -= gravity* Time.deltaTime;
	// Move the controller
	var flags = characterController.Move(Vector3(moveV.x * runSpeed,moveV.y,0)* Time.deltaTime);
	grounded = (flags & CollisionFlags.CollidedBelow) != 0;
	//if(userControl || clearCommandEveryFrame)
	//	actionCommand.clear();
}

function setCommand(pActionCommand:UnitActionCommand)
{
	actionCommand=pActionCommand;
}

function getCommand()
{
	return actionCommand;
}