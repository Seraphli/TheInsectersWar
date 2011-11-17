using UnityEngine;

public class zzGUIBubbleComputeRect:MonoBehaviour
{
    public Transform bubblePosition;
    public zzGUIBubbleLayout bubbleLayout;
    public Rect bubbleRect;
    public Rect bubbleBound;
    public enum Dock
    {
        top,bottom,left,right,
    }
    public Dock dock;

    bool boundIntersect(Rect lBubbleCenterBound, ref Vector2 lBubbleCenter)
    {
        if (lBubbleCenter.x < lBubbleCenterBound.xMin
            || lBubbleCenter.x > lBubbleCenterBound.xMax
            || lBubbleCenter.y < lBubbleCenterBound.yMin
            || lBubbleCenter.y > lBubbleCenterBound.yMax)
        {
            var lBoundCenter = new Vector2(
                lBubbleCenterBound.x + lBubbleCenterBound.width / 2f,
                lBubbleCenterBound.y + lBubbleCenterBound.height / 2f);
            var lBoundToBubbleCenter = lBubbleCenter - lBoundCenter;
            var lBoundK = lBubbleCenterBound.height / lBubbleCenterBound.width;
            var lK = lBoundToBubbleCenter.y / lBoundToBubbleCenter.x;
            float lX;
            float lY;
            //上
            //(lBoundToBubbleCenter.y<0&&(|lK|>lBoundK))
            //右
            //(lBoundToBubbleCenter.x>0&&(|lK|<lBoundK))
            //下
            //(lBoundToBubbleCenter.y>0&&(|lK|>lBoundK))
            //左
            //(lBoundToBubbleCenter.x<0&&(|lK|<lBoundK))
            if (Mathf.Abs(lK) < lBoundK)
            {
                //左或右
                if (lBoundToBubbleCenter.x > 0)
                {
                    //右
                    lX = lBubbleCenterBound.xMax;
                    dock = Dock.right;
                }
                else
                {
                    //左
                    lX = lBubbleCenterBound.xMin;
                    dock = Dock.left;
                }
                lY = lK * (lX - lBoundCenter.x) + lBoundCenter.y;
            }
            else
            {
                //上或下
                if (lBoundToBubbleCenter.y < 0)
                {
                    //上
                    lY = lBubbleCenterBound.yMin;
                    dock = Dock.top;
                }
                else
                {
                    //下
                    lY = lBubbleCenterBound.yMax;
                    dock = Dock.bottom;
                }
                lX = (lY - lBoundCenter.y) / lK + lBoundCenter.x;
            }
            lBubbleCenter = new Vector2(lX, lY);
            return true;
        }
        return false;
    }

    public bool showInsideBound = true;

    public void drawBubble()
    {
        if (bubblePosition)
        {
            var lRect = bubbleRect;
            var lScreenPoint = Camera.main.WorldToScreenPoint(bubblePosition.position);
            lRect.x += lScreenPoint.x;
            lRect.y += Screen.height - lScreenPoint.y;

            if (showInsideBound)
            {
                var lBubbleCenter = new Vector2(lRect.x + lRect.width / 2f,
                    lRect.y + lRect.height / 2f);
                //气泡中心点,在屏幕的范围
                var lBubbleCenterBound = new Rect(
                    bubbleBound.x + lRect.width / 2f,
                    bubbleBound.y + lRect.height / 2f,
                    bubbleBound.width - lRect.width,
                    bubbleBound.height - lRect.height);
                if (boundIntersect(lBubbleCenterBound, ref lBubbleCenter))
                {
                    lRect.x = lBubbleCenter.x - lRect.width / 2f;
                    lRect.y = lBubbleCenter.y - lRect.height / 2f;
                }

            }
            //if (bubbleBound.Contains())
            bubbleLayout.impGUI(lRect);
        }
        else
            Destroy(gameObject);
    }
}
