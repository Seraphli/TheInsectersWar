using UnityEngine;

public class ColliderPickSwitcher:zzEditableObject
{
    public Transform meshParent;
    public GameObject shaper;
    public GameObject rigidbodyObject;

    void OnEnable()
    {
        shaper.active = objectContainer.draged;
    }

    public override void OnDragOn()
    {
        foreach (Transform lMesh in meshParent)
        {
            DestroyImmediate(lMesh.gameObject.GetComponent<MeshCollider>());
        }
        shaper.active = true;
        var lRigidbody = rigidbodyObject.AddComponent<Rigidbody>();
        lRigidbody.useGravity = false;
    }

    public override void OnDragOff()
    {
        DestroyImmediate(rigidbodyObject.GetComponent<Rigidbody>());
        shaper.active = false;
        foreach (Transform lMesh in meshParent)
        {
            lMesh.gameObject.AddComponent<MeshCollider>().sharedMesh
                = lMesh.GetComponent<MeshFilter>().sharedMesh;
        }
    }
}