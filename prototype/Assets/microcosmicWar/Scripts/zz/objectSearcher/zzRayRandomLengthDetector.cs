using UnityEngine;
using System.Collections;

public class zzRayRandomLengthDetector : zzDetectorBase
{

    public Vector3 direction;

    public Vector3 worldDirection
    {
        get { return transform.TransformDirection(direction); }
    }

    //检测距离将在介于与以下值之间
    public float lengthMin;
    public float lengthMax;

    //为了使行为不同,范围也取随机值
    public float lengthMinRandomMin = 0.0f;
    public float lengthMinRandomMax = 5.0f;

    public float lengthMaxRandomMin = 5.0f;
    public float lengthMaxRandomMax = 10.0f;


    void Awake()
    {
        lengthMin = Random.Range(lengthMinRandomMin, lengthMinRandomMax);
        lengthMax = Random.Range(lengthMaxRandomMin, lengthMaxRandomMax);
    }

    public override RaycastHit[] _impDetect(LayerMask pLayerMask)
    {
        return Physics.RaycastAll(transform.position, worldDirection, getLength(), pLayerMask);
    }

    float getLength()
    {
        return Random.Range(lengthMin, lengthMax);
    }

    void OnDrawGizmos()
    {
        DrawGizmos(Color.blue, Color.yellow);
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmos(Color.black, Color.white);
    }

    void DrawGizmos(Color fixedAreaColor, Color randomAreaColor)
    {
        Gizmos.color = fixedAreaColor;
        var lBeginPos = transform.position;
        var lWorldNormalizedDirection =  worldDirection.normalized;
        if (Application.isPlaying)
        {
            var lMinLengthPos = lBeginPos + lWorldNormalizedDirection * lengthMax;
            var lMaxLengthPos = lBeginPos + lWorldNormalizedDirection * lengthMin;
            Gizmos.DrawLine(lBeginPos, lMinLengthPos);
            Gizmos.color = randomAreaColor;
            Gizmos.DrawLine(lMinLengthPos, lMaxLengthPos);
        }
        else
        {
            var lLengthMinRandomMinPos
                = lBeginPos + lWorldNormalizedDirection * lengthMinRandomMin;

            var lLengthMinRandomMaxPos
                = lBeginPos + lWorldNormalizedDirection * lengthMinRandomMax;

            var lLengthMaxRandomMinPos
                = lBeginPos + lWorldNormalizedDirection * lengthMaxRandomMin;

            var lLengthMaxRandomMaxPos
                = lBeginPos + lWorldNormalizedDirection * lengthMaxRandomMax;
            Gizmos.DrawLine(lBeginPos, lLengthMinRandomMinPos);
            Gizmos.color = randomAreaColor;
            Gizmos.DrawLine(lLengthMinRandomMinPos, lLengthMinRandomMaxPos);
            Gizmos.color = fixedAreaColor;
            Gizmos.DrawLine(lLengthMinRandomMaxPos, lLengthMaxRandomMinPos);
            Gizmos.color = randomAreaColor;
            Gizmos.DrawLine(lLengthMaxRandomMinPos, lLengthMaxRandomMaxPos);

        }
    }

}