
using UnityEngine;
using System.Collections;

public enum Rule1LoseConditionType
{
    eInAll,
    eInOne,
}


class rule1LoseCondition : IobjectListener
{
    public string teamName;
    public Rule1LoseConditionType rule1LoseConditionType;

    void Start()
    {
        print(gameObject.name + " " + teamName);
        initedCall();
    }

    public override void initedCall()
    {
        //print("initedCall");
        if (rule1.getSingleton())
        {
            if (rule1LoseConditionType == Rule1LoseConditionType.eInAll)
            {
                rule1.getSingleton().getTeamLoseRule(teamName).addLoseConditionInAll(GetInstanceID());
            }
            else
            {
                rule1.getSingleton().getTeamLoseRule(teamName).addLoseConditionInOne(GetInstanceID());
            }
        }
    }

    public override void removedCall()
    {
        //print("removedCall");
        if (rule1.getSingleton())
        {
            if (rule1LoseConditionType == Rule1LoseConditionType.eInAll)
            {
                //print("rule1LoseConditionType == Rule1LoseConditionType.eInAll");
                rule1.getSingleton().getTeamLoseRule(teamName).removeLoseConditionInAll(GetInstanceID());
            }
            else
            {
                //print("rule1LoseConditionType != Rule1LoseConditionType.eInAll");
                rule1.getSingleton().getTeamLoseRule(teamName).removeLoseConditionInOne(GetInstanceID());
            }
            checkResult();
        }
    }

    public void checkResult()
    {
        string teamName = GameScene.getSingleton().getPlayerInfo().getTeamName();

        if (rule1.getSingleton().isWin(teamName))
        {
            //print("rule1.getSingleton().isWin(teamName)");
            GameScene.getSingleton().endGameScene("you win");
        }
        else if (rule1.getSingleton().isLose(teamName))
        {
            //print("rule1.getSingleton().isLose(teamName)");
            GameScene.getSingleton().endGameScene("you lose");
        }
    }

}

