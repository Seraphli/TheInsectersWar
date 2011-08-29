using UnityEngine;

public class zzMaterialSpritePlayer:MonoBehaviour
{
    public zzSpriteAsset sprite;
    public float timePos;
    
    [SerializeField]
    int framePos;

    void Awake()
    {
        renderer.material.mainTexture = sprite.image;
        framePos = sprite.getFrameIndex(timePos);
        smaple();
    }

    void Update()
    {
        timePos = sprite.moveTime(timePos, Time.deltaTime);
        var lNewFramePos = sprite.getFrameIndex(timePos);
        if(lNewFramePos!=framePos)
        {
            framePos = lNewFramePos;
            smaple();
        }
    }

    public void smaple()
    {
        smaple(renderer.material);
    }

    public void smaple(Material pMaterial)
    {
        var lRect = sprite.smapleFrameRect(framePos);
        pMaterial.mainTextureOffset = new Vector2(lRect.x, 1f - lRect.y - lRect.height);
        pMaterial.mainTextureScale = new Vector2(lRect.width, lRect.height);
    }

    void OnDrawGizmosSelected()
    {
        if(renderer
            && renderer.sharedMaterial
            && sprite
            && sprite.image)
        {
            renderer.sharedMaterial.mainTexture = sprite.image;
            framePos = sprite.getFrameIndex(timePos);
            smaple(renderer.sharedMaterial);
        }
    }
}