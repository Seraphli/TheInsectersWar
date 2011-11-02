using UnityEngine;

public class AICommand:MonoBehaviour
{
    public enum AIState
    {
        free,
        follow,
        guard,
    }

    public AIState state = AIState.free;

    public ISoldierAI ai;

    public void follow(Transform pAim)
    {
        releaseAI();
        state = AIState.follow;
        ai.followTranform = pAim;
    }

    public void releaseAI()
    {
        state = AIState.free;
    }

    public void guard()
    {
        releaseAI();
        state = AIState.guard;
        ai.followTranform = transform;
    }
}