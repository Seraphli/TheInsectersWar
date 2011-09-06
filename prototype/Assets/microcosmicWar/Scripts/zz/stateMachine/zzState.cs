using UnityEngine;

public class zzState:MonoBehaviour
{
    [System.Serializable]
    public class StateWay
    {
        public zzState destState;
        public zzOnAction action;
    }

    public StateWay defaultNextWay;

    public zzStateMachine stateMachine;

    public void tryToThisState()
    {
        stateMachine.tryToState(this);
    }

}