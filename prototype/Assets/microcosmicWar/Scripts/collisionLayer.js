
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

static function initialize()
{
	for(var i=0; i<32; i+=1)
	{
		mLayersIgnoreList[i]=Array();
		mLayersObjectList[i]=Array();
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
	if(gameObject.collider || gameObject.layer<8 )
	{
		var ignoreList:Array = mLayersIgnoreList[gameObject.layer];
		for(var i:int in ignoreList )
		{
			//print("(var i:int in ignoreList )"+gameObject.name);
			var objectList:Array=mLayersObjectList[i];
			for( var a: int = objectList.length - 1; a >= 0; a-=1 )
			{
				//print("( var a: int = objectList.length - 1; a >= 0; a-=1 )");
				if (objectList[a].active)
				{
					print("Physics.IgnoreCollision "+gameObject.name+" "+objectList[a].name);
					Physics.IgnoreCollision(gameObject.collider, objectList[a].collider);
				}
				else
				{
					print("objectList.RemoveAt(a)");
					objectList.RemoveAt(a);
				}
			}
		}
		mLayersObjectList[gameObject.layer].Add(gameObject);

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
