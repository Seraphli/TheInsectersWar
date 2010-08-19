
static protected var singletonInstance:zzGameObjectCreator=null;

static function getSingleton()
{
	return singletonInstance;
}

class zzCreatorInfo
{
	//±ÿ–Î”–zzGameObjectInit
	var name:String;
	var prefab:GameObject;
}

var creatorList:zzCreatorInfo[];

//[name] = prefab
protected var creatorMap=Hashtable();


/*
["creatorName"]
["position"]
["rotation"]
*/
function create( p:Hashtable)
{
	var position=Vector3();
	var rotation=Quaternion();
	if(p.ContainsKey("position"))
		position = p["position"];
	if(p.ContainsKey("rotation"))
		rotation = p["rotation"];
	var clone = zzCreatorUtility.Instantiate( creatorMap[p["creatorName"]] , position , rotation , 0) ;
	var initObject:zzGameObjectInit = clone.GetComponent(zzGameObjectInit);
	initObject.init(p);
}

function Awake()
{
	if(singletonInstance)
		Debug.LogError("have singletonInstance");
	singletonInstance = this;
	
	for(var v:zzCreatorInfo in creatorList)
	{
		creatorMap[v.name]=v.prefab;
	}
}
