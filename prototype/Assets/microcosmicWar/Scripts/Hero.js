
var runSpeed=2.0;
var userControl=false;
//var controlByOther=false;
var gravity = 10.0;
var jumpSpeed = 8.0;

var actionCommand:UnitActionCommand;

var emitter:Emitter;

//����ڶ����Ķ���,Ҫ����С������
var fireTimeList:float[];

//��������Сʱ��ʱ��
var deadDisappearTimePos=4.0;

//var fireSound:AudioSource;

//�ڲ�����������ʱ,��ִ�еĶ���
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

var objectListener:IobjectListener;

//Component.SendMessage ("dieCallFunction")
//var dieCallFunction:Component;

//var upBodyAction

//��ɫ�ĳ���
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
	//life.setDieCallback(deadAction);
	life.addDieCallback(deadAction);
	
	//?
	//characterController .detectCollisions=false;
	
	collisionLayer.addCollider(gameObject);
	
	reverseObjectTransform= transform;

	//Xscale=transform.localScale.x;

	Xscale=reverseObjectTransform.localScale.x;
	
	//}
	//�����ĺ�Ķ���
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
	//�ӵ����ڲ�����Ϊ:��������+Bullet
	return LayerMask.NameToLayer( LayerMask.LayerToName(transform.Find("CubeReact").gameObject.layer)+"Bullet" );
}

function EmitBullet()
{
	//print("EmitBullet");
	emitter.EmitBullet();
}

//�������Ļص���ʹ��
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
	if(Input.GetButton ("up"))
		lActionCommand.FaceUp=true;
		
	lActionCommand.Fire=Input.GetButton ("fire");
	lActionCommand.Jump=Input.GetButton ("jump");
	return lActionCommand;
}

function UpdateFaceShow()
{
	
	//Xscale=|reverseObjectTransform.localScale.x|,ʡȥ�ж�����
	reverseObjectTransform.localScale.x=face*Xscale;
	//moveV.x=lMove;
}

//���¶���
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


		//���ý�ɫ����
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
	
		//���ö��� ����
	if(actionCommand.Fire)
	{
		upBodyAction.playAction("fire");
	}
	else
		upBodyAction.playAction("standby");
		
	if(characterController.isGrounded)
		if(actionCommand.GoForward)
		{
			downBodyAction.playAction("run");
		}
		else
		{
			downBodyAction.playAction("stand");
		}
	else
		downBodyAction.playAction("onAir");

	if(actionCommand.GoForward)
		moveV.x=face;
	
	if(actionCommand.Jump&&actionCommand.FaceDown)
	{
		boardDetector.down();
	}
	else
		boardDetector.recover();
		
	updatePosture(actionCommand.FaceUp,actionCommand.FaceDown,actionCommand.GoForward);
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

var preIsGrounded = true;

//function isGrounded()
//{
//	return preIsGrounded==true || characterController.isGrounded==true;
//}

//����characterController
function FixedUpdate()
{

	
	if(life.isAlive() && characterController.isGrounded)
	{
		if( !actionCommand.FaceDown)
		{
			if(actionCommand.Jump)
				moveV.y = jumpSpeed;
			else
				moveV.y = -0.1;	//���������
		}
	}
	else
		moveV.y -= gravity* Time.deltaTime;
		
	// Move the controller
	var lVelocity=Vector3(moveV.x * runSpeed,moveV.y,0);
	//print(lVelocity);
	preIsGrounded = characterController.isGrounded;
	var flags:CollisionFlags=characterController.Move(lVelocity* Time.deltaTime);
	//grounded = (flags & CollisionFlags.CollidedBelow) != 0;
	//print(""+preIsGrounded+characterController.isGrounded+":"+isGrounded());
	//print(grounded);
	//print(characterController.isGrounded );
	
}

function setCommand(pActionCommand:UnitActionCommand)
{
	actionCommand=pActionCommand;
}

function getCommand()
{
	return actionCommand;
}