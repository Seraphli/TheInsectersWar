using UnityEngine;

public class zz2DRigidbodyDragMove:MonoBehaviour
{
    public Joint jointDrag;

    Vector3 getXYWantPos()
    {
        var lOut = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lOut.z = 0f;
        return lOut;
    }

    public void OnDragEnd()
    {
        if (enabled)
        {
            jointDrag.connectedBody = null;
            editableObject.draged = false;
            editableObject = null;
            enabled = false;

        }
    }

    zzEditableObjectContainer editableObject;

    public void OnDragStart(GameObject pObject)
    {
        var lEditableObject = zzEditableObjectContainer.findRoot(pObject);
        if (!lEditableObject)
            return;
        enabled = true;

        editableObject = lEditableObject;
        lEditableObject.draged = true;

        jointDrag.transform.position = getXYWantPos();
        jointDrag.connectedBody = lEditableObject.rigidbody;
    }


    void FixedUpdate()
    {
        jointDrag.transform.position = getXYWantPos();
    }

}