
class TeamLoseData
{
	var mLoseConditionInAll=Array();
	var mLoseConditionInOne=Array();
	var mLose=false;
	
	//要失去所有的战败条件才会输
	function addLoseConditionInAll(pValue)
	{
		//Debug.Log("addLoseConditionInAll"+pValue);
		mLoseConditionInAll.Add(pValue);
	}
	
	//失去一个条件即输
	function addLoseConditionInOne(pValue)
	{
		//Debug.Log("addLoseConditionInOne"+pValue);
	}
	
	//去掉多个战败条件中特定编号的一个条件
	function removeLoseConditionInAll(pValue)
	{/*
		for( var i=0;i<mLoseConditionInAll.length;++i)
		{
			if(mLoseConditionInAll[i]== pValue)
			{
				mLoseConditionInAll.RemoveAt(i);
				break;
			}
		}*/
		//Debug.Log("removeLoseConditionInAll"+pValue);
		zzUtilities.removeValueInArray(mLoseConditionInAll,pValue);
			
		//判断战败条件是否为空
		if(mLoseConditionInAll.length==0)
			mLose=true;
		//Debug.Log("mLoseConditionInAll.length"+mLoseConditionInAll.length+mLose);
	}
	
	function removeLoseConditionInOne( pValue)
	{
		//Debug.Log("removeLoseConditionInOne"+pValue);
		mLose=true;
	}
	
	function isLose()
	{
		return mLose;
	}
}

class TeamData
{
	var teamLoseRule=TeamLoseData();
}

var teamNameList=Array();

//["String"]=TeamNameList
var teamLoseInfoList=Hashtable();

class TeamInfo
{
	var teamName:String;
}

function addTeam( teamInfo :TeamInfo)
{
	//print(teamInfo.teamName);
	teamNameList.Add(teamInfo.teamName);
	teamLoseInfoList[teamInfo.teamName]=TeamData();
}

function isWin(teamName:String)
{	
	//print("isWin"+teamName);
	var isError= teamLoseInfoList[teamName];
	
	var lWin=true;
	
	for(var i:System.Collections.DictionaryEntry in teamLoseInfoList)
	{
		var lTeamName:String = i.Key;
		var lTeamLoseData:TeamData=i.Value;
		
		//并且没有输 isLose() ==
		if( lTeamName!=teamName  
		&& !lTeamLoseData.teamLoseRule.isLose() )
		{
			//print(lTeamName+1);
			//print(teamName+1);
			//print(lTeamLoseData.teamLoseRule.isLose());
			lWin=false;
			break;
		}
	}
	return lWin;
	
}

function isLose(teamName:String)
{
	//print(teamName);
	var teamData:TeamData=teamLoseInfoList[teamName];
	return  teamData.teamLoseRule.isLose();
}

function getTeamLoseRule( teamName:String)
{
	//print(teamName);
	var teamData:TeamData=teamLoseInfoList[teamName];
	return  teamData.teamLoseRule;
}
/*
function showResult( teamName:String)
{
	if(isWin(teamName))
	else if(isLose(teamName))
}*/

static protected var singletonInstance:rule1=null;

static function getSingleton()
{
	return singletonInstance;
}

var teamNameListInfo:TeamInfo[];

function Awake()
{
	//print(gameObject.name);
	if(singletonInstance)
		Debug.LogError("have singletonInstance");
	singletonInstance = this;
	//print(gameObject.name);
	
	for(var i:TeamInfo in teamNameListInfo)
	{
		getSingleton().addTeam(i);
	}
	//print("Awake "+teamLoseInfoList.Count);
}

