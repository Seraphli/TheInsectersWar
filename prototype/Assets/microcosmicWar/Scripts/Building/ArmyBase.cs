
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmyBase : MonoBehaviour, SoldierFactory.SoldierFactoryListener
{

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

    public Transform prepareProduce() 
    {
        return _produceTransform;
    }

    public string adversaryName = "";

    [SerializeField]
    Transform[] _finalAims;

    [SerializeField]
    LayerMask _adversaryLayerMask;

    [SerializeField]
    Transform _produceTransform;

    public IobjectListener objectListener;

    Dictionary<GameObject, SoldierFactory> armyPrefabToFactory;

    public static Transform[] getArmyBase(Race pRace)
    {
        var lManager = GameSceneManager.Singleton.getManager(pRace,
            GameSceneManager.UnitManagerType.raceBase);
        var lOut = new Transform[lManager.objectCount];
        int i = 0;
        foreach (Transform lObject in lManager)
        {
            lOut[i] = lObject;
            ++i;
        }
        return lOut;
    }

    void Awake()
    {
        armyPrefabToFactory = new Dictionary<GameObject, SoldierFactory>();
    }

    public void addFactory(GameObject armyPrefab,float pProduceInterval,float pfirstTimeOffset)
    {
        SoldierFactory lSoldierFactory = gameObject.AddComponent<SoldierFactory>();
        lSoldierFactory.soldierToProduce = armyPrefab;
        lSoldierFactory.produceInterval = pProduceInterval;
        lSoldierFactory.firstTimeOffset = pfirstTimeOffset;
        armyPrefabToFactory[armyPrefab] = lSoldierFactory;
    }

    public void removeFactory(GameObject armyPrefab)
    {
        Destroy(armyPrefabToFactory[armyPrefab]);
        armyPrefabToFactory.Remove(armyPrefab);
    }


    void Start()
    {

        //adversaryLayerValue = 1 << LayerMask.NameToLayer(adversaryName);
        _adversaryLayerMask = PlayerInfo.getRaceObjectValue(
            PlayerInfo.stringToRace(adversaryName));

        if (!_produceTransform)
            _produceTransform = transform;

        if (_finalAims == null || _finalAims.Length == 0)
            _finalAims = getArmyBase(PlayerInfo.stringToRace(adversaryName));

        Life lLife = gameObject.GetComponent<Life>();
        //lLife.setDieCallback(dieCall);
        lLife.addDieCallback(dieCall);

        foreach (var lSoldierFactory in GetComponents<SoldierFactory>())
        {
            lSoldierFactory.listener = this;
        }
    }

    public void dieCall(Life p)
    {
        if (objectListener != null)
            objectListener.removedCall();
        Destroy(gameObject);
    }

}