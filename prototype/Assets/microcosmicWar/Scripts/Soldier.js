
var character=zzCharacter();

//var actionCommand:UnitActionCommand;

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
//protected var characterController:CharacterController;

private var moveV = Vector3.zero;
private var grounded : boolean = false;

private var life:Life;

var actionCommandControl:ActionCommandControl;

//��ɫ�ĳ���
//protected var face = -1;
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

function getActionCommandControl()
{
	return actionCommandControl;
}

function getCharacter()
{
	return character;
}

function getFaceDirection()
{
	return actionCommandControl.getFaceValue();
}

function Start()
{
	//����Ȩ
	//userControl=userControl&&zzCreatorUtility.isMine(networkView);
	//userControl=userControl&&zzCreatorUtility.isHost();

	mZZSprite = GetComponentInChildren(ZZSprite);
	//characterController = GetComponentInChildren(CharacterController);
	character.characterController = GetComponentInChildren(CharacterController);
	
	emitter = GetComponentInChildren(Emitter);
	life= GetComponentInChildren(Life);
	
	if(!actionCommandControl)
		actionCommandControl= GetComponentInChildren(ActionCommandControl);
	//life.setDieCallback(deadAction);
	life.addDieCallback(deadAction);
	
	//?
	//characterController .detectCollisions=false;
	
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
	//return LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" );
	return LayerMask.NameToLayer( LayerMask.LayerToName(transform.Find("CubeReact").gameObject.layer)+"Bullet" );
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
	
	/*//����Ӣ��
	var lInjureInfo:Hashtable = life.getInjureInfo();
	if(lInjureInfo && lInjureInfo.ContainsKey("bagControl"))
	{
		var lBagControl:zzItemBagControl = lInjureInfo["bagControl"];
		lBagControl.addMoney(shootAward);
	}*/
}

function disappear()
{
	//zzCreatorUtility.Destroy(gameObject);
	Destroy(gameObject);
}

function UpdateFaceShow()
{
	var lFace:int = actionCommandControl.getFaceValue();
	//Xscale=|reverseObjectTransform.localScale.x|,ʡȥ�ж�����
	reverseObjectTransform.localScale.x=lFace*Xscale;
	//moveV.x=lMove;
	if(lFace==1)
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

	if(actionCommandControl.updateFace())
		UpdateFaceShow();
		
	var lActionCommand = actionCommandControl.getCommand();
	
		//���ö��� ����
	if(lActionCommand.Fire)
	{
		mZZSprite.playAnimation("fire");
	}
	else
	{
		if(lActionCommand.GoForward)
		{
			mZZSprite.playAnimation("run");
			//moveV.x=face;
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
	character.update(actionCommandControl.getCommand(),actionCommandControl.getFaceValue(),life.isAlive());
/*	

	var lActionCommand = actionCommandControl.getCommand();
	if(life.isAlive() && grounded)
	{
		if( !lActionCommand.FaceDown)
		{
			if(lActionCommand.Jump)
				moveV.y = jumpSpeed;
			else
				moveV.y = -0.1;		
		}
	}
	else
		moveV.y -= gravity* Time.deltaTime;//������ moveV.y �Ͳ���������
		
	// Move the controller
	var flags = characterController.Move(Vector3(moveV.x * runSpeed,moveV.y,0)* Time.deltaTime);
	grounded = (flags & CollisionFlags.CollidedBelow) != 0;
	//if(userControl || clearCommandEveryFrame)
	//	actionCommand.clear();
	*/
}
