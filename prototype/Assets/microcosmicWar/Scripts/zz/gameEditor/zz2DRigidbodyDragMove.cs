using UnityEngine;

public class zz2DRigidbodyDragMove:MonoBehaviour
{
    public Joint jointDrag;
    public bool detectCollisions = true;

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
            jointDrag.connectedBody.detectCollisions = true;
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
        jointDrag.connectedBody.detectCollisions = detectCollisions;
    }


    void FixedUpdate()
    {
        jointDrag.transform.position = getXYWantPos();
    }

}