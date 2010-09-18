
@script RequireComponent(destroyWhenDie)

var runSpeed=2.0;
//var userControl=false;
//var controlByOther=false;
var gravity = 10.0;
var jumpSpeed = 8.0;
var character=zzCharacter();

var emitter:Emitter;

//被打死后小时的时间
var deadDisappearTimePos=4.0;

//var fireSound:AudioSource;

//在播放死亡动画时,会执行的动作
protected var actionImpDuringDeadAnimation=AnimationImpInTimeList();

//protected var turnObjectTransform:Transform;
var reverseObjectTransform:Transform;

protected var Xscale:float;
//protected var mZZSprite:ZZSprite;
//protected var characterController:CharacterController;

//private var moveV = Vector3.zero;
private var grounded : boolean = false;

private var life:Life;

private var myAnimation:Animation;

var boardDetector:BoardDetector;

var inFiring:boolean;

var upBodyActionInfo=BodyActionInfo();
var downBodyActionInfo=BodyActionInfo();


var upBodyAction=BodyAction();
var downBodyAction=BodyAction();

var objectListener:IobjectListener;


//原始的朝向
var originalFace= UnitFaceDirection.left;

//Component.SendMessage ("dieCallFunction")
//var dieCallFunction:Component;

//var upBodyAction

//角色的朝向
//protected var face = -1;
var actionCommandControl:ActionCommandControl;
/*
function getVelocity()
{
	return moveV;
}

function setVelocity(pVelocity:Vector3)
{
	moveV=pVelocity;
}
*/

function getCharacter()
{
	return character;
}

function getFaceDirection()
{
	return actionCommandControl.getFaceValue();
}

function getFace()
{
	return actionCommandControl.getFace();
}

//var upBody:Transform;

function Start()
{

	if(!myAnimation)
		myAnimation = GetComponentInChildren(Animation);
	if(!actionCommandControl)
		actionCommandControl = GetComponent(ActionCommandControl);
		
	upBodyAction.init(upBodyActionInfo,myAnimation);
	downBodyAction.init(downBodyActionInfo,myAnimation);

	//characterController = GetComponentInChildren(CharacterController);
	character.characterController = GetComponentInChildren(CharacterController);
	/*
	character.runSpeed=runSpeed;
	character.gravity = gravity;
	character.jumpSpeed = jumpSpeed;
	*/
	//characterController.Move(Vector3(0,0,0));
	emitter = GetComponentInChildren(Emitter);
	

	life= GetComponentInChildren(Life);
	//life.setDieCallback(deadAction);
	life.addDieCallback(deadAction);
	
	//?
	//characterController .detectCollisions=false;
	
	collisionLayer.addCollider(gameObject);
	
	if(!reverseObjectTransform)
		reverseObjectTransform= transform;

	//Xscale=transform.localScale.x;

	Xscale=Mathf.Abs( reverseObjectTransform.localScale.x );
	
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
	if(objectListener)
		objectListener.removedCall();
	//Destroy(gameObject);
}

function UpdateFaceShow()
{
	
	//Xscale=|reverseObjectTransform.localScale.x|,省去判断正负
	//reverseObjectTransform.localScale.x=face*Xscale;
	//reverseObjectTransform.localScale.x=actionCommandControl.getFaceValue()*Xscale;
	//moveV.x=lMove;
	
	if( originalFace == actionCommandControl.getFace() )
		reverseObjectTransform.localScale.x=Xscale;
	else
		reverseObjectTransform.localScale.x=-Xscale;
}

//更新动画
function Update() 
{	

	//moveV.x=0;
	//moveV.z=0;
	var lActionCommand;
	
	if( life.isDead() )
	{
		return;
	}
	
	if(actionCommandControl.updateFace())
		UpdateFaceShow();
	
	lActionCommand = actionCommandControl.getCommand();
		//设置动画 动作
	if(lActionCommand.Fire)
	{
		upBodyAction.playAction("fire");
	}
	else
		upBodyAction.playAction("standby");
		
	if(character.isGrounded())
		if(lActionCommand.GoForward)
		{
			downBodyAction.playAction("run");
		}
		else
		{
			downBodyAction.playAction("stand");
		}
	else
		downBodyAction.playAction("onAir");

	
	if(lActionCommand.Jump&&lActionCommand.FaceDown)
	{
		boardDetector.down();
	}
	else
		boardDetector.recover();
		
	updatePosture(lActionCommand.FaceUp,lActionCommand.FaceDown,lActionCommand.GoForward);
	//print(""+actionCommand.FaceUp+actionCommand.FaceDown+actionCommand.GoForward);
		
}

function updatePosture(pUp:boolean,pDwon:boolean,pForward:boolean)
{
	if(pUp==pDwon)
	{
		upBodyAction.playActionType("angle0");
		return;
	}
	
	if(pUp)
	{
		if(pForward)
			upBodyAction.playActionType("angle45");
		else
			upBodyAction.playActionType("angle90");
	}
	else
	{
		if(pForward)
			upBodyAction.playActionType("angle-45");
		else
			upBodyAction.playActionType("angle-90");
	}
}

//var preIsGrounded = true;

//function isGrounded()
//{
//	return preIsGrounded==true || characterController.isGrounded==true;
//}

//更新characterController
function FixedUpdate()
{
	character.update(actionCommandControl.getCommand(),actionCommandControl.getFaceValue(),life.isAlive());
/*
	var lActionCommand = actionCommandControl.getCommand();
	
	if(life.isAlive() && characterController.isGrounded)
	{
		if( !lActionCommand.FaceDown)
		{
			if(lActionCommand.Jump)
				moveV.y = jumpSpeed;
			else
				moveV.y = -0.1;	//以免飞起来
		}
	}
	else
		moveV.y -= gravity* Time.deltaTime;
		
	// Move the controller
	var lVelocity=Vector3(moveV.x * runSpeed,moveV.y,0);

	var flags:CollisionFlags=characterController.Move(lVelocity* Time.deltaTime);
*/
	
}
