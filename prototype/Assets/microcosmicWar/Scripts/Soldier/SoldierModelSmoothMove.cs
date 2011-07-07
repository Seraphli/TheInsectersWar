using UnityEngine;

public class SoldierModelSmoothMove:MonoBehaviour
{
    public Transform zeroTransform;
    public Transform parent;

    const float minGroundSpeed = 5f;

    const float maxGroundSpeed = 15f;

    const float minGroundSpeedDistance = 0.5f;

    const float minGroundSpeedDistanceSqrt = minGroundSpeedDistance * minGroundSpeedDistance;

    const float maxGroundDistance = 3.5f;

    const float minToMaxGroundDistance = maxGroundDistance - minGroundSpeedDistance;

    const float maxGroundDistanceSqrt = maxGroundDistance * maxGroundDistance;

    const float ySpeed = 20f;

    public bool onlyInClient = true;

    void Awake()
    {
        if ((onlyInClient && Network.isServer)
            ||Network.peerType == NetworkPeerType.Disconnected)
        {
            Destroy(this);
            return;
        }
        enabled = false;
        parent = zeroTransform.parent;
    }

    float getSpeed(float pDistance)
    {
        return pDistance < minGroundSpeedDistance ? minGroundSpeed
            : Mathf.Lerp(minGroundSpeed, maxGroundSpeed, (pDistance - minGroundSpeedDistance) / minToMaxGroundDistance);
    }

    public void beginMove()
    {
        zeroTransform.parent = null;
    }

    public void endMove()
    {
        zeroTransform.parent = parent;
        zeroPositionOffset = zeroTransform.position - parent.position;
        moveEndCheck();
    }

    void moveEndCheck()
    {
        if (zeroPositionOffset.sqrMagnitude > maxGroundDistanceSqrt)
        {
            zeroPositionOffset = Vector3.zero;
            zeroTransform.localPosition = Vector3.zero;
            enabled = false;
        }
        else
            enabled = true;
    }

    public Vector3 zeroPositionOffset;

    public void move(Vector3 pTargetPos)
    {
        //print(zeroPosition);
        print("zeroTransform.position.y:" + zeroTransform.position.y
            + " zeroTransform.localPosition.y" + zeroTransform.localPosition.y
            + " zeroPosition.y:" + zeroPositionOffset.y);
        //zeroPosition += pPrePos - pNewPos;
        zeroPositionOffset = zeroTransform.position - pTargetPos;
        if (zeroPositionOffset.sqrMagnitude > maxGroundDistanceSqrt)
        {
            zeroPositionOffset = Vector3.zero;
            zeroTransform.localPosition = Vector3.zero;
            enabled = false;
            print("> maxDistanceSqrt pTargetPos" + pTargetPos + " zeroPosition:" + zeroPositionOffset
                + "\nzeroTransform.position.y:" + zeroTransform.position.y);
            return;
        }
        enabled = true;
        zeroTransform.localPosition = reachTargetCheck(ref zeroPositionOffset);
        print("pTargetPos.y" + pTargetPos.y + " zeroPosition.y:" + zeroPositionOffset.y
                + "\nzeroTransform.position.y:" + zeroTransform.position.y);
    }

    public Vector3 reachTargetCheck(ref Vector3 pPostion)
    {
        print(" x:" + Mathf.Approximately(pPostion.x, 0f)
            + " y:" + Mathf.Approximately(pPostion.y, 0f)
            + " z:" + Mathf.Approximately(pPostion.z, 0f));
        if (
            Mathf.Approximately(pPostion.x, 0f)
            && Mathf.Approximately(pPostion.y, 0f)
            && Mathf.Approximately(pPostion.z, 0f)
            )
        {
            enabled = false;
            print("enabled = false");
            pPostion = Vector3.zero;
        }
        return pPostion;
    }

    void reachTargetCheck(float pMagnitude)
    {
        if (pMagnitude<0.0005)
        {
            enabled = false;
            //print("enabled = false");
            zeroPositionOffset = Vector3.zero;
        }
    }

    void reachTargetCheck()
    {
        if(zeroPositionOffset.sqrMagnitude<0.00000025)
        {
            enabled = false;
            zeroPositionOffset = Vector3.zero;
        }
    }

    //public float lastUpdateTime = 0f;

    float moveToZero(float current, float speed)
    {
        var lLength = Mathf.Abs(current);
        var lMoveDistance = speed * Time.deltaTime;
        lMoveDistance = Mathf.Min(lLength, lMoveDistance);
        return current + (current > 0 ? (-lMoveDistance) : lMoveDistance);
    }

    void Update()
    {
        //print("before:" + zeroPositionOffset.y);
        var lXLength = Mathf.Abs(zeroPositionOffset.x) ;
        //var lXDistance = getSpeed(lXLength) * Time.deltaTime;
        //lXDistance = Mathf.Min(lXDistance, lXLength);
        //var lScale = (lXLength-lXDistance) / lXLength;
        //zeroPositionOffset.x *= lScale;

        zeroPositionOffset.x = moveToZero(zeroPositionOffset.x, getSpeed(lXLength));
        zeroPositionOffset.y = moveToZero(zeroPositionOffset.y, ySpeed);
        //zeroPosition = Vector3.MoveTowards(zeroPosition, Vector3.zero,
        //    );//(Time.time - lastUpdateTime));
        //lastUpdateTime = Time.time;
        reachTargetCheck();
        //reachTargetCheck(lXLength - lXDistance);
        zeroTransform.position = parent.position + zeroPositionOffset;
        //print("after:" + zeroPositionOffset.y);
    }
}