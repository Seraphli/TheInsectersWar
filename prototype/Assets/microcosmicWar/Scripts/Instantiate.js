
var lastSceneInfoName="_info";
//var sceneDataName = "_sceneData";
var sceneData:GameObject;

var pismirePlayerSpawn:Transform;
var beePlayerSpawn:Transform;

var pismirePlayerPrefab:GameObject;
var beePlayerPrefab:GameObject;

var playerSpawn:Transform;
var playerPrefab:GameObject;
var adversaryLayerValue:int;

//#pragma strict

// Use this for initialization
function Awake () {
	//print("Awake");
	//if(Network.peerType==NetworkPeerType.Disconnected || Network.peerType==NetworkPeerType.Connecting )
	//	Network.InitializeServer(32, 25000);
	var lLastSceneInfo = GameObject.Find(lastSceneInfoName);
	if(lLastSceneInfo)
	{
		var playerInfo:PlayerInfo = sceneData.GetComponent(PlayerInfo);
		playerInfo.setData( lLastSceneInfo.GetComponent(PlayerInfo) );
		//sceneData.AddComponent( lLastSceneInfo.GetComponent(PlayerInfo) );
		Destroy( lLastSceneInfo );
	}
	
	Network.isMessageQueueRunning = true;
	zzCreatorUtility.resetCreator();
}

function CreatePlayer()
{			
	var lClone:GameObject = zzCreatorUtility.Instantiate(playerPrefab, playerSpawn.position,playerSpawn.rotation, 0);
	//lClone.GetComponent(SoldierAI).SetAdversaryLayerValue(adversaryLayerValue);
	
	var cameraFollow:_2DCameraFollow = GameObject.Find("Main Camera").GetComponent(_2DCameraFollow);
	cameraFollow.target=lClone.transform;
	
	var soldier:Soldier = lClone.GetComponent(Soldier);
	soldier.userControl=true;
	
	//		var soldierAI:SoldierAI = lClone.GetComponent(SoldierAI);
	//		soldierAI.SetFinalAim(playerSpawn);
	//	soldierAI.SetAdversaryLayerValue(adversaryLayerValue);
}

function Start()
{
	var playerInfo:PlayerInfo = sceneData.GetComponent(PlayerInfo);
	//print(playerInfo.getRace());
	//var race:Race=playerInfo.getRace();

	if(playerInfo.getRace()==Race.ePismire)
	//sif(1)
	{
		playerSpawn=pismirePlayerSpawn;
		playerPrefab=pismirePlayerPrefab;
		adversaryLayerValue= 1<<LayerMask.NameToLayer("pismire");
	}
	else
	{
		playerSpawn=beePlayerSpawn;
		playerPrefab=beePlayerPrefab;
		adversaryLayerValue= 1<<LayerMask.NameToLayer("bee");
	}
	
	//adversaryLayerValue= 1<<LayerMask.NameToLayer(adversaryName);
	
	CreatePlayer();
}
