
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class zzAimTranformList
{
    //目标列表,用于存储与获取目标,自动剔除已销毁的目标

    //用来初始化目标列表.索引小的 先被追踪
    //public Transform[] aims = new Transform[] { };

    public enum AimType
    {
        aliveAim,
        checkPoint,
    }

    [System.Serializable]
    public class AimInfo
    {
        public AimInfo(Transform pAimTransform, AimType pAimType)
        {
            aimType = pAimType;
            aimTransform = pAimTransform;
        }

        public AimType aimType = AimType.aliveAim;
        public Transform aimTransform = null;
    }

    const float checkPointRadius = 3f;
    const float sqrCheckPointRadius = checkPointRadius * checkPointRadius;

    //目标列表
    List<AimInfo> mAimList = new List<AimInfo>();

    //for debug
    public AimInfo[] aimDebugInfo;
    void refreshAimDebugInfo()
    {
        aimDebugInfo = mAimList.ToArray();
    }

    /// <summary>
    /// 检测目标点是否可用,可用则返回真
    /// </summary>
    /// <param name="pAimInfo">目标点信息</param>
    /// <returns></returns>
    bool activeCheck(AimInfo pAimInfo)
    {
        if (pAimInfo!=null)
        {
            switch(pAimInfo.aimType)
            {
                case AimType.checkPoint:
                    return pAimInfo.aimTransform;
                    //break;

                case AimType.aliveAim:
                    //Debug.Log(collisionLayer.isAliveFullCheck(pAimInfo.aimTransform));
                    return collisionLayer.isAliveFullCheck(pAimInfo.aimTransform);
                    //break;

                //default: 
                //    Debug.LogError("activeCheck(AimInfo pAimInfo)"); 
                    //break;
            }
            Debug.LogError("activeCheck(AimInfo pAimInfo)");

        }
        //Debug.Log("activeCheck(AimInfo pAimInfo) return false");
        return false;
    }

    /// <summary>
    /// 检测目标点是否可用,可用则返回真
    /// </summary>
    /// <param name="pAimInfo">目标点信息</param>
    /// <param name="pSearcher">寻找者的位置</param>
    /// <returns></returns>
    bool activeCheck(AimInfo pAimInfo,Transform pSearcher)
    {
        //Debug.Log(pAimInfo==null?"null":pAimInfo.ToString());
        if (pAimInfo != null)
        {
            switch (pAimInfo.aimType)
            {
                case AimType.checkPoint:
                    if (pAimInfo.aimTransform)
                    {
                        Vector3 lLengthVector
                            = pAimInfo.aimTransform.position - pSearcher.position;

                        //Debug.Log("" + lLengthVector.sqrMagnitude + ">" + sqrCheckPointRadius);
                        return lLengthVector.sqrMagnitude > sqrCheckPointRadius;
                    }
                    else
                        return false;

                case AimType.aliveAim:
                    //Debug.Log(AimType.aliveAim);
                    return collisionLayer.isAliveFullCheck(pAimInfo.aimTransform);

            }
            Debug.LogError("activeCheck(AimInfo pAimInfo,Transform pSearcher)");
        }
        //Debug.Log("pAimInfo==null");
        return false; 

    }

    AimInfo popAim()
    {
        AimInfo lOut = null;

        //获取排在最后一个的 可用的 Transform
        //todo:可用性检测
        if (mAimList.Count > 0)
        {
            while ((lOut == null) && mAimList.Count > 0)
            {
                lOut = mAimList[mAimList.Count - 1];
                mAimList.RemoveAt(mAimList.Count - 1);
            }

            refreshAimDebugInfo();

        }
        return lOut;
    }

    //现在锁定的目标
    [SerializeField]
    AimInfo nowAim ;

    //移除当前的目标
    public void removeNowAim()
    {
        nowAim = popAim();
    }

    //未测试
    public void removeAim(Transform pAim)
    {
        if (nowAim.aimTransform == pAim)
            removeNowAim();
        else if (mAimList.Count > 0)
        {
            var lAimIndex = mAimList.FindIndex((x) => x.aimTransform == pAim);
            if (lAimIndex>0)
            {
                mAimList.RemoveAt(lAimIndex);
                refreshAimDebugInfo();
            }
        }
    }

    /// <summary>
    /// 得到目标的位移
    /// </summary>
    /// <param name="pSearcher">搜寻者自身的位移</param>
    /// <returns></returns>
    public Transform getAim(Transform pSearcher)
    {
        //if (!collisionLayer.isAliveFullCheck(nowAim) && (mAimList.Count > 0))
        //Debug.Log(activeCheck(nowAim, pSearcher));
        //Debug.Log("mAimList.Count" + mAimList.Count);

        if(!activeCheck(nowAim, pSearcher))
        {
            nowAim = popAim();
        }

        return nowAim == null ? null : nowAim.aimTransform;
    }

    //增加目标,并设置为当前目标,类型为AimType.aliveAim
    public void checkAndAddAim(Transform pAim)
    {
        checkAndAddAim(new AimInfo(pAim, AimType.aliveAim));
    }

    public void checkAndAddAim(AimInfo pAim)
    {
        //Debug.Log(pAim);
        //Debug.Log(nowAim);
        //Debug.Log(pAim && (nowAim != pAim));
        //if (collisionLayer.isAliveFullCheck(pAim) && (nowAim != pAim))
        //Debug.Log(pAim.aimTransform.name);
        //Debug.Log("added before" + mAimList.Count);
        if (activeCheck(pAim))//目标可用可用
        {
            //如果原目标与新的不一样,并且可用,则增加到候选表中
            if (
                nowAim != null
                && nowAim.aimTransform != pAim.aimTransform
                && activeCheck(nowAim)
                )
                mAimList.Add(nowAim);

            //新目标
            nowAim = pAim;
        }
        refreshAimDebugInfo();
        //Debug.Log("added after" + mAimList.Count);
    }

    //public void initAimList()
    //{
    //    mAimList.Clear();
    //    for (int i = aims.Length - 1; i >= 0; --i)
    //    {
    //        checkAndAddAim(aims[i]);
    //    }
    //}
}