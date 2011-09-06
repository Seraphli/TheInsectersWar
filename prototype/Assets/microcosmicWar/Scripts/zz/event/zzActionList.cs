using UnityEngine;

public class zzActionList : zzOnAction
{
    public zzOnAction[] actionList;

    public override void impAction()
    {
        foreach (var lScript in actionList)
        {
            lScript.impAction();
        }
    }
}