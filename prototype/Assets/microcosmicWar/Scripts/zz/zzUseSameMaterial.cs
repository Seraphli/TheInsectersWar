using UnityEngine;

public class zzUseSameMaterial:MonoBehaviour
{
    public Renderer sourceMaterialRenderer;

    public Renderer[] renderers;

    [ContextMenu("findChildrenRenderers")]
    void findChildrenRenderers()
    {
        renderers = gameObject.GetComponentsInChildren<Renderer>();
    }

    public Material objectMaterial
    {
        get { return sourceMaterialRenderer.material; }
    }

    void Awake()
    {
        var lMaterial = objectMaterial;
        foreach (var lRenderer in renderers)
        {
            lRenderer.material = lMaterial;
        }
    }
}