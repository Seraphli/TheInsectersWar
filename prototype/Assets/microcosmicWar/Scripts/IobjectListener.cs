
using UnityEngine;
using System.Collections;

public class IobjectListener : MonoBehaviour
{

    //class IobjectListener
    //{
    protected zzUtilities.voidFunction initedCallFunc = zzUtilities.nullFunction;
    protected zzUtilities.voidFunction removedCallFunc = zzUtilities.nullFunction;

    public void setInitedCallFunc(zzUtilities.voidFunction func)
    {
        initedCallFunc = func;
    }

    public void setRemovedCallFunc(zzUtilities.voidFunction func)
    {
        removedCallFunc = func;
    }

    public virtual void initedCall()
    {
        initedCallFunc();
    }

    public virtual void removedCall()
    {
        removedCallFunc();
    }
    //}

    //function Start()
    //{
    //	initCall();
    //}

    //void  Update (){
    //}
}