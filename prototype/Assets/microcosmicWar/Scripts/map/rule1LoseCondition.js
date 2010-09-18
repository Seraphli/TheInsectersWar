
enum Rule1LoseConditionType
{
	eInAll,
	eInOne,
}

class rule1LoseCondition extends IobjectListener//,MonoBehaviour
{
	var teamName:String;
	var rule1LoseConditionType:Rule1LoseConditionType;
	
	function Start()
	{
		print(gameObject.name+" "+teamName);
		initedCall();
	}

	virtual function initedCall()
	{
		if(rule1.getSingleton())
		{
			if(rule1LoseConditionType==Rule1LoseConditionType.eInAll)
			{
				rule1.getSingleton().getTeamLoseRule(teamName).addLoseConditionInAll(GetInstanceID());
			}
			else
			{
				rule1.getSingleton().getTeamLoseRule(teamName).addLoseConditionInOne(GetInstanceID());
			}
		}
	}

	virtual function removedCall()
	{
		if(rule1.getSingleton())
		{
			if(rule1LoseConditionType==Rule1LoseConditionType.eInAll)
			{
				rule1.getSingleton().getTeamLoseRule(teamName).removeLoseConditionInAll(GetInstanceID());
			}
			else
			{
				rule1.getSingleton().getTeamLoseRule(teamName).removeLoseConditionInOne(GetInstanceID());
			}
			checkResult();
		}
	}
	
	function checkResult()
	{
		var teamName:String = GameScene.getSingleton().getPlayerInfo().getTeamName();
		
		if(rule1.getSingleton().isWin(teamName))
		{
			GameScene.getSingleton().endGameScene( "you win" );
		}
		else if(rule1.getSingleton().isLose(teamName))
		{
			GameScene.getSingleton().endGameScene( "you lose" );	
		}
	}
}

