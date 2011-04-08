using UnityEngine;

public class StrongholdFactory : MonoBehaviour, SoldierFactory.SoldierFactoryListener
{

    public Transform prepareProduce()
    {
        switch(nowProduceDirection)
        {
            case UnitFaceDirection.left:
                nowProduceDirection = UnitFaceDirection.right;
                strongholdAnimation.CrossFade("leftProduce",0.1f);
                return leftProduceTransform;
            case UnitFaceDirection.right:
                nowProduceDirection = UnitFaceDirection.left;
                strongholdAnimation.CrossFade("rightProduce", 0.1f);
                return rightProduceTransform;
            default:
                Debug.LogError("no the direction");
                return null;
        }
    }

    public Transform[] finalAims
    {
        get
        {
            return _finalAims;
        }
    }

    public LayerMask adversaryLayerMask
    {
        get
        {
            return _adversaryLayerMask;
        }
    }

    public Race race;

    public Transform leftProduceTransform;
    public Transform rightProduceTransform;

    Transform[] _finalAims;

    LayerMask _adversaryLayerMask;

    public Animation strongholdAnimation;

    public UnitFaceDirection nowProduceDirection = UnitFaceDirection.left;

    void Start()
    {
        var lAdversaryRace = PlayerInfo.getAdversaryRace(race);
        _adversaryLayerMask = PlayerInfo.getRaceObjectValue(lAdversaryRace);
        if (_finalAims == null || _finalAims.Length == 0)
            _finalAims = ArmyBase.getArmyBase(lAdversaryRace);

        foreach (var lSoldierFactory in GetComponents<SoldierFactory>())
        {
            print("lSoldierFactory");
            lSoldierFactory.listener = this;
        }
    }

}