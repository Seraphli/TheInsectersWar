
//是否被限制转角,否则可以任意旋转,为true时, maxUpAngle , maxDownAngle 才有用
var limitedAngle = true;

var maxUpAngle=50.0;

var maxDownAngle=50.0;

var angularVelocity=20.0;

//会在update中同步到物体,以原始角度为0度
var nowAngular=0.0;

//剩余要移动的角度,用于平滑的移动
//var wantToTurn:float;


static var NULL_aimAngular=1000.0;
//设置一个不会用到的值,作为不是用时的值
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
	//探测器 扳机 的设置在UI脚本中
	
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
	//子弹所在层名字为:种族名字+Bullet
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

//以角度转动
//function smoothTurn()
//{
//}

protected function impSmoothTurn(pElapseTime:float)
{
	if(aimAngular!=NULL_aimAngular)
	{
		var lRemainAngular=aimAngular-nowAngular;
		//print("b:"+lRemainAngular);
		//转过一周后的处理办法,虽然和目标角度离得很近,但是值却差很多
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
			//到达目标位置,并停止转动
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

//以转速转到此角度
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

//瞄准到指定位置,如果已在枪口的deviation角度范围内,则不动
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
		
		//因为射击口射线不一定穿过转轴,所以采取 nowAngular+/-lAngle的方式 设置角度
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


//角度向上转为正
function getAngle()
{
	return nowAngular;
}

function getTowards()
{
}