//单实例类,一个场景只许有一个
static protected var singletonInstance:GameScene;

var lastSceneInfoName="_info";
//var sceneDataName = "_sceneData";

var sceneData:GameObject;
/*
var pismirePlayerSpawn:Transform;
var beePlayerSpawn:Transform;

var pismirePlayerPrefab:GameObject;
var beePlayerPrefab:GameObject;

var playerSpawn:Transform;
var playerPrefab:GameObject;
var adversaryLayerValue:int;
*/
var pismirePlayerSpawn:HeroSpawn;
var beePlayerSpawn:HeroSpawn;

var playerSpawn:HeroSpawn;
var adversaryPlayerSpawn:HeroSpawn;

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
	
	
	//creat team info
	//{
		var lRule:rule1 = gameObject.GetComponent(rule1);
		var lTeam1Info = TeamInfo();
		lTeam1Info.teamName = PlayerInfo.eRaceToString(Race.ePismire);
		var lTeam2Info = TeamInfo();
		lTeam2Info.teamName = PlayerInfo.eRaceToString(Race.eBee);
		lRule.addTeam(lTeam1Info);
		lRule.addTeam(lTeam2Info);
	
	//}
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
		playerSpawn.createHeroFirstTime();
		
		if(Network.connections.Length>0)
			adversaryPlayerSpawn.createHeroFirstTime();
}

function Start()
{
	//var playerInfo:PlayerInfo = sceneData.GetComponent(PlayerInfo);
	var playerInfo:PlayerInfo = getPlayerInfo();
	//print(playerInfo.getRace());
	//var race:Race=playerInfo.getRace();

	if(playerInfo.getRace()==Race.ePismire)
	//sif(1)
	{
		playerSpawn=pismirePlayerSpawn;
		adversaryPlayerSpawn=beePlayerSpawn;
		//playerPrefab=pismirePlayerPrefab;
		adversaryLayerValue= 1<<LayerMask.NameToLayer("pismire");
	}
	else
	{
		playerSpawn=beePlayerSpawn;
		adversaryPlayerSpawn=pismirePlayerSpawn;
		//playerPrefab=beePlayerPrefab;
		adversaryLayerValue= 1<<LayerMask.NameToLayer("bee");
	}
	
	//adversaryLayerValue= 1<<LayerMask.NameToLayer(adversaryName);
	if( zzCreatorUtility.isHost() )
	{
		if(needCreatePlayer)
		{
			playerSpawn.setOwer(Network.player);
			
			if(Network.connections.Length>0)
			{
				var lIntRace:int = playerInfo.getAdversaryRace(playerInfo.getRace());
				networkView.RPC("RPCSetRace",Network.connections[0],lIntRace);
				adversaryPlayerSpawn.setOwer(Network.connections[0]);
			}
			CreatePlayer();
		}
	}
	
}

@RPC
function RPCSetRace(pRace:int)
{
	var playerInfo:PlayerInfo = getPlayerInfo();
	playerInfo.setRace(pRace);
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
	Time.timeScale=0;
	if(needOnGUI)
		return;
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