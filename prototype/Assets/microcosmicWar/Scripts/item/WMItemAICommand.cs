using UnityEngine;

public class WMItemAICommand : WMBagCellCreator
{
    public AICommand.AIState cammand;

    public override WM.IBagCell getBagCell()
    {
        return new WMGenericBagCell() { useFunc = use, doEffectFunc = doAICommandEffect };
    }

    public bool use(GameObject pGameObject)
    {
        doAICommand(pGameObject);
        return true;
    }

    void doAICommandEffect(GameObject pGameObject)
    {
        pGameObject.GetComponent<AICammander>().doEffect(cammand);
    } 

    void doAICommand(GameObject pGameObject)
    {
        pGameObject.GetComponent<AICammander>().doCammand(cammand);
    }
}