

// Use this for initialization
function Awake () {
	//print("Awake");
	//if(Network.peerType==NetworkPeerType.Disconnected || Network.peerType==NetworkPeerType.Connecting )
	//	Network.InitializeServer(32, 25000);
	
	Network.isMessageQueueRunning = true;
	zzCreatorUtility.resetCreator();
}
