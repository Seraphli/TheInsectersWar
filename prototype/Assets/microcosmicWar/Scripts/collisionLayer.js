
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
	
	public static var deadObject = LayerMask.NameToLayer("deadObject");
	
	public static var characterShape = LayerMask.NameToLayer("characterShape");
}

//static var mIgnoreList = new Hashtable();

static var mLayersIgnoreList = new Array[32];
static var mLayersObjectList = new Array[32];

//��������Ĳ���
protected static var nextStepToClear=5;

//���� �������
protected static var intervalStepToClear =5;

static function initialize()
{
	for(var i=0; i<32; i+=1)
	{
		mLayersIgnoreList[i]=Array();
		mLayersObjectList[i]=Array();
	}
}

//���ĳ���е������Ƿ����,��������
protected static function checkAndClear(objectList:Array)
{
	//print("checkAndClear before"+objectList.length);
	for( var a: int = objectList.length - 1; a >= 0; a-=1 )
	{
		//�����˲�����
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

protected static function checkAndClearStep(objectList:Array)
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
	if(layer1Num==layer2Num)
	{
		mLayersIgnoreList[layer1Num].Add(layer1Num);
		return;
	}
	mLayersIgnoreList[layer1Num].Add(layer2Num);
	mLayersIgnoreList[layer2Num].Add(layer1Num);
}

//Ŀǰֻ������deadObject
static function updateCollider( gameObject:GameObject)
{
	//print("@@@@@@@@@@@@@@@@  updateCollider");
	addCollider(gameObject);
}

static function addCollider( gameObject:GameObject)
{
	if(gameObject.collider && gameObject.layer>7 )
	{
		//print("addCollider :"+gameObject.name);
		var ignoreList:Array = mLayersIgnoreList[gameObject.layer];
		//print(ignoreList);
		for(var i:int in ignoreList )
		{
			//�������ԵĲ�
			//print("(var i:int in ignoreList )"+gameObject.name);
			var objectList:Array=mLayersObjectList[i];
			for( var a: int = objectList.length - 1; a >= 0; a-=1 )
			{
				//�����˲�����
				//print("( var a: int = objectList.length - 1; a >= 0; a-=1 )");
				var aGameObject:GameObject=objectList[a];
				if ( aGameObject && aGameObject.active && aGameObject!=gameObject)
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
		//print(LayerMask.LayerToName(gameObject.layer)+mLayersObjectList[gameObject.layer].length);
		checkAndClearStep(mLayersObjectList[gameObject.layer]);
		//print(LayerMask.LayerToName(gameObject.layer)+mLayersObjectList[gameObject.layer].length);
		mLayersObjectList[gameObject.layer].Add(gameObject);
		//print(LayerMask.LayerToName(gameObject.layer)+mLayersObjectList[gameObject.layer].length);

	}
	
	//����������
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
	
	IgnoreCollisionBetween(layers.pismireBullet,layers.pismireBullet);
	IgnoreCollisionBetween(layers.beeBullet,layers.beeBullet);
	IgnoreCollisionBetween(layers.pismireBullet,layers.beeBullet);
	
	//��ֹ�����ӵ�����Ծ
	IgnoreCollisionBetween(layers.characterShape,layers.beeBullet);
	IgnoreCollisionBetween(layers.characterShape,layers.pismireBullet);
	
	if(!zzCreatorUtility.isHost())
	{
		//�ͻ����ӵ����ɴ�͸  :�ж϶����ڷ������� , �ͻ��˾Ͳ��ù��ӵ�����
		IgnoreCollisionBetween(layers.pismire,layers.beeBullet);
		IgnoreCollisionBetween(layers.bee,layers.pismireBullet);
	}
	
	
	IgnoreCollisionBetween(layers.bee,layers.bee);
	IgnoreCollisionBetween(layers.pismire,layers.pismire);
	
	for(var i=8; i<32; i+=1)
	{
		IgnoreCollisionBetween(layers.deadObject,i);
	}
}
