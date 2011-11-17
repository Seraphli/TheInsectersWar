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

    void Awake()
    {
        var lMaterial = sourceMaterialRenderer.material;
        foreach (var lRenderer in renderers)
        {
            lRenderer.material = lMaterial;
        }
    }
}