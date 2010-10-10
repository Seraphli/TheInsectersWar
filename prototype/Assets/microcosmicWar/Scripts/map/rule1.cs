
using UnityEngine;
using System.Collections;

[System.Serializable]
public class TeamLoseData
{
    public ArrayList mLoseConditionInAll = new ArrayList();
    public ArrayList mLoseConditionInOne = new ArrayList();
    public bool mLose = false;

    //要失去所有的战败条件才会输
    public void addLoseConditionInAll(object pValue)
    {
        //Debug.Log("addLoseConditionInAll"+pValue);
        mLoseConditionInAll.Add(pValue);
    }

    //失去一个条件即输
    public void addLoseConditionInOne(object pValue)
    {
        //Debug.Log("addLoseConditionInOne"+pValue);
    }

    //去掉多个战败条件中特定编号的一个条件
    public void removeLoseConditionInAll(object pValue)
    {/*
		for( FIXME_VAR_TYPE i=0;i<mLoseConditionInAll.length;++i)
		{
			if(mLoseConditionInAll[i]== pValue)
			{
				mLoseConditionInAll.RemoveAt(i);
				break;
			}
		}*/
        //Debug.Log("removeLoseConditionInAll"+pValue);
        zzUtilities.removeValueInArray(mLoseConditionInAll, pValue);

        //判断战败条件是否为空
        if (mLoseConditionInAll.Count == 0)
            mLose = true;
        //Debug.Log("mLoseConditionInAll.length"+mLoseConditionInAll.Count+mLose);
    }

    public void removeLoseConditionInOne(object pValue)
    {
        //Debug.Log("removeLoseConditionInOne"+pValue);
        mLose = true;
    }

    public bool isLose()
    {
        return mLose;
    }
}

[System.Serializable]
public class TeamData
{
    public TeamLoseData teamLoseRule = new TeamLoseData();
}

[System.Serializable]
public class TeamInfo
{
    public string teamName;
}

public class rule1 : MonoBehaviour
{



    public TeamInfo[] teamNameListInfo;

    public ArrayList teamNameList = new ArrayList();

    //["String"]=TeamNameList
    public Hashtable teamLoseInfoList = new Hashtable();

    public void addTeam(TeamInfo teamInfo)
    {
        //print(teamInfo.teamName);
        teamNameList.Add(teamInfo.teamName);
        teamLoseInfoList[teamInfo.teamName] = new TeamData();
    }

    public bool isWin(string teamName)
    {
        //print("isWin"+teamName);
        object isError = teamLoseInfoList[teamName];

        bool lWin = true;

        foreach (System.Collections.DictionaryEntry i in teamLoseInfoList)
        {
            string lTeamName = (string)i.Key;
            TeamData lTeamLoseData = (TeamData)i.Value;

            //并且没有输 isLose() ==
            if (lTeamName != teamName
            && !lTeamLoseData.teamLoseRule.isLose())
            {
                //print(lTeamName+1);
                //print(teamName+1);
                //print(lTeamLoseData.teamLoseRule.isLose());
                lWin = false;
                break;
            }
        }
        return lWin;

    }

    public bool isLose(string teamName)
    {
        //print(teamName);
        TeamData teamData = (TeamData)teamLoseInfoList[teamName];
        return teamData.teamLoseRule.isLose();
    }

    public TeamLoseData getTeamLoseRule(string teamName)
    {
        //print(teamName);
        TeamData teamData = (TeamData)teamLoseInfoList[teamName];
        return teamData.teamLoseRule;
    }
    /*
    void  showResult (  string teamName  ){
        if(isWin(teamName))
        else if(isLose(teamName))
    }*/

    static protected rule1 singletonInstance = null;

    public static rule1 getSingleton()
    {
        return singletonInstance;
    }

    void Awake()
    {
        //print(gameObject.name);
        if (singletonInstance)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
        //print(gameObject.name);

        foreach (TeamInfo i in teamNameListInfo)
        {
            getSingleton().addTeam(i);
        }
        //print("Awake "+teamLoseInfoList.Count);
    }


}