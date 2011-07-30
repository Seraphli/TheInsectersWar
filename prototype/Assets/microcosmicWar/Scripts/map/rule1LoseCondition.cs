
using UnityEngine;
using System.Collections;

public enum Rule1LoseConditionType
{
    eInAll,
    eInOne,
}


class rule1LoseCondition : IobjectListener
{
    public string teamName = "unnamed team";
    public Rule1LoseConditionType rule1LoseConditionType;

    void Start()
    {
        //print(gameObject.name + " " + teamName);
        initedCall();
    }

    public override void initedCall()
    {
        var lRule = rule1.Singleton;
        if (lRule)
        {
            if (rule1LoseConditionType == Rule1LoseConditionType.eInAll)
            {
                lRule.getTeamLoseRule(teamName).addLoseConditionInAll(GetInstanceID());
            }
            else
            {
                lRule.getTeamLoseRule(teamName).addLoseConditionInOne(GetInstanceID());
            }
        }
    }

    public override void removedCall()
    {
        //print("removedCall");
        var lRule = rule1.Singleton;
        if (lRule)
        {
            if (rule1LoseConditionType == Rule1LoseConditionType.eInAll)
            {
                //print("rule1LoseConditionType == Rule1LoseConditionType.eInAll");
                lRule.getTeamLoseRule(teamName).removeLoseConditionInAll(GetInstanceID());
            }
            else
            {
                //print("rule1LoseConditionType != Rule1LoseConditionType.eInAll");
                lRule.getTeamLoseRule(teamName).removeLoseConditionInOne(GetInstanceID());
            }
            checkResult();
        }
    }

    public void checkResult()
    {
        var lGameScene =  GameScene.Singleton;
        string teamName = lGameScene.playerInfo.getTeamName();
        if (rule1.Singleton.isWin(teamName))
        {
            lGameScene.gameResult(teamName,true);
            //GameScene.getSingleton().endGameScene("you win");
        }
        else if (rule1.Singleton.isLose(teamName))
        {
            lGameScene.gameResult(teamName, false);
            //GameScene.getSingleton().endGameScene("you lose");
        }
    }

}

