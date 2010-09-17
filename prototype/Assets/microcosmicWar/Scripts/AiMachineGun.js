

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
	
	function getFace():UnitFaceDirection
	{
		return face;
	}
	
	function getNowSprite():ZZSprite
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

class AiMachineGun extends DefenseTower
{

	var gunSprite:ZZSprite;

	var fireTime:float;


	//物体朝向
	//var face = 1;

	//protected var gunPivot:Transform;
	//protected var Xscale:float;

	var invert=_2dInvertDoubleFaceSprite();

	//在播放射击动画时,会执行的动作
	protected var actionImpDuringFireAnimation=AnimationImpInTimeList();


	function Start()
	{
		super.Start();
		invert.setAnimationListener("fire",actionImpDuringFireAnimation);
		
		actionImpDuringFireAnimation.setImpInfoList(
				[AnimationImpTimeListInfo(fireTime,EmitBullet)]
			);
			
	}


	virtual function initWhenHost()
	{
		var lIntType:int = invert.getFace();
		zzCreatorUtility.sendMessage(gameObject,"initFace", lIntType);
	}
	
	virtual function init(info:Hashtable)
	{
		//print(info["face"]);
		//print(invert);
		invert.face = info["face"];
		super.init(info);
	}

	@RPC
	function initFace(pFace:int)
	{
		invert.init(pFace);
		gunSprite=invert.getNowSprite();
	}
	
	virtual function setFaceDirection(pFace:int)
	{
		//print(gunSprite);
		gunSprite=invert.setFace(pFace);
	}


	virtual function getFaceDirection()
	{
		return invert.getFace();
		//return face;
	}

	virtual function Update () 
	{
		//print(gunSprite);
		if(inFiring())
			gunSprite.playAnimation("fire");
		else
			gunSprite.playAnimation("wait");
		super.Update();
		//impSmoothTurn(Time.deltaTime );
		//setAngle(nowAngular);
	}


}