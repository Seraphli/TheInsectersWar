using UnityEngine;

public class VariationBeeAI : SoldierAI
{
    public Soldier soldier;
    public zzDetectorBase[] swoopDetectors;
    public zzDetectorBase swoopAbleDetector;
    public Transform swoopPoint;

    public SoldierAction swoopAction;

    public float overaweRate = 0.1f;

    bool swoopCheck()
    {
        if (character.isGrounded
            ||swoopAbleDetector.detect(1, layers.standPlaceValue).Length > 0)
            return false;
        var lSwoopPoint = swoopPoint.position;
        foreach (var lSwoopDetector in swoopDetectors)
        {
            var lColliders = lSwoopDetector.detect(1, adversaryLayerMask);
            if (lColliders.Length > 0)
            {
                var lSwoopPointToTarget = lColliders[0].transform.position - lSwoopPoint;

                //检测之间是否有障碍
                if (!Physics.Raycast(lSwoopPoint,
                    lSwoopPointToTarget,
                    lSwoopPointToTarget.magnitude,
                    layers.standPlaceValue))
                    return true;
            }
        }
        return false;
    }

    protected override void actionCommandUpdate()
    {

        Transform lAim = getNowAimTransform();
        if (enable && lAim && !soldier.nowAction)
        {
            int lFireTaget = needFire();
            actionCommand.clear();
            if (swoopCheck())
            {
                if (Random.value < overaweRate)
                    actionCommand.Action2 = true;
                else
                    actionCommand.Action1 = true;
            }
            else if (lFireTaget != 0)
            {
                if (Random.value < overaweRate)
                    actionCommand.Action2 = true;
                else
                {
                    actionCommand.Fire = true;
                    setFaceCommand(actionCommand, lFireTaget);
                }
            }
            else
                actionCommand = moveToAim(aimPosition, lAim);

            actionCommandControl.setCommand(getCommand());
        }

    }
}