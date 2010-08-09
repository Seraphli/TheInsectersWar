
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

//�ڲ����������ʱ,��ִ�еĶ���
protected var actionImpDuringFireAnimation=AnimationImpInTimeList();

function setFire(pNeedFire:boolean)
{
	fire=pNeedFire;
}

function Start()
{
	//actionImpDuringFireAnimation.addImp(fireTime,EmitBullet);
	for(var i:Transform in transform.Find("shape"))
	{
		i.gameObject.layer=gameObject.layer;
	}
	transform.Find("enemyDetector").gameObject.layer=gameObject.layer;
	
	actionImpDuringFireAnimation.setImpInfoList(
			[AnimationImpTimeListInfo(fireTime,EmitBullet)]
		);
	gunSprite.setListener("fire",actionImpDuringFireAnimation);
	emitter.setBulletLayer( getBulletLayer() );
	
	maxDownAngle=-maxDownAngle;
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

function getFace()
{
}

//�Ƕ�����תΪ��
function getAngle()
{
	return nowAngular;
}

function getTowards()
{
}