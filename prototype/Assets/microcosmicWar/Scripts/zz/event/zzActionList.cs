using UnityEngine;

public class zzActionList : MonoBehaviour
{
    public zzOnAction[] actionList;

    public void impAction()
    {
        foreach (var lScript in actionList)
        {
            lScript.impAction();
        }
    }
}