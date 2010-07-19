
class layers
{
	public static var pismire = LayerMask.NameToLayer("pismire");
	public static var pismireValue = 1 << pismire;
	public static var bee = LayerMask.NameToLayer("bee");
	public static var beeValue = 1 << bee;
	public static var pismireBullet = LayerMask.NameToLayer("pismireBullet");
	public static var pismireBulletValue = 1 << pismireBullet;
	public static var beeBullet = LayerMask.NameToLayer("beeBullet");
	public static var beeBulletValue = 1 << beeBullet;
}

//static var mIgnoreList = new Hashtable();

static var mLayersIgnoreList = new Array[32];
static var mLayersObjectList = new Array[32];

//离清理还差的步数
protected static var nextStepToClear=5;

//清理 间隔步数
protected static var intervalStepToClear =5;

static function initialize()
{
	for(var i=0; i<32; i+=1)
	{
		mLayersIgnoreList[i]=Array();
		mLayersObjectList[i]=Array();
	}
}

//检测某层中的物体是否可用,否则消除
static function checkAndClear(objectList:Array)
{
	//print("checkAndClear before"+objectList.length);
	for( var a: int = objectList.length - 1; a >= 0; a-=1 )
	{
		//遍历此层物体
		//print("( var a: int = objectList.length - 1; a >= 0; a-=1 )");
		if ( !objectList[a] || !objectList[a].active )
		{
			//objectList[a]=objectList.Pop();
			zzUtilities.quickRemoveArrayElement(objectList,a);
			//print(objectList[a].name);
			//Debug.LogWarning("checkAndClear(a)@@@@@@@@@@@@@@@@"+objectList.length+"  a:"+a);
		}
	}
	//print("checkAndClear end"+objectList.length);
}

static function checkAndClearStep(objectList:Array)
{
	//	print("checkAndClearStep");
	nextStepToClear-=1;
	if(nextStepToClear<0)
	{
		//print("nextStepToClear<0");
		checkAndClear(objectList);
		nextStepToClear=intervalStepToClear;
	}
}

static function IgnoreCollisionBetween(layer1Num:int, layer2Num:int)
{
	//mIgnoreList[1 << layer1Num & 1 << layer2Num] = true;
	mLayersIgnoreList[layer1Num].Add(layer2Num);
	mLayersIgnoreList[layer2Num].Add(layer1Num);
}

static function addCollider( gameObject:GameObject)
{
	if(gameObject.collider && gameObject.layer>7 )
	{
		var ignoreList:Array = mLayersIgnoreList[gameObject.layer];
		for(var i:int in ignoreList )
		{
			//遍历忽略的层
			//print("(var i:int in ignoreList )"+gameObject.name);
			var objectList:Array=mLayersObjectList[i];
			for( var a: int = objectList.length - 1; a >= 0; a-=1 )
			{
				//遍历此层物体
				//print("( var a: int = objectList.length - 1; a >= 0; a-=1 )");
				if ( objectList[a] && objectList[a].active )
				{
					//print("Physics.IgnoreCollision "+gameObject.name+" "+objectList[a].name);
					Physics.IgnoreCollision(gameObject.collider, objectList[a].collider);
				}
				else
				{
					//Debug.LogWarning("objectList.RemoveAt(a)@@@@@@@@@@@@@@@@");
					//objectList.RemoveAt(a);
					//objectList[a]=objectList.Pop();
					zzUtilities.quickRemoveArrayElement(objectList,a);
				}
			}
		}
		//print("addCollider :"+gameObject.name);
		//print(LayerMask.LayerToName(gameObject.layer)+mLayersObjectList[gameObject.layer].length);
		checkAndClearStep(mLayersObjectList[gameObject.layer]);
		//print(LayerMask.LayerToName(gameObject.layer)+mLayersObjectList[gameObject.layer].length);
		mLayersObjectList[gameObject.layer].Add(gameObject);
		//print(LayerMask.LayerToName(gameObject.layer)+mLayersObjectList[gameObject.layer].length);

	}
	
	//遍历子物体
	for (var  child : Transform in gameObject.transform) 
	{
		//print(child.name);
		addCollider(child.gameObject);
	}
}

// Use this for initialization
function Start () {
	//print("Start");

}

// Use this for initialization
function Awake () {
	//print("Awake");
	initialize();
	IgnoreCollisionBetween(layers.pismire,layers.pismireBullet);
	IgnoreCollisionBetween(layers.bee,layers.beeBullet);
}
