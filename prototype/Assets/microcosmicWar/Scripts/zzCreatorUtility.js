//ͳһ�����������Ĵ��������ٽӿ�


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

//����ʱ����Ϊ������ʱ������
static function isHost()
{
	return host;
}

static function isMine(networkView:NetworkView)
{
	return zzGenericCreator.isMine(networkView);
}

//�ڵ����б���RPC
static function sendMessage(gameObject:GameObject,methodName : String, value : Object)
{
	zzGenericCreator.sendMessage(gameObject,methodName,value);
}

static function sendMessage(gameObject:GameObject,methodName : String)
{
	zzGenericCreator.sendMessage(gameObject,methodName);
}

//function Update () {
//}