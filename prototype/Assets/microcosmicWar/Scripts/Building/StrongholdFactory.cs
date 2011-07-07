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
                if (Network.isServer)
                    networkView.RPC("RPCLeftProduce", RPCMode.Others);
                return leftProduceTransform;
            case UnitFaceDirection.right:
                nowProduceDirection = UnitFaceDirection.left;
                strongholdAnimation.CrossFade("rightProduce", 0.1f);
                if (Network.isServer)
                    networkView.RPC("RPCRightProduce", RPCMode.Others);
                return rightProduceTransform;
            default:
                Debug.LogError("no the direction");
                return null;
        }
    }

    [RPC]
    void RPCLeftProduce()
    {
        strongholdAnimation.CrossFade("leftProduce", 0.1f);
    }

    [RPC]
    void RPCRightProduce()
    {
        strongholdAnimation.CrossFade("rightProduce", 0.1f);
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

    public HashSet<Soldier> popSoldierCreatedList()
    {
        var lOut = _soldierCreatedList;
        _soldierCreatedList = null;
        return lOut;
    }

    public HashSet<Soldier> soldierCreatedList
    {
        get
        {
            return _soldierCreatedList;
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