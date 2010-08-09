
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

//�ڲ����������ʱ,��ִ�еĶ���
protected var actionImpDuringFireAnimation=AnimationImpInTimeList();
//�ڲ�����������ʱ,��ִ�еĶ���
protected var actionImpDuringDeadAnimation=AnimationImpInTimeList();

protected var turnObjectTransform:Transform;
protected var reverseObjectTransform:Transform;

protected var Xscale:float;
protected var mZZSprite:ZZSprite;
protected var characterController:CharacterController;

private var moveV = Vector3.zero;
private var grounded : boolean = false;

private var life:Life;

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

class UnitActionCommand
{
	//None,
	
	var FaceLeft=false;//������
	var FaceRight=false;//������
	var GoForward=false;//ǰ��
	var Fire=false;//����
	var Jump=false;//��Ծ
	
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
	//����Ȩ
	//userControl=userControl&&zzCreatorUtility.isMine(networkView);
	//userControl=userControl&&zzCreatorUtility.isHost();

	mZZSprite = GetComponentInChildren(ZZSprite);
	characterController = GetComponentInChildren(CharacterController);
	emitter = GetComponentInChildren(Emitter);
	life= GetComponentInChildren(Life);
	life.setDieCallback(deadAction);
	
	characterController .detectCollisions=false;
	
	collisionLayer.addCollider(gameObject);

	Xscale=transform.localScale.x;
	
	turnObjectTransform= transform.Find("turn").transform;
	reverseObjectTransform= transform.Find("reverse").transform;
	
	//actionImpDuringFireAnimation.ImpFunction=EmitBullet;
	
	
	//��Ϊhost ʱ,�������ӵ�
	//emitter.EmitBullet Ҳ���ж�,��ȥ��һ��
	//if( zzCreatorUtility.isHost())
	//{//Ϊ�˿ͻ�������Ч,���Զ�������
		//print("userControl || zzCreatorUtility.isHost()");
		//for(var e:AnimationImpTimeListInfo in actionImpDuringFireAnimation.getImpInfoList())
		//{
		//	e.ImpFunction=EmitBullet;
		//}
		//var infos=Array();
		for(var iTime:float in fireTimeList)
		{
			//var lEmitBulletImp=AnimationImpTimeListInfo();
			//lEmitBulletImp.ImpTime=iTime;
			//lEmitBulletImp.ImpFunction=EmitBullet;
			//infos.Add(lEmitBulletImp);
			actionImpDuringFireAnimation.addImp(iTime,EmitBullet);
			
			//var lEmitBulletSoundImp=AnimationImpTimeListInfo();
			//lEmitBulletSoundImp.ImpTime=iTime;
			//lEmitBulletSoundImp.ImpFunction=EmitBulletSound;
			//infos.Add(lEmitBulletSoundImp);
			//actionImpDuringFireAnimation.addImp(iTime,EmitBulletSound);
		}
		//var lTemp:AnimationImpTimeListInfo[] = infos.ToBuiltin( AnimationImpTimeListInfo );
		//actionImpDuringFireAnimation.setImpInfoList(lTemp);
		//actionImpDuringFireAnimation.ImpFunction=EmitBullet;
		actionImpDuringFireAnimation.endAddImp();
		mZZSprite.setListener("fire",actionImpDuringFireAnimation);
	//}
	//�����ĺ�Ķ���
	//{
		//var lDeadInfos=Array();
		//var lDeadImp=AnimationImpTimeListInfo();
		//lDeadImp.ImpTime=deadDisappearTimePos;
		//lDeadImp.ImpFunction=disappear;
		//lDeadInfos.Add(lDeadImp);
		actionImpDuringDeadAnimation.setImpInfoList(
				[AnimationImpTimeListInfo(deadDisappearTimePos,disappear)]
			);
		mZZSprite.setListener("dead",actionImpDuringDeadAnimation);
	//}
			
	emitter.setBulletLayer( getBulletLayer() );
	UpdateFaceShow();
}

function getBulletLayer()
{
	//�ӵ����ڲ�����Ϊ:��������+Bullet
	return LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" );
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
	mZZSprite.playAnimation("dead");
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
	if(face==1)
		turnObjectTransform.rotation=Quaternion(0,0,0,1);
	else
		turnObjectTransform.rotation=Quaternion(0,1,0,0);
}

//���¶���
function Update() 
{	
	moveV.x=0;
	moveV.z=0;
	if( life.isDead() )
	{
		//mZZSprite.playAnimation("dead");
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

//����characterController
function FixedUpdate()
{
	
	//if (characterController.isGrounded && actionCommand.Jump)
	if ( life.isAlive() && grounded && actionCommand.Jump)
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