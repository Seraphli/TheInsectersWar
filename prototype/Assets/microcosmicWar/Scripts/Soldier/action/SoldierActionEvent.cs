
public class SoldierActionEvent:SoldierAction
{
    System.Action actionEndReceiver;

    public void addActionEndReceiver(System.Action pReceiver)
    {
        actionEndReceiver += pReceiver;
    }

    public override void OnActionEnd()
    {
        base.OnActionEnd();
        actionEndReceiver();
    }
}