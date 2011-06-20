using UnityEngine;
using System.Collections;

public class EffectOfShield : MonoBehaviour
{
    public GameObject effectPrefab;
    public float shieldRadius;
    public float roll = 10f;    //转过一个角度
    public int filterLayer = 11;   //阻挡的子弹的层


    void OnTriggerEnter(Collider lOther)
    {
        if (lOther.gameObject.layer != filterLayer)
            return;
        var lCenterToOther = lOther.transform.position - transform.position;
        lCenterToOther.z = 0f;
        float lSign = lCenterToOther.y > 0f ? 1f : -1f;
        var lRotation = Quaternion.Euler(0f, roll, lSign * Vector3.Angle(Vector3.right, lCenterToOther));
        var lRayCollider = new Ray(transform.position, lCenterToOther);
        var lEffect = (GameObject)Instantiate(effectPrefab, lRayCollider.GetPoint(shieldRadius), lRotation);
        lEffect.transform.parent = transform;
    }
}
