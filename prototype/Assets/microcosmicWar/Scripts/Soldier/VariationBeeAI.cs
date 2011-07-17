using UnityEngine;

public class VariationBeeAI : SoldierAI
{
    public zzDetectorBase swoopDetector;
    public zzDetectorBase swoopAbleDetector;

    public SoldierAction swoopAction;

    public float overaweRate = 0.1f;

    protected override void actionCommandUpdate()
    {

        Transform lAim = getNowAimTransform();
        if (enable && lAim && !swoopAction.inActing)
        {
            int lFireTaget = needFire();
            actionCommand.clear();
            if (lFireTaget != 0)
            {
                if (Random.value < overaweRate)
                    actionCommand.Action2 = true;
                else
                {
                    actionCommand.Fire = true;
                    setFaceCommand(actionCommand, lFireTaget);
                }
            }
            else if (swoopDetector.detect(1, adversaryLayerMask).Length>0
                && swoopAbleDetector.detect(1, layers.standPlaceValue).Length==0)
            {
                if (Random.value < overaweRate)
                    actionCommand.Action2 = true;
                else
                    actionCommand.Action1 = true;
            }
            else
                actionCommand = moveToAim(aimPosition, lAim);

            actionCommandControl.setCommand(getCommand());
        }

    }
}