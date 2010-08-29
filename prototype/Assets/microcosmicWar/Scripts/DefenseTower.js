
var maxUpAngle=50.0;

var maxDownAngle=50.0;

var angularVelocity=20.0;

//会在update中同步到物体
var nowAngular=0.0;

//剩余要移动的角度,用于平滑的移动
//var wantToTurn:float;


static var NULL_aimAngular=361.0;
//设置一个不会用到的值,作为不是用时的值
var aimAngular=NULL_aimAngular;

var gunPivot:Transform;

var gunSprite:ZZSprite;

var emitter:Emitter;

var fire=false;

var fireTime:float;

class _2dInvertDoubleFaceSprite
{
	var face=UnitFaceDirection.left;
	
	//var preFace=1;
	
	//var leftFaceValue=1;
	
	//原始的朝向
	var originalFace= UnitFaceDirection.left;
	
	//var invertObject:Transform;
	
	var turnObject:Transform;

	var faceLeftSprite:ZZSprite;

	var faceRightSprite:ZZSprite;
	
	var nowSprite:ZZSprite;
	
	function _2dInvertDoubleFaceSprite()
	{
		//preFace=face;
		//_UpdateFaceShow();
	}
	
	function init(pFace:int)
	{
		face=pFace;
		_UpdateFaceShow();
	}
	
	protected var invertObjectXscale:float;
	
	function setFace(pFace:int)
	{
		//invertObjectXscale=invertObject.localScale.x;
		UpdateFaceShow(pFace);
		face=pFace;
		return nowSprite;
	}
	
	function getFace()
	{
		return face;
	}
	
	function getNowSprite()
	{
		return nowSprite;
	}
	
	//以右为正
	function setFaceDirection(pFace:int)
	{
		//setFace(pFace*leftFaceValue);
		setFace(pFace);
	}
	
	function setAnimationListener(pAnimationName:String,pListener:AnimationListener)
	{
		faceLeftSprite.setListener(pAnimationName,pListener);
		faceRightSprite.setListener(pAnimationName,pListener);
	}
	
	function UpdateFaceShow(pFace:int)
	{
		if(face!=pFace)
		{
			face=pFace;
			_UpdateFaceShow();
		}
			
		return nowSprite;
	}
	
	protected function _UpdateFaceShow()
	{
		if(face==originalFace)
			turnObject.rotation=Quaternion(0,0,0,1);
		else
			turnObject.rotation=Quaternion(0,1,0,0);
			
		//if(face==leftFaceValue)
		if(face==UnitFaceDirection.right)
			nowSprite = faceRightSprite;
		else
			nowSprite = faceLeftSprite;
	}
}

//物体朝向
//var face = 1;

//protected var gunPivot:Transform;
//protected var Xscale:float;

var invert=_2dInvertDoubleFaceSprite();

//在播放射击动画时,会执行的动作
protected var actionImpDuringFireAnimation=AnimationImpInTimeList();

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
	life= GetComponentInChildren(Life);
	//life.setDieCallback(deadAction);
	life.addDieCallback(deadAction);
	
	gunPivot = transform.Find("turn/gunPivot");
	
	maxDownAngle=-maxDownAngle;
	
	//UpdateFaceShow();
	//gunSprite=invert.UpdateFaceShow();
	invert.setAnimationListener("fire",actionImpDuringFireAnimation);
	
	actionImpDuringFireAnimation.setImpInfoList(
			[AnimationImpTimeListInfo(fireTime,EmitBullet)]
		);
		
	
	if(zzCreatorUtility.isHost())
	{
		zzCreatorUtility.sendMessage(gameObject,"initLayer",gameObject.layer);
		var lIntType:int = invert.getFace();
		zzCreatorUtility.sendMessage(gameObject,"initFace", lIntType);
	}
}

/*
info["face"]
info["layer"]
info["adversaryLayer"]
*/
function init(info:Hashtable)
{
	//print(info["face"]);
	//print(invert);
	invert.face = info["face"];
	gameObject.layer=info["layer"];
	var lAi:AiMachineGunAI = GetComponentInChildren(AiMachineGunAI);

	if(zzCreatorUtility.isHost())
		lAi.adversaryLayer=info["adversaryLayer"];
}

@RPC
function initFace(pFace:int)
{
	invert.init(pFace);
	gunSprite=invert.getNowSprite();
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
	zzCreatorUtility.Destroy(gameObject);
}

function getBulletLayer()
{
	//print( LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" ));
	//子弹所在层名字为:种族名字+Bullet
	return LayerMask.NameToLayer( LayerMask.LayerToName(gameObject.layer)+"Bullet" );
}

function Update () 
{
	//print(gunSprite);
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

//以角度转动
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
			//到达目标位置,并停止转动
			nowAngular+=lRemainAngular;
			aimAngular=NULL_aimAngular;
		}
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

//瞄准到指定位置,如果已在枪口的deviation角度范围内,则不动
function takeAim(pAimPos:Vector3,deviation:float)
{
	//print(pAimPos);
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

//function getAngle()
//{
//	return gunPivot.localEulerAngles.z;
//}

function setFaceDirection(pFace:int)
{
	//print(gunSprite);
	gunSprite=invert.setFace(pFace);
}


function getFaceDirection()
{
	return invert.getFace();
	//return face;
}


//角度向上转为正
function getAngle()
{
	return nowAngular;
}

function getTowards()
{
}