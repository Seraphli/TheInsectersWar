
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class zzAimTranformList
{
    //目标列表,用于存储与获取目标,自动剔除已销毁的目标

    //用来初始化目标列表.索引小的 先被追踪
    public Transform[] aims = new Transform[] { };

    //目标列表
    List<Transform> mAimList = new List<Transform>();

    Transform popAim()
    {
        Transform lOut = null;

        //获取排在最后一个的 可用的 Transform
        while ((!lOut) && mAimList.Count > 0)
        {
            lOut = mAimList[mAimList.Count - 1];
            mAimList.RemoveAt(mAimList.Count - 1);
        }
        return lOut;
    }

    //现在锁定的目标
    [SerializeField]
    Transform nowAim;

    //移除当前的目标
    public void removeNowAim()
    {
        nowAim = popAim();
    }

    public Transform getAim()
    {
        if (!collisionLayer.isAliveFullCheck(nowAim) && (mAimList.Count>0) )
            nowAim = popAim();
        return nowAim;
    }

    //增加目标,并设置为当前目标
    public void checkAndAddAim(Transform pAim)
    {
        //Debug.Log(pAim);
        //Debug.Log(nowAim);
        //Debug.Log(pAim && (nowAim != pAim));
        if (collisionLayer.isAliveFullCheck(pAim) && (nowAim != pAim))
        {
            if (nowAim)
                mAimList.Add(nowAim);
            nowAim = pAim;
        }
    }

    public void initAimList()
    {
        mAimList.Clear();
        for (int i = aims.Length - 1; i >= 0; --i)
        {
            checkAndAddAim(aims[i]);
        }
    }
}