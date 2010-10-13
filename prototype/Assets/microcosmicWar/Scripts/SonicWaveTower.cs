
using UnityEngine;
using System.Collections;

public class SonicWaveTower : MonoBehaviour
{

    public float frequencySeconds = 1;
    public Race race = Race.eNone;

    void Awake()
    {
        initRace(race);
    }

    void My()
    {
        SonicAttack sonicAttackAI = gameObject.GetComponentInChildren<SonicAttack>();
        sonicAttackAI.Attack();
    }

    void Start()
    {
        zzTimer zTimer = gameObject.GetComponentInChildren<zzTimer>();
        zTimer.setInterval(frequencySeconds);
        zTimer.setImpFunction(My);
    }

    public void initRace(Race pRace)
    {
        if (pRace == Race.eNone)
            return;
        race = pRace;
        GameObject lTowerObject = gameObject;
        //AcousticTower lAcousticTower = lTowerObject.GetComponent<AcousticTower>();
        SonicAttack lSonicAttack = lTowerObject.GetComponent<SonicAttack>();

        lTowerObject.layer = PlayerInfo.getRaceLayer(pRace);
        //lSonicAttack.setAdversaryLayer(PlayerInfo.getAdversaryRaceLayer(race));
        lSonicAttack.setHarmLayerMask(1<<PlayerInfo.getAdversaryRaceLayer(pRace));

        foreach(Transform subShape in transform.Find("shape"))
            subShape.gameObject.layer = lTowerObject.layer;
        collisionLayer.addCollider(gameObject);

    }

    public virtual void init(Hashtable info)
    {
        initRace((Race)info["race"]);
    }
}