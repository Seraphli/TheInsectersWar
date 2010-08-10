
var maxUpAngle=50.0;

var maxDownAngle=50.0;

var angularVelocity=20.0;

var nowAngular=0.0;

//ʣ��Ҫ�ƶ��ĽǶ�,����ƽ�����ƶ�
//var wantToTurn:float;


static var NULL_aimAngular=361.0;
//����һ�������õ���ֵ,��Ϊ������ʱ��ֵ
var aimAngular=NULL_aimAngular;

var gunPivot:Transform;

var gunSprite:ZZSprite;

var emitter:Emitter;

var fire=false;

var fireTime:float;

class _2dInvertDoubleFaceSprite
{
	var face=1;
	
	var leftFaceValue=1;
	
	var invertObject:Transform;
	
	var turnObject:Transform;

	var faceLeftSprite:ZZSprite;

	var faceRightSprite:ZZSprite;
	
	protected var invertObjectXscale:float;
	
	function setFace(pFace:int)
	{
		//invertObjectXscale=invertObject.localScale.x;
		face=pFace;
		UpdateFaceShow();
	}
	
	function UpdateFaceShow()
	{
		//invertObject.localScale.x=face*invertObjectXscale;
		
		if(face==1)
			turnObject.rotation=Quaternion(0,0,0,1);
		else
			turnObject.rotation=Quaternion(0,1,0,0);
			
		if(face==leftFaceValue)
			return faceLeftSprite;
		else
			return faceRightSprite;
	}
}

//���峯��
//var face = 1;

//protected var gunPivot:Transform;
//protected var Xscale:float;

var invert=_2dInvertDoubleFaceSprite();

//�ڲ����������ʱ,��ִ�еĶ���
protected var actionImpDuringFireAnimation=AnimationImpInTimeList();

function setFire(pNeedFire:boolean)
{
	fire=pNeedFire;
}

protected var life:Life;

function Start()
{
	life= GetComponentInChildren(Life);
	life.setDieCallback(deadAction);
	
	gunPivot = transform.Find("turn/gunPivot");
	//Xscale=transform.localScale.x;
	
	//actionImpDuringFireAnimation.addImp(fireTime,EmitBullet);
	for(var i:Transform in transform.Find("shape"))
	{
		i.gameObject.layer=gameObject.layer;
	}
	collisionLayer.addCollider(gameObject);
	//transform.Find("turn/enemyDetector").gameObject.layer=gameObject.layer;
	
	actionImpDuringFireAnimation.setImpInfoList(
			[AnimationImpTimeListInfo(fireTime,EmitBullet)]
		);
	gunSprite=invert.UpdateFaceShow();
	gunSprite.setListener("fire",actionImpDuringFireAnimation);
	emitter.setBulletLayer( getBulletLayer() );
	
	maxDownAngle=-maxDownAngle;
	
	//UpdateFaceShow();
}

function deadAction()
{
	zzCreatorUtility.Destroy(gameObject);
}

function getBulletLayer()
{
	//�ӵ����ڲ�����Ϊ:��������+Bullet
	return LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" );
}

function Update () 
{
	if(fire)
		gunSprite.playAnimation("fire");
	else
		gunSprite.playAnimation("wait");
	impSmoothTurn(Time.deltaTime );
	setAngle(nowAngular);
}

function EmitBullet()
{
	//print("EmitBullet");
	emitter.EmitBullet();
}

//�ԽǶ�ת��
//function smoothTurn()
//{
//}

protected function impSmoothTurn(pElapseTime:float)
{
	if(aimAngular!=NULL_aimAngular)
	{
		var lRemainAngular=aimAngular-nowAngular;
		var lRemainAngularAbs=Mathf.Abs (lRemainAngular);
		var lTurnAngular=angularVelocity*pElapseTime;
		if(lTurnAngular<lRemainAngularAbs)
			nowAngular+=lTurnAngular*(lRemainAngular/lRemainAngularAbs);
		else
		{
			//����Ŀ��λ��,��ֹͣת��
			nowAngular+=lRemainAngular;
			aimAngular=NULL_aimAngular;
		}
	}
}

//��ת��ת���˽Ƕ�
function smoothTurnToAngle(pAimAngular:float)
{
	if(pAimAngular==NULL_aimAngular)
		return;
		
	if(pAimAngular>maxUpAngle)
	{
		aimAngular=maxUpAngle;
	}
	else if(pAimAngular<maxDownAngle)
	{
		aimAngular=maxDownAngle;
	}
	else
		aimAngular=pAimAngular;
}

//��׼��ָ��λ��,�������ǹ�ڵ�deviation�Ƕȷ�Χ��,�򲻶�
function takeAim(pAimPos:Vector3,deviation:float)
{
	var lFireRay = emitter.getFireRay();
	var lEmitterToAim=pAimPos-lFireRay.origin;
	lEmitterToAim.Normalize();
	
	var lAngle = Vector3.Angle(lFireRay.direction, lEmitterToAim);
	
	if(lAngle>deviation)
	{
	/*
		print(pAimPos);
		print(lFireRay);
		print(lEmitterToAim);
		print(lAngle);
	*/
		if(lEmitterToAim.y>lFireRay.direction.y)
		{
			//print("lEmitterToAim.y>lFireRay.direction.y");
			smoothTurnToAngle(nowAngular+lAngle);
		}
		else
		{
			//print("lEmitterToAim.y<=lFireRay.direction.y");
			smoothTurnToAngle(nowAngular-lAngle);
		}
	}
	else
		smoothTurnToAngle(NULL_aimAngular);
}

function setTowards()
{
}

function setAngle(pAngle:float)
{
	gunPivot.localEulerAngles=Vector3(0, 0, pAngle);
}

function setFace()
{
}


function getFaceDirection()
{
	//return face;
}


//�Ƕ�����תΪ��
function getAngle()
{
	return nowAngular;
}

function getTowards()
{
}