
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArmyBase : MonoBehaviour
{
    public string adversaryName = "";

    public Transform finalAim;
    public Transform[] finalAims;

    public LayerMask adversaryLayerMask;

    public Transform produceTransform;

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
        adversaryLayerMask = PlayerInfo.getRaceObjectValue(
            PlayerInfo.stringToRace(adversaryName));

        if (!produceTransform)
            produceTransform = transform;

        if (finalAims == null || finalAims.Length == 0)
            finalAims = getArmyBase(PlayerInfo.stringToRace(adversaryName));

        Life lLife = gameObject.GetComponent<Life>();
        //lLife.setDieCallback(dieCall);
        lLife.addDieCallback(dieCall);
    }

    public void dieCall(Life p)
    {
        if (objectListener != null)
            objectListener.removedCall();
        Destroy(gameObject);
    }

}