
//�Ƿ�����ת��,�������������ת,Ϊtrueʱ, maxUpAngle , maxDownAngle ������
var limitedAngle = true;

var maxUpAngle=50.0;

var maxDownAngle=50.0;

var angularVelocity=20.0;

//����update��ͬ��������,��ԭʼ�Ƕ�Ϊ0��
var nowAngular=0.0;

//ʣ��Ҫ�ƶ��ĽǶ�,����ƽ�����ƶ�
//var wantToTurn:float;


static var NULL_aimAngular=1000.0;
//����һ�������õ���ֵ,��Ϊ������ʱ��ֵ
var aimAngular=NULL_aimAngular;

var gunPivot:Transform;

var emitter:Emitter;

var fire=false;

function setFire(pNeedFire:boolean)
{
	fire=pNeedFire;
}

function inFiring()
{
	return fire;
}

protected var life:Life;

function Start()
{
	if(!limitedAngle)
	{
		maxUpAngle = 360;
		maxDownAngle = 360;
	}
	
	life= GetComponentInChildren(Life);
	//life.setDieCallback(deadAction);
	life.addDieCallback(deadAction);
	
	if(!gunPivot)
		gunPivot = transform.Find("turn/gunPivot");
	
	maxDownAngle=-maxDownAngle;
	
	
	if(zzCreatorUtility.isHost())
	{
		zzCreatorUtility.sendMessage(gameObject,"initLayer",gameObject.layer);
		//var lIntType:int = invert.getFace();
		//zzCreatorUtility.sendMessage(gameObject,"initFace", lIntType);
		initWhenHost();
	}
}

virtual function initWhenHost()
{
}

/*
info["face"]
info["layer"]
info["adversaryLayer"]
*/
virtual function init(info:Hashtable)
{
	//print(info["face"]);
	//print(invert);
	//invert.face = info["face"];
	gameObject.layer=info["layer"];
	var lAi:AiMachineGunAI = GetComponentInChildren(AiMachineGunAI);

	if(zzCreatorUtility.isHost())
		lAi.adversaryLayer=info["adversaryLayer"];
}

@RPC
function initLayer(pLayer:int)
{
	//̽���� ��� ��������UI�ű���
	
	gameObject.layer=pLayer;
	for(var i:Transform in transform.Find("shape"))
	{
		i.gameObject.layer=pLayer;
	}
	collisionLayer.addCollider(gameObject);
	emitter.setBulletLayer( getBulletLayer() );
}

function deadAction()
{
	//zzCreatorUtility.Destroy(gameObject);
	Destroy(gameObject);
}

function getBulletLayer()
{
	//print( LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" ));
	//�ӵ����ڲ�����Ϊ:��������+Bullet
	return LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" );
}

virtual function Update () 
{
	//print(gunSprite);
	//if(fire)
	//	gunSprite.playAnimation("fire");
	//else
	//	gunSprite.playAnimation("wait");
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
		//print("b:"+lRemainAngular);
		//ת��һ�ܺ�Ĵ���취,��Ȼ��Ŀ��Ƕ���úܽ�,����ֵȴ��ܶ�
		if(Mathf.Abs (lRemainAngular)>180)
			lRemainAngular = lRemainAngular-zzUtilities.normalize(lRemainAngular)*360;
		//print("a:"+lRemainAngular);
		var lRemainAngularAbs=Mathf.Abs (lRemainAngular);
		var lTurnAngular=angularVelocity*pElapseTime;
		//print("lTurnAngular:"+lTurnAngular+"lRemainAngularAbs:"+lRemainAngularAbs);
		if(lTurnAngular<lRemainAngularAbs)
			nowAngular+=lTurnAngular*(lRemainAngular/lRemainAngularAbs);
		else
		{
			//����Ŀ��λ��,��ֹͣת��
			nowAngular+=lRemainAngular;
			//print("final:"+nowAngular);
			aimAngular=NULL_aimAngular;
		}
		
		nowAngular = nowAngular%360;
	}
}

function _getSmoothAngle()
{
	return aimAngular;
}

function _setSmoothAngle(pAimAngular:float)
{
	aimAngular=pAimAngular;
}

//��ת��ת���˽Ƕ�
function smoothTurnToAngle(pAimAngular:float)
{
	//print("smoothTurnToAngle"+pAimAngular);
	if(pAimAngular==NULL_aimAngular)
		return;
	pAimAngular = pAimAngular%360.0;
		
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
	//print(pAimPos);
	var lFireRay = emitter.getFireRay();
	var lEmitterToAim=pAimPos-lFireRay.origin;
	lEmitterToAim.Normalize();
	
	var lAngle:float = Vector3.Angle(lFireRay.direction, lEmitterToAim);
	
	if(lAngle>deviation)
	{
		var lCross:Vector3 = Vector3.Cross(lFireRay.direction,lEmitterToAim);
	/*
		print("pAimPos:"+pAimPos);
		print("lFireRay:"+lFireRay);
		print("lEmitterToAim:"+lEmitterToAim);
		print("lAngle:"+lAngle);
		print("deviation:"+deviation);
	*/
		//if(lEmitterToAim.y>lFireRay.direction.y)
		
		//��Ϊ��������߲�һ������ת��,���Բ�ȡ nowAngular+/-lAngle�ķ�ʽ ���ýǶ�
		if(lCross.z>0)
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

//function getAngle()
//{
//	return gunPivot.localEulerAngles.z;
//}

virtual function setFaceDirection(pFace:int)
{
	//print(gunSprite);
	//gunSprite=invert.setFace(pFace);
}


virtual function getFaceDirection():UnitFaceDirection
{
	//return invert.getFace();
	//return face;
	return UnitFaceDirection.left;
}


//�Ƕ�����תΪ��
function getAngle()
{
	return nowAngular;
}

function getTowards()
{
}