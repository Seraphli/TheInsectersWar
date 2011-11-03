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

    public Transform guardPos;

    void Awake()
    {
        guardPos = new GameObject("guardPos").transform;
        releaseAI();
    }

    void OnDestroy()
    {
        if (guardPos)
            Destroy(guardPos.gameObject);
    }

    public void follow(Transform pAim)
    {
        releaseAI();
        state = AIState.follow;
        ai.followTranform = pAim;
    }

    public void releaseAI()
    {
        state = AIState.free;
        guardPos.parent = transform;
        guardPos.localPosition = Vector3.zero;
        ai.followTranform = null;
    }

    public void guard()
    {
        releaseAI();
        state = AIState.guard;
        guardPos.parent = null;
        ai.followTranform = guardPos;
    }
}