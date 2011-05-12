

using UnityEngine;
using System.Collections;

public class FireSpark : MonoBehaviour
{
    #region IAbleSetForward 成员

    public void setForward(float pAngle)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, pAngle);
    }

    public void setForward(Vector3 pForward)
    {
        pForward.z = 0;
        float lSign = pForward.y > 0f ? 1f : -1f;
        setForward(Vector3.Angle(Vector3.right, pForward) * lSign);
    }

    public Vector3 getForward()
    {
        return transform.rotation * Vector3.right;
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + getForward() * 3);
    }
}