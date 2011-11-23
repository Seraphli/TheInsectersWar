using UnityEngine;

//[ExecuteInEditMode]
public class zzGUIBubbleIconMessage:zzInterfaceGUI
{
    public zzGUIBubbleComputeRect getBubble(Transform pBoxPosition)
    {
        foreach (Transform lSub in transform)
        {
            var lSunMessageBox = lSub.GetComponent<zzGUIBubbleComputeRect>();
            if (lSunMessageBox && lSunMessageBox.bubblePosition == pBoxPosition)
            {
                return lSunMessageBox;
            }
        }
        return null;
    }

    public override void impGUI(Rect pRect)
    {
        drawBubble( pRect );
    }

    public void drawBubble(Rect pRect)
    {
        foreach (Transform lSub in transform)
        {
            var lBubble = lSub.GetComponent<zzGUIBubbleComputeRect>();
            if (lBubble)
            {
                lBubble.bubbleBound = pRect;
                lBubble.drawBubble();
            }
        }
    }

    //之前不存在,则返回真
    public bool addBubble(Transform pTransform, GameObject pBubblePrefab, out zzGUIBubbleComputeRect lBubble)
    {
        lBubble = getBubble(pTransform);
        if(!lBubble)
        {
            var lBubbleObject = (GameObject)Object.Instantiate(pBubblePrefab);
            lBubbleObject.transform.parent = transform;
            lBubble = lBubbleObject.GetComponent<zzGUIBubbleComputeRect>();
            lBubble.bubblePosition = pTransform;
            return true;
        }
        return false;
    }

    public zzGUIBubbleComputeRect addBubble(GameObject pBubblePrefab)
    {
        var lBubbleObject = (GameObject)Object.Instantiate(pBubblePrefab);
        lBubbleObject.transform.parent = transform;
        return lBubbleObject.GetComponent<zzGUIBubbleComputeRect>();
    }

    public zzGUIBubbleComputeRect addBubble(Transform pTransform,GameObject pBubblePrefab)
    {
        zzGUIBubbleComputeRect lBubble;
        lBubble = getBubble(pTransform);
        if(!lBubble)
        {
            var lBubbleObject = (GameObject)Object.Instantiate(pBubblePrefab);
            lBubbleObject.transform.parent = transform;
            lBubble = lBubbleObject.GetComponent<zzGUIBubbleComputeRect>();
            lBubble.bubblePosition = pTransform;
        }
        return lBubble;
    }

}