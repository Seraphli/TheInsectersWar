
var detector:zzDetectorBase;
var maxRequired = 3;

var targetList:Transform[];
var emitter:Emitter;
//var defenseTower:DefenseTower;
var defenseTowerAim:DefenseTowerAim;
//protected 
var targetNum = 0;
//protected 
var targetNowIndex = 0;
//var timer:zzTimer;
var adversaryLayer:int;

var sphereAreaHarm:SphereAreaHarm;

function setAdversaryLayer(pLayer:int)
{
	adversaryLayer = pLayer;
}

function Start()
{
	targetList = new Transform[maxRequired];
	//timer = AddComponent(zzTimer);
	emitter.setInitBulletFunc(initBullet);
}

function resetAndSearchEnemy():int
{
	targetNowIndex = 0;
	var lHits:Collider[] = detector.detector(maxRequired,1<<adversaryLayer);
	targetNum = lHits.Length;
	//print("targetNum:"+targetNum);
	if(targetNum>0)
	{
		for(var i=0;i<targetNum;++i)
			targetList[i] = lHits[i].transform;
		
		//朝第一个目标转动
		//defenseTower.takeAim(targetList[0].position,fireDeviation);
		defenseTowerAim.setTarget(targetList[0]);
	}
	return targetNum;
}

//得到目标列表中的下一个目标,如果到尾则返回第一个
function getNextTagetIndex():int
{
	var lOut = targetNowIndex+1;
	if(lOut>=targetNum)
		return 0;
	return lOut;
}

function getTargetAndMove():Transform
{
	var targetIndex = getNextTagetIndex();
	if( targetNum>0 &&  targetList[targetIndex] )
	{
		targetNowIndex = targetIndex;
		return targetList[targetIndex] ;
	}
	if(resetAndSearchEnemy()>0)
		return targetList[0];
	return null;
}

function createSphereAreaHarm(pLife:Life)
{
	var lAreaHarm:GameObject = gameObject.Instantiate(sphereAreaHarm.gameObject,pLife.transform.position,pLife.transform.rotation);
	//print("createSphereAreaHarm:"+pLife.transform.position);
	//var lAreaHarm:GameObject = new GameObject("MissileSphereAreaHarm");
	
	//var lSphereAreaHarm:SphereAreaHarm = lAreaHarm.AddComponent(SphereAreaHarm);
	var lSphereAreaHarm:SphereAreaHarm = lAreaHarm.GetComponent(SphereAreaHarm);
	lSphereAreaHarm.setHarmLayerMask(1<<adversaryLayer);
}

function initBullet(pBullet:Bullet)
{
	var lTaget:Transform = getTargetAndMove();
	if(lTaget)
	{
		var lBulletFollowAI = pBullet.GetComponent(BulletFollowAI);
		lBulletFollowAI.setTarget(lTaget);
	}
	
	var lBulletLife:Life = pBullet.gameObject.GetComponent(Life);
	lBulletLife.addDieCallback(createSphereAreaHarm);
}