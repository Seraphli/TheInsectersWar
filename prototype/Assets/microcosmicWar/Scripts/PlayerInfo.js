//������� ѡ�� ����Ϣ,�Ա㳡����ȡ

enum Race
{
	ePismire=0,
	eBee,
	eNone,
};

var	race:Race = Race.eNone;

var 	playerName="player";

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

function setPlayerName(pName:String)
{
	playerName=pName;
}

function getPlayerName()
{
	return playerName;
}

//function Update () {
//}