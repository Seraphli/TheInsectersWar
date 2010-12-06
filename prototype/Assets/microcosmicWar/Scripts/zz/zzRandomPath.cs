using UnityEngine;
using System.Collections;

public class zzRandomPath : MonoBehaviour
{
    [System.Serializable]
    public class CheckPointPath
    {
        public Transform[] CheckPointList;

        //会选择此条路径的概率 weight/权重总和
        public int weight;
    }

    public CheckPointPath[] checkPointPaths = new CheckPointPath[0];

    CheckPointPath[] checkPointPathWeightList;

    void Start()
    {
        //初始化随机路径
        int lTotalWeigth = 0;
        foreach (CheckPointPath lCheckPointPath in checkPointPaths)
        {
            //若权重为0，改为1
            if (lCheckPointPath.weight == 0)
                lCheckPointPath.weight = 1;
            lTotalWeigth += lCheckPointPath.weight;
        }

        checkPointPathWeightList = new CheckPointPath[lTotalWeigth];
        int lIndex = 0;

        //按权重比例将路径填充进查询表checkPointPathWeightList
        foreach (CheckPointPath lCheckPointPath in checkPointPaths)
        {
            int lBeginIndex = lIndex;
            int lEndIndex = lBeginIndex + lCheckPointPath.weight;
            for (; lIndex < lEndIndex; ++lIndex)
                checkPointPathWeightList[lIndex] = lCheckPointPath;
        }

    }

    public int totalWeigth
    {
        get{return checkPointPathWeightList.Length;}
    }

    public CheckPointPath randomPath()
    {
        return checkPointPathWeightList[
            Random.Range(0, checkPointPathWeightList.Length)
            ];
    }

    void OnDrawGizmosSelected()
    {

        foreach (CheckPointPath lCheckPointPath in checkPointPaths)
        {
            if (lCheckPointPath.CheckPointList.Length > 0)
            {
                Transform lLastPoint = lCheckPointPath.CheckPointList[0];
                for (int i = 1; i < lCheckPointPath.CheckPointList.Length; ++i)
                {
                    Transform lNowPoint = lCheckPointPath.CheckPointList[i];
                    Gizmos.DrawLine(lLastPoint.position, lNowPoint.position);
                    lLastPoint = lNowPoint;
                }
                //Gizmos.DrawLine(lLastPoint.position, finalAim.position);
            }
        }
    }

}