using UnityEngine;
using System.Collections;



//class AnimationImpInTime extends AnimationListener
//{

//    FIXME_VAR_TYPE ImpFunction=zzUtilities.nullFunction;

//    FIXME_VAR_TYPE ImpTime=0.0f;

//    //此次循环中是否执行过
//    protected FIXME_VAR_TYPE IsImpInThisLoop=false;

//    virtual  void  updateCallback ( float time  ){
//        //Debug.Log("AnimationImpInTime");
//        if(!IsImpInThisLoop && time>ImpTime)
//        {
//            ImpFunction();
//            IsImpInThisLoop=true;
//        }
//    }

//    virtual  void  overEndBeforeUpdateCallback (){IsImpInThisLoop=false;}

//    virtual  void  endTheAnimationCallback (){IsImpInThisLoop=false;}
//}

//-----------------------------------------------------------------------------------------------

public class AnimationImpTimeListInfo
{
    public AnimationImpTimeListInfo(float pTime, zzUtilities.voidFunction func)
    {
        ImpFunction = func;
        ImpTime = pTime;
    }

    public AnimationImpTimeListInfo()
    {
        //ImpFunction=zzUtilities.voidFunction();

        ImpTime = 0.0f;
    }

    public zzUtilities.voidFunction ImpFunction = zzUtilities.nullFunction;

    public float ImpTime = 0.0f;
}

public class AnimationImpInTimeList : AnimationListener
{
    //时间要从小到大排列
    public AnimationImpTimeListInfo[] animationImpTimeListInfo;

    protected ArrayList infoArray = new ArrayList();

    protected int playNum = 0;

    public override void beginTheAnimationCallback()
    {
        playNum = 0;
    }

    public override void updateCallback(float time)
    {
        //Debug.Log("AnimationImpInTimeList:" + time);
        while (playNum < animationImpTimeListInfo.Length && time > animationImpTimeListInfo[playNum].ImpTime)
        {
            animationImpTimeListInfo[playNum].ImpFunction();
            ++playNum;
        }
    }

    public override void overBeginCall()
    {
        playNum = 0;
    }

    public void addImp(float pTime, zzUtilities.voidFunction func)
    {
        AnimationImpTimeListInfo imp = new AnimationImpTimeListInfo();
        imp.ImpTime = pTime;
        imp.ImpFunction = func;
        infoArray.Add(imp);
    }

    public void endAddImp()
    {
        AnimationImpTimeListInfo[] lTemp = (AnimationImpTimeListInfo[])infoArray.ToArray(typeof(AnimationImpTimeListInfo));
        setImpInfoList(lTemp);
        infoArray = new ArrayList();
    }


    //virtual  void  overEndCallback (){IsImpInThisLoop=false;}

    //考虑到执行在非循环动画中,超出动画范围处,所有将下面删除
    //virtual  function endTheAnimationCallback()
    //{
    //	playNum=animationImpTimeListInfo.length;
    //}

    //virtual  function overEndAfterUpdateCallback()
    //{
    //	playNum=0;
    //}

    public AnimationImpTimeListInfo[] getImpInfoList()
    {
        return animationImpTimeListInfo;
    }

    public void setImpInfoList(AnimationImpTimeListInfo[] pInfo)
    {
        animationImpTimeListInfo = pInfo;
    }
}

