using UnityEngine;

public class AiMachineGunAI:MonoBehaviour
{
    public DefenseTower defenseTower;
    public LifeTriggerDetector[] lifeTriggerDetectors;
    public UnitFaceDirectionObject unitFaceDirectionObject;
    //public

    //敌人还在这个角度时,枪不动
    public float fireDeviation = 4.0f;

    public LayerMask adversaryMask;

    public float detectInterval = 0.2f;

    void Start()
    {
        adversaryMask = PlayerInfo.getAdversaryRaceObjectMask(
            PlayerInfo.getRace(gameObject.layer));
        if (!zzCreatorUtility.isHost())
        {
            foreach (var lDetector in lifeTriggerDetectors)
            {
                Destroy(lDetector.gameObject);
            }
            Destroy(this);
            return;
        }
        foreach (var lDetector in lifeTriggerDetectors)
        {
            lDetector.detectLayerMask = adversaryMask;
        }
        var lTimer = gameObject.AddComponent<zzCoroutineTimer>();
        lTimer.setInterval(detectInterval);
        lTimer.setImpFunction(AiUpdate);
    }

    void AiUpdate()
    {
        LifeTriggerDetector lMaxDetected = null;
        int lMaxEnemyCount = 0;
        foreach (var lDetector in lifeTriggerDetectors)
        {
            lDetector.detect(1, adversaryMask);
            if (lDetector.targetCount > lMaxEnemyCount)
                lMaxDetected = lDetector;
        }
        if (lMaxDetected != null)
        {
            var lAimPosition = lMaxDetected.lockedTarget.position;
            unitFaceDirectionObject.face =
                UnitFace.getFace(lAimPosition.x - transform.position.x);
            defenseTower.takeAim(lAimPosition, fireDeviation);
            defenseTower.fire = true;
        }
        else
            defenseTower.fire = false;
    }

    //void setRace(Race pRace)
    //{
    //    adversaryMask = PlayerInfo.getAdversaryRaceObjectMask(pRace);
    //}
}