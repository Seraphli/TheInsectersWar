//单实例类,一个场景只许有一个

static protected var singletonInstance:GameScene;

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

var needCreatePlayer=true;

static var sSceneData:GameObject;
//#pragma strict
/*
var winCondition=Hashtable();
var loseCondition=Hashtable();

function addWinCondition(pValue)
{
	winCondition[pValue]=true;
}

function reachCondition(pValue)
{
	enemyList.Remove(other.transform);
}

function checkCondition(pList:Hashtable,isWin:boolean)
{
	if(pList.Count==0)
	{
	}
}
*/
// Use this for initialization
function Awake () {
	singletonInstance = this;
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
	
	sSceneData=sceneData;
}

static function getSingleton()
{
	return singletonInstance;
}

function CreatePlayer()
{			
/*
	var lClone:GameObject = zzCreatorUtility.Instantiate(playerPrefab, playerSpawn.position,playerSpawn.rotation, 0);
	//lClone.GetComponent(SoldierAI).SetAdversaryLayerValue(adversaryLayerValue);
	
	var cameraFollow:_2DCameraFollow = GameObject.Find("Main Camera").GetComponent(_2DCameraFollow);
	cameraFollow.target=lClone.transform;
	
	//var soldier:Soldier = lClone.GetComponent(Soldier);
	//#####soldier.userControl=true;
	
	//		var soldierAI:SoldierAI = lClone.GetComponent(SoldierAI);
	//		soldierAI.SetFinalAim(playerSpawn);
	//	soldierAI.SetAdversaryLayerValue(adversaryLayerValue);
*/
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
	if(needCreatePlayer)
		CreatePlayer();
}

protected var needOnGUI=false;
protected var buttonInfo="";

function OnGUI ()
{
	if(needOnGUI)
	{
		
		GUILayout.BeginArea(Rect(Screen.width/2,Screen.height/2,200,Screen.height/2));
		if(GUILayout.Button(buttonInfo))
		{
			Time.timeScale=1;
			Application.LoadLevel("LoaderMenu");
		}
		GUILayout.EndArea();
	}
}

function  endGameScene(pInfo:String)
{
	//假如已经停止了,则不往下执行
	Board.clearList();
	if(needOnGUI)
		return;
	Time.timeScale=0;
	needOnGUI=true;
	buttonInfo=pInfo;
}

function OnDisconnectedFromServer(info : NetworkDisconnection) 
{
	endGameScene( "Network Player Disconnection" );
}

function OnPlayerDisconnected (player : NetworkPlayer)
{
	endGameScene( "Network Player Disconnection" );
}

function gameResult(pWinerRaceName:String)
{
	ImpGameResult(pWinerRaceName);
	if(Network.peerType ==NetworkPeerType.Disconnected)
		ImpGameResult(pWinerRaceName);
	else
		networkView.RPC( "ImpGameResult", RPCMode.Others, pWinerRaceName);
}

function getPlayerInfo()
{
	var playerInfo:PlayerInfo=sSceneData.GetComponent(PlayerInfo);
	return playerInfo;
}

@script RequireComponent(NetworkView)


@RPC
function ImpGameResult(pWinerRaceName:String)
{
	var playerInfo:PlayerInfo = sSceneData.GetComponent(PlayerInfo);
	
	if(pWinerRaceName==playerInfo.getPlayerName())
		endGameScene( "you win" );
	else
		endGameScene(  "you lose" ) ;
}