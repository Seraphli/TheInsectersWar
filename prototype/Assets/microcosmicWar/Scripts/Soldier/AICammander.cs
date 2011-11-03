using UnityEngine;

public class AICammander:MonoBehaviour
{
    public zzDetectorBase detector;

    public AICommand.AIState cammand;

    public Transform owner;

    public Transform effectTransform;

    public GameObject freeEffect;
    public GameObject followEffect;
    public GameObject guardEffect;

    public void doCammand(AICommand.AIState pCammand)
    {
        cammand = pCammand;
        doCammand();
    }

    public void doEffect(AICommand.AIState pCammand)
    {
        GameObject lEffect = null;
        switch (pCammand)
        {
            case AICommand.AIState.free:
                lEffect = (GameObject)Instantiate(freeEffect);
                break;
            case AICommand.AIState.follow:
                lEffect = (GameObject)Instantiate(followEffect);
                break;
            case AICommand.AIState.guard:
                lEffect = (GameObject)Instantiate(guardEffect);
                break;
        }
        lEffect.transform.parent = effectTransform;
        lEffect.transform.localPosition = Vector3.zero;
    }

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