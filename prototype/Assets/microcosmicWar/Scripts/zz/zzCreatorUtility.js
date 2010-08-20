//统一单机与联网的创建与销毁接口


class IzzGenericCreator
{
	virtual function Instantiate(prefab : Object, position : Vector3, rotation : Quaternion, group : int) : Object
	{
	}
	
	virtual function Destroy (gameObject : GameObject):void
	{
	}
	
	virtual function isMine(networkView:NetworkView)
	{
		return true;
	}
	
	virtual function sendMessage(gameObject:GameObject,methodName : String )
	{
	}
	
	virtual function sendMessage(gameObject:GameObject,methodName : String, value : Object)
	{
	}
	
}

class ZzNetCreator extends IzzGenericCreator
{
	virtual function Instantiate(prefab : Object, position : Vector3, rotation : Quaternion, group : int) 
	{
		return Network.Instantiate(prefab, position, rotation, group);
	}
	
	virtual function Destroy (gameObject : GameObject)
	{
		Network.Destroy(gameObject);
	}
	virtual function isMine(networkView:NetworkView)
	{
		return networkView.isMine;
	}
	
	virtual function sendMessage(gameObject:GameObject,methodName : String)
	{
		gameObject.networkView.RPC(methodName,RPCMode.All); 
	}
	
	virtual function sendMessage(gameObject:GameObject,methodName : String, value : Object)
	{
		gameObject.networkView.RPC(methodName,
			RPCMode.AllBuffered,
			value); 
	}
}

class ZzSingleCreator extends IzzGenericCreator
{
	virtual function Instantiate(prefab : Object, position : Vector3, rotation : Quaternion, group : int) 
	{
		return prefab.Instantiate(prefab, position, rotation);
	}
	
	virtual function Destroy (gameObject : GameObject)
	{
		gameObject.Destroy(gameObject);
	}
	
	virtual function isMine(networkView:NetworkView)
	{
		return true;
	}
	
	virtual function sendMessage(gameObject:GameObject,methodName : String)
	{
		gameObject.SendMessage(methodName);
	}
	
	virtual function sendMessage(gameObject:GameObject,methodName : String, value : Object)
	{
		gameObject.SendMessage(methodName,value);
	}
}

//默认为单机,省去初始化操作
static var zzGenericCreator:IzzGenericCreator=ZzSingleCreator();

static var host=true;

static function resetCreator()
{
	if(Network.peerType ==NetworkPeerType.Disconnected)
	{
		//print("Network.peerType ==NetworkPeerType.Disconnected");
		zzGenericCreator=ZzSingleCreator();
		host=true;
	}
	else
	{
		//print("Network.peerType !=NetworkPeerType.Disconnected");
		zzGenericCreator=ZzNetCreator();
		if (Network.isServer)
			host=true;
		else
			host=false;
	}
	//print(zzGenericCreator);
	//zzGenericCreator
	//print(host);
}

static function Instantiate(prefab : Object, position : Vector3, rotation : Quaternion, group : int) 
{
	//Debug.Log("Instantiate");
	//Debug.Log(zzGenericCreator);
	return zzGenericCreator.Instantiate(prefab, position, rotation, group);
}
static function Destroy (gameObject : GameObject)
{
	zzGenericCreator.Destroy(gameObject);
}

//单机时与作为服务器时返回真
static function isHost()
{
	return host;
}

static function isMine(networkView:NetworkView)
{
	return zzGenericCreator.isMine(networkView);
}

//在单机中避免RPC
static function sendMessage(gameObject:GameObject,methodName : String, value : Object)
{
	zzGenericCreator.sendMessage(gameObject,methodName,value);
}

static function sendMessage(gameObject:GameObject,methodName : String)
{
	zzGenericCreator.sendMessage(gameObject,methodName);
}

//联网时使用netMethodName,单机时 使用 singleMethodName
static function sendMessag2Two(gameObject:GameObject,singleMethodName : String, netMethodName : String,value : Object)
{
	if(Network.peerType ==NetworkPeerType.Disconnected)
		gameObject.SendMessage(singleMethodName,value);
	else
		gameObject.networkView.RPC(netMethodName,
			RPCMode.AllBuffered,
			value); 
}

//function Update () {
//}