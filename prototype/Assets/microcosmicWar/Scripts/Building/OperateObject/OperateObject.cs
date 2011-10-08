using UnityEngine;

public class OperateObject:MonoBehaviour
{
    public CommandControlBase objectControl;
    public zzInterfaceGUI objectGui;

    public virtual bool operateThis(ObjectOperatives pUser)
    {
        if (objectControl)
            pUser.commandControl = objectControl;
        if (objectGui)
            pUser.gui = objectGui;
        return true;
    }

    public void endOperate(ObjectOperatives pUser)
    {
        pUser.commandControl = null;
    }
}