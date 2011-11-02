using UnityEngine;

public class AICammander:MonoBehaviour
{
    public zzDetectorBase detector;

    public AICommand.AIState cammand;

    public Transform owner;

    public void doCammand()
    {
        var lColliders = detector.detect();
        foreach (var lCollider in lColliders)
        {
            var lAICommand = lCollider.GetComponent<AICommand>();
            if(lAICommand)
            {
                switch(cammand)
                {
                    case AICommand.AIState.free:
                        lAICommand.releaseAI();
                        break;
                    case AICommand.AIState.follow:
                        lAICommand.follow(owner);
                        break;
                    case AICommand.AIState.guard:
                        lAICommand.guard();
                        break;
                }
            }
        }
    }
}