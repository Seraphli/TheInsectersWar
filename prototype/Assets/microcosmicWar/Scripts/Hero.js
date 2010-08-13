
var runSpeed=2.0;
var userControl=false;
//var controlByOther=false;
var gravity = 10.0;
var jumpSpeed = 8.0;

var actionCommand:UnitActionCommand;

var emitter:Emitter;

//射击在动画的动画,要按从小到大排
var fireTimeList:float[];

//被打死后小时的时间
var deadDisappearTimePos=4.0;

//var fireSound:AudioSource;

//在播放死亡动画时,会执行的动作
protected var actionImpDuringDeadAnimation=AnimationImpInTimeList();

protected var turnObjectTransform:Transform;
protected var reverseObjectTransform:Transform;

protected var Xscale:float;
//protected var mZZSprite:ZZSprite;
protected var characterController:CharacterController;

private var moveV = Vector3.zero;
private var grounded : boolean = false;

private var life:Life;

private var myAnimation:Animation;

var boardDetector:BoardDetector;

var inFiring:boolean;

var upBodyActionInfo=BodyActionInfo();
var downBodyActionInfo=BodyActionInfo();


var upBodyAction=BodyAction();
var downBodyAction=BodyAction();

var objectListener=IobjectListener();

//Component.SendMessage ("dieCallFunction")
//var dieCallFunction:Component;

//var upBodyAction

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

var upBody:Transform;

function Start()
{

	if(!myAnimation)
		myAnimation = GetComponentInChildren(Animation);
		
	upBodyAction.init(upBodyActionInfo,myAnimation);
	downBodyAction.init(downBodyActionInfo,myAnimation);

	characterController = GetComponentInChildren(CharacterController);
	//characterController.Move(Vector3(0,0,0));
	emitter = GetComponentInChildren(Emitter);
	

	life= GetComponentInChildren(Life);
	life.setDieCallback(deadAction);
	
	//?
	//characterController .detectCollisions=false;
	
	collisionLayer.addCollider(gameObject);
	
	reverseObjectTransform= transform;

	//Xscale=transform.localScale.x;

	Xscale=reverseObjectTransform.localScale.x;
	
	//}
	//死亡的后的动作
		//actionImpDuringDeadAnimation.setImpInfoList(
				//[AnimationImpTimeListInfo(deadDisappearTimePos,disappear)]
			//);
		//mZZSprite.setListener("dead",actionImpDuringDeadAnimation);
	//}
		
	emitter.setBulletLayer( getBulletLayer() );
	UpdateFaceShow();
}

function getBulletLayer()
{
	//子弹所在层名字为:种族名字+Bullet
	return LayerMask.NameToLayer( LayerMask.LayerToName(transform.Find("CubeReact").gameObject.layer)+"Bullet" );
}

function EmitBullet()
{
	//print("EmitBullet");
	emitter.EmitBullet();
}

//在死亡的回调中使用
function deadAction()
{
	//mZZSprite.playAnimation("dead");
	gameObject.layer=layers.deadObject;
	transform.Find("CubeReact").gameObject.layer=layers.deadObject;
	
	collisionLayer.updateCollider(gameObject);
	
	disappear();
}

function disappear()
{
	//zzCreatorUtility.Destroy(gameObject);
	//if(dieCallFunction)
	//	dieCallFunction.SendMessage ("dieCallFunction");
	objectListener.removedCall();
	Destroy(gameObject);
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
	if(Input.GetButton ("down"))
		lActionCommand.FaceDown=true;
		
	lActionCommand.Fire=Input.GetButton ("fire");
	lActionCommand.Jump=Input.GetButton ("jump");
	return lActionCommand;
}

function UpdateFaceShow()
{
	
	//Xscale=|reverseObjectTransform.localScale.x|,省去判断正负
	reverseObjectTransform.localScale.x=face*Xscale;
	//moveV.x=lMove;
}

//更新动画
function Update() 
{	

	moveV.x=0;
	moveV.z=0;
	
	if( life.isDead() )
	{
		return;
	}
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
		upBodyAction.playAction("fire");
	}
	else
		upBodyAction.playAction("standby");

	if(actionCommand.GoForward)
	{
		downBodyAction.playAction("run");
		moveV.x=face;
	}
	else
	{
		downBodyAction.playAction("stand");
	}
	
	if(actionCommand.Jump&&actionCommand.FaceDown)
	{
		boardDetector.down();
	}
	else
		boardDetector.recover();
		
}

//更新characterController
function FixedUpdate()
{

	
	if(life.isAlive() && grounded)
	{
		if( !actionCommand.FaceDown)
		{
			if(actionCommand.Jump)
				moveV.y = jumpSpeed;
			else
				moveV.y = 0;		
		}
	}
	else
		moveV.y -= gravity* Time.deltaTime;
		
	// Move the controller
	var lVelocity=Vector3(moveV.x * runSpeed,moveV.y,0);
	//print(lVelocity);
	var flags:CollisionFlags=characterController.Move(lVelocity* Time.deltaTime);
	grounded = (flags & CollisionFlags.CollidedBelow) != 0;
	
}

function setCommand(pActionCommand:UnitActionCommand)
{
	actionCommand=pActionCommand;
}

function getCommand()
{
	return actionCommand;
}