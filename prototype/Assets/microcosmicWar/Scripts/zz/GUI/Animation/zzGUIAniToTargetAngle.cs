using UnityEngine;

public class zzGUIAniToTargetAngle:MonoBehaviour
{

    public zzGUIGroupTransform panUiTransform;
    public float angularSpeed = 180f;

    //包含方向
    //float nowSpeed;
    public float targetAngle;

    public float angle
    {
        get { return panUiTransform.angle; }
        set { panUiTransform.angle = value; }
    }

    public void rotateToTarget(float pTargetAngle)
    {
        targetAngle = angle + Mathf.DeltaAngle(angle, pTargetAngle);
        enabled = true;
    }

    void Update()
    {
        angle = Mathf.MoveTowardsAngle(angle, targetAngle, Time.deltaTime * angularSpeed);
        if (Mathf.Approximately(angle, targetAngle))
            enabled = false;
        //Mathf.MoveTowards();
    }
}