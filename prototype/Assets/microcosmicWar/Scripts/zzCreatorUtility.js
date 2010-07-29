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
}

static var zzGenericCreator=IzzGenericCreator();
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
	//print("Instantiate");
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

//function Update () {
//}