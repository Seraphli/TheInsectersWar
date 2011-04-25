using UnityEngine;

public class zzOnFrameList:MonoBehaviour
{
    public zzOnAction[] actionList = new zzOnAction[0]{};

    bool updated = false;

    bool fixedUpdated = false;

    public int actionIndex = 0;

    void Start()
    {
        if (actionList.Length == 0)
            enabled = false;
    }

    void checkAction()
    {
        if(updated&&fixedUpdated)
        {
            updated = false;
            fixedUpdated = false;
            if (actionList[actionIndex])
                actionList[actionIndex].impAction();
            ++actionIndex;
            if (actionIndex >= actionList.Length)
                enabled = false;

        }
    }

    void LateUpdate()
    {
        updated = true;
        checkAction();
    }

    void FixedUpdate()
    {
        fixedUpdated = true;
        checkAction();
    }

}