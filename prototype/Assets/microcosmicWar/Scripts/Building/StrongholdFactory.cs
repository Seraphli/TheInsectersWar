using UnityEngine;
using System.Collections.Generic;

public class StrongholdFactory : MonoBehaviour, SoldierFactory.Listener
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

    public HashSet<Soldier> soldierCreatedList
    {
        get
        {
            var lOut = _soldierCreatedList;
            _soldierCreatedList = null;
            return lOut;
        }
        set
        {
            if (_soldierCreatedList!=null)
            {
                foreach (var lSoldier in value)
                {
                    _soldierCreatedList.Add(lSoldier);
                }
            }
            else
                _soldierCreatedList = value;
        }
    }

    HashSet<Soldier> _soldierCreatedList = null;

    public Race race;

    public Transform leftProduceTransform;
    public Transform rightProduceTransform;

    Transform[] _finalAims;

    LayerMask _adversaryLayerMask;

    public Animation strongholdAnimation;

    public UnitFaceDirection nowProduceDirection = UnitFaceDirection.left;

    public void setRace(Race pRace)
    {
        race = pRace;
        var lAdversaryRace = PlayerInfo.getAdversaryRace(race);
        _adversaryLayerMask = PlayerInfo.getRaceObjectValue(lAdversaryRace);
        _finalAims = ArmyBase.getArmyBase(lAdversaryRace);
    }

}