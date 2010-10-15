

using UnityEngine;
using System.Collections;

public class FireSpark : MonoBehaviour
{
    #region IAbleSetForward 成员

    public void setForward(Vector3 pForward)
    {
        pForward.z = 0;
        Quaternion lRotation = new Quaternion();
        lRotation.SetFromToRotation(Vector3.right, pForward);
        //transform.rotation.SetFromToRotation(Vector3.right,pForward);
        transform.rotation = lRotation;
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