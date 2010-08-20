//保存玩家 选队 等信息,以便场景读取

enum Race
{
	ePismire=0,
	eBee,
	eNone,
};

static function eRaceToString(race:Race)
{
	switch(race)
	{
		case Race.ePismire: return "pismire";
		case Race.eBee: return "bee";
		//case Race.ePismire: return "pismire";
	}
	return ;
}

static function getAdversaryRace(race:Race)
{
	switch(race)
	{
		case Race.ePismire: return Race.eBee;
		case Race.eBee: return Race.ePismire;
		//case Race.ePismire: return "pismire";
	}
	return ;
}

static function getRaceLayer(race:Race):int
{
	switch(race)
	{
		case Race.eBee: return layers.bee;
		case Race.ePismire: return layers.pismire;
		//case Race.ePismire: return "pismire";
	}
	Debug.LogError(race);
	return ;
}

static function getAdversaryRaceLayer(raceLayer:int)
{
	switch(raceLayer)
	{
		case layers.pismire: return layers.bee;
		case layers.bee: return layers.pismire;
		//case Race.ePismire: return "pismire";
	}
	Debug.LogError(raceLayer);
	return ;
}


var	race:Race = Race.eNone;

var 	playerName="player";

var	teamName="";

function Awake()
{
	if(teamName=="")
		teamName=eRaceToString(race);
}


//function Awake()
//{
//	gameObject.DontDestroyOnLoad ();
//}
function setData(pOther:PlayerInfo)
{
	this.race = pOther.race;
}

function setRace(pRace:Race)
{
	race=pRace;
}

function getRace():Race
{
	return race;
	//return 1;
}

function getRaceName()
{
	return race;
	//return 1;
}

function setPlayerName(pName:String)
{
	playerName=pName;
}

function getPlayerName()
{
	return eRaceToString(getRaceName());
}

function getTeamName()
{
	return teamName;
}

//function Update () {
//}