
var character:zzCharacter;
var actionCommandControl:ActionCommandControl;
var owner:GameObject;
//var transform;

function Start()
{
	//var lOwner:GameObject  = transform.parent.gameObject;
	if(!owner)
		return;
	var lHero:Hero =  owner.GetComponentInChildren(Hero);
	character = lHero.getCharacter();
	actionCommandControl = owner.GetComponentInChildren(ActionCommandControl);
	
	if(Network.peerType !=NetworkPeerType.Disconnected && networkView.isMine)
	{
		networkView.RPC("RPCSetOwner",RPCMode.Others, owner.networkView.viewID);
	}

}

function setOwner(pOwner:GameObject)
{
	gameObject.name="NS";
	transform.parent = pOwner.transform;
	owner = pOwner;
	
}

@RPC
function RPCSetOwner(pOwnerID:NetworkViewID)
{
	var lOwnerHeroObject:GameObject = NetworkView.Find(pOwnerID).gameObject;
	
	gameObject.name="NS";
	owner = lOwnerHeroObject;
	
	//-------------------------------------------
	var lHero:Hero =  owner.GetComponentInChildren(Hero);
	character = lHero.getCharacter();
	actionCommandControl = owner.GetComponentInChildren(ActionCommandControl);
}

function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo)
{
	character.OnSerializeNetworkView(stream,info);
	actionCommandControl.OnSerializeNetworkView(stream,info);
}

//function Update () {
//}