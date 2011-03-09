using UnityEngine;
using System.Collections;

public class BomberAI:MonoBehaviour
{
    public ActionCommandControl actionCommandControl;

    protected UnitActionCommand actionCommand = new UnitActionCommand();

    public LayerMask adversaryLayerMask = -1;

    public Emitter emitter;

    public zzDetectorBase fireDetector;

    public Transform selfAirBase;
    public Transform adversaryBase;

    public Transform nowAim;

    //public enum State
    //{
    //    attack,
    //    goBack,
    //}

    //public State state;

    public float actionCommandUpdateInterval = 0.25f;

    public float fireDelayRange = 0.5f;

    public float goBackDelayAfterFire = 1f;

    zzTimer actionCommandTimer;

    void Start()
    {
        actionCommandTimer = gameObject.AddComponent<zzTimer>();
        actionCommandTimer.setInterval(actionCommandUpdateInterval);
        actionCommandTimer.setImpFunction(fireDetect);

        nowAim = adversaryBase;

        UpdateCommand();
    }

    void moveToAim(UnitActionCommand pCommand,Vector3 pAim)
    {
        var lPosition = transform.position;
        if (lPosition.x < pAim.x)
        {
            pCommand.FaceLeft = false;
            pCommand.FaceRight = true;
        }
        else
        {
            pCommand.FaceLeft = true;
            pCommand.FaceRight = false;
        }
    }

    void fireDetect()
    {
        var lResult = fireDetector.detect(1, adversaryLayerMask);
        if (lResult.Length > 0)
        {
            actionCommandTimer.setInterval(fireDelayRange);
            actionCommandTimer.setImpFunction(fire);
            emitter.bulletAliveTime = getBulletAliveTime(lResult[0].transform.position);
            print(getBulletAliveTime(lResult[0].transform.position));
        }

    }

    void fire()
    {
        print("fire");

        actionCommand.Fire = true;
        actionCommandControl.setCommand(actionCommand);

        actionCommandTimer.setInterval(0f);
        actionCommandTimer.setImpFunction(closeFire);
    }

    void closeFire()
    {
        actionCommand.Fire = false;
        actionCommandControl.setCommand(actionCommand);

        actionCommandTimer.setInterval(goBackDelayAfterFire);
        actionCommandTimer.setImpFunction(toGoBackState);

    }

    void toGoBackState()
    {
        print("toGoBackState");
        nowAim = selfAirBase;
        Destroy(actionCommandTimer);
        UpdateCommand();
    }

    void UpdateCommand()
    {
        moveToAim(actionCommand, nowAim.position);
        actionCommandControl.setCommand(actionCommand);
    }

    float getBulletAliveTime(Vector3 pAim)
    {
        float pS = pAim.y - transform.position.y;
        return Mathf.Sqrt(2f * pS/Physics.gravity.y);
    }

}