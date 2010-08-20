
var componentToInit:Component;

function init( p:Hashtable)
{
	//componentToInit.SendMessage("init",p);
	if(zzCreatorUtility.isHost())
	{
		//zzCreatorUtility.sendMessag2Two(gameObject,"impInit","initedFromRPC",gameObject.layer);
		if(Network.peerType ==NetworkPeerType.Disconnected)
			impInit(p);
		else
			gameObject.networkView.RPC("initedFromRPC",
				RPCMode.AllBuffered,
				zzSerializeString.getSingleton().pack(p)); 
	}
}

@RPC
function initedFromRPC(p:String)
{
	impInit(zzSerializeString.getSingleton().unpackToData(p));
}

function impInit( p:Hashtable )
{
	var object;
	object=componentToInit;;
	object.init(p);
}