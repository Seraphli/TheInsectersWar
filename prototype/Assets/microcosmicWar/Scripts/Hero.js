
var runSpeed=2.0;
var userControl=false;
//var controlByOther=false;
var gravity = 10.0;
var jumpSpeed = 8.0;

var actionCommand:UnitActionCommand;

var clearCommandEveryFrame=true;

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

var inFiring:boolean;

var upBodyActionInfo=BodyActionInfo();
var downBodyActionInfo=BodyActionInfo();


var upBodyAction=BodyAction();
var downBodyAction=BodyAction();

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
		/*
	myAnimation.AddClip(myAnimation["stand"].clip, "standby"); 
	
	//myAnimation["fire"].AddMixingTransform(transform.Find("root/Bone01/Bone02"));
	//myAnimation["standby"].AddMixingTransform(transform.Find("root/Bone01/Bone02"));
	myAnimation["fire"].AddMixingTransform(upBody);
	myAnimation["standby"].AddMixingTransform(upBody);
	
	 myAnimation["run"].layer = 1;
	 myAnimation["fire"].layer = 2;
	 
	myAnimation.CrossFade("stand");
	myAnimation.CrossFade("standby");
	
	var lFireAnimationEvent=AnimationEvent();
	lFireAnimationEvent.functionName="EmitBullet";
	lFireAnimationEvent.time=0;
	myAnimation["fire"].clip.AddEvent(lFireAnimationEvent);*/

	//animation.Play("fire");
	//animation.Play("run");

	//����Ȩ
	//userControl=userControl&&zzCreatorUtility.isMine(networkView);
	//userControl=userControl&&zzCreatorUtility.isHost();

	//mZZSprite = GetComponentInChildren(ZZSprite);
	characterController = GetComponentInChildren(CharacterController);
	//characterController.Move(Vector3(0,0,0));
	emitter = GetComponentInChildren(Emitter);
	
	//@@@@@@@@@@
	//life= GetComponentInChildren(Life);
	//life.setDieCallback(deadAction);
	
	//?
	characterController .detectCollisions=false;
	
	collisionLayer.addCollider(gameObject);
	
	//@@@@@@@@@@
	//reverseObjectTransform= transform.Find("model").transform;
	reverseObjectTransform= transform;
	//turnObjectTransform= transform.Find("model").transform;

	//Xscale=transform.localScale.x;

	Xscale=reverseObjectTransform.localScale.x;
	
	//}
	//�����ĺ�Ķ���
		//actionImpDuringDeadAnimation.setImpInfoList(
				//[AnimationImpTimeListInfo(deadDisappearTimePos,disappear)]
			//);
		//mZZSprite.setListener("dead",actionImpDuringDeadAnimation);
	//}
	
	//@@@@@@@@@@		
	//emitter.setBulletLayer( getBulletLayer() );
	UpdateFaceShow();
}

function getBulletLayer()
{
	//�ӵ����ڲ�����Ϊ:��������+Bullet
	return 1;
	//@@@@return LayerMask.NameToLayer( LayerMask.LayerToName(transform.Find("CubeReact").gameObject.layer)+"Bullet" );
}

function EmitBullet()
{
	//print("EmitBullet");
	emitter.EmitBullet();
}

//function EmitBulletSound()
//{
	//print("EmitBullet");
//	fireSound.Play();
//}

//�������Ļص���ʹ��
function deadAction()
{
	//mZZSprite.playAnimation("dead");
	gameObject.layer=layers.deadObject;
	transform.Find("CubeReact").gameObject.layer=layers.deadObject;
	
	collisionLayer.updateCollider(gameObject);
}

function disappear()
{
	//zzCreatorUtility.Destroy(gameObject);
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
	lActionCommand.Fire=Input.GetButton ("fire");
	lActionCommand.Jump=Input.GetButton ("jump");
	return lActionCommand;
}

function UpdateFaceShow()
{
	
	//Xscale=|reverseObjectTransform.localScale.x|,ʡȥ�ж�����
	reverseObjectTransform.localScale.x=face*Xscale;
	//moveV.x=lMove;
	
	
	//@@@@@@@@@@
	//if(face==1)
	//	turnObjectTransform.rotation=Quaternion(0,0,0,1);
	//else
	//	turnObjectTransform.rotation=Quaternion(0,1,0,0);
}

//var preCommand=UnitActionCommand();
/*
function playAnimation(pName:String)
{
	 if(!myAnimation.IsPlaying(pName) )
	 {
		print(myAnimation.IsPlaying(pName));
		print(pName);
		myAnimation.CrossFade(pName);
	}
}
*/
//���¶���
function Update() 
{	

	print("###########################");
	for(var i:AnimationState in myAnimation)
		Debug.Log(i.name+" "+i.weight+" layer:"+i.layer);
	print("###########################");
	moveV.x=0;
	moveV.z=0;
	//@@@@@@@@@@
	//if( life.isDead() )
	//{
	//	return;
	//}
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
		//myAnimation.CrossFade("fire");
		//impAnimation(preCommand.Fire,actionCommand.Fire,"fire");
		upBodyAction.playAction("fire");
		//mZZSprite.playAnimation("fire");
	}
	else
		upBodyAction.playAction("standby");
	//else
	//{
		if(actionCommand.GoForward)
		{
			//mZZSprite.playAnimation("run");
			//myAnimation.CrossFade("run");
			//impAnimation(preCommand.GoForward,actionCommand.GoForward,"run");
			//playAnimation("run");
			downBodyAction.playAction("run");
			moveV.x=face;
		}
		else
		{
			//myAnimation.CrossFade("stand");
			//impAnimation(preCommand.GoForward,actionCommand.GoForward,"stand");
			//playAnimation("stand");
			downBodyAction.playAction("stand");
			//mZZSprite.playAnimation("stand");
		}
		//preCommand=actionCommand;
		
}

//����characterController
function FixedUpdate()
{
	//if (characterController.isGrounded && actionCommand.Jump)
	//@@@@@@@@@@
	//if ( life.isAlive() && grounded && actionCommand.Jump)
	//{
		//moveV.y = jumpSpeed;
		//controller.animation
	//}
	if(grounded)
		moveV.y = 0;
	else
		moveV.y -= gravity* Time.deltaTime;
		
	// Move the controller
	var lVelocity=Vector3(moveV.x * runSpeed,moveV.y,0);
	//print(lVelocity);
	var flags:CollisionFlags=characterController.Move(lVelocity* Time.deltaTime);
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