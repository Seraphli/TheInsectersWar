using UnityEngine;

public class zzSpriteAsset:ScriptableObject
{
    //public zz.Sprite.AnimationInfo spriteInfo;
    public Texture2D image;
    public float length = 1f;
    public Vector2 leftTop = Vector2.zero;
    public Vector2 rightBottom = Vector2.one;
    public int xCount = 1;
    public int yCount = 1;
    public int beginIndex = 0;
    public int endIndex = 0;
    public bool loop;

    public int frameCount
    {
        get { return maxFramePos + 1; }
    }

    public int maxFramePos
    {
        get { return endIndex - beginIndex; }
    }

    public float frameLength
    {
        get { return length / frameCount; }
    }

    public float spriteWidth
    {
        get { return (rightBottom.x - leftTop.x) / xCount; }
    }

    public float spriteHeigth
    {
        get { return (rightBottom.y - leftTop.y) / yCount; }
    }

    public int getFrameIndex(float pTime)
    {
        int lFrameIndex = Mathf.FloorToInt(pTime / frameLength);
        if(loop)
            return lFrameIndex % frameCount + beginIndex;
        return Mathf.Min(lFrameIndex, maxFramePos);
    }

    public float getTimePosition(int pFramePos)
    {
        var lFramePos = pFramePos % frameCount;
        return lFramePos * frameLength;
    }

    //返回比例
    public Rect smapleFrameRect(int pFrameIndex)
    {
        var lXPos = pFrameIndex % xCount;
        var lYPos = pFrameIndex / xCount;
        var lWidth = spriteWidth;
        var lHeigth = spriteHeigth;
        return new Rect(lWidth * lXPos, lHeigth * lYPos, lWidth, lHeigth);
    }

    public Rect smapleTimeRect(float pTime)
    {
        return smapleFrameRect(getFrameIndex(pTime));
    }
}