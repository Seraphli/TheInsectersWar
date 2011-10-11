using UnityEngine;

public class OperateObject:MonoBehaviour
{
    //高的被处理
    public int priority = 0;

    public CommandControlBase objectControl;
    public zzInterfaceGUI objectGui;

    public virtual bool canOperate(ObjectOperatives pUser)
    {
        return true;
    }

    public virtual void operateThis(ObjectOperatives pUser)
    {
        //因为网络部分需要延时处理,所以放在此处复制
        pUser.operatingObject = this;
        //if (objectControl)
        //    pUser.commandControl = objectControl;
        //if (objectGui)
        //    pUser.gui = objectGui;
        //return true;
    }

    public virtual void endOperate(ObjectOperatives pUser)
    {
        pUser.operatingObject = null;
        //pUser.commandControl = null;
    }
}