using UnityEngine;

public class zzStateMachine:MonoBehaviour
{
    public zzState state;

    void Start()
    {
        foreach (Transform lSub in transform)
        {
            foreach (var lState in lSub.GetComponents<zzState>())
            {
                lState.stateMachine = this;
            }
        }
    }

    public void toDefaultNextState()
    {
        var lWay = state.defaultNextWay;
        lWay.action.impAction();
        state = lWay.destState;
    }

    public void tryToState(zzState pDestState)
    {
        if (state.defaultNextWay.destState == pDestState)
            toDefaultNextState();
    }
}