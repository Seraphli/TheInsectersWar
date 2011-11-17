using UnityEngine;

public class zzGUIBubbleMessage:MonoBehaviour
{
    public zzGUIBubbleMessageBox getMessageBox(Transform pBoxPosition)
    {
        foreach (Transform lSub in transform)
        {
            var lSunMessageBox = lSub.GetComponent<zzGUIBubbleMessageBox>();
            if (lSunMessageBox && lSunMessageBox.boxPosition == pBoxPosition)
            {
                return lSunMessageBox;
            }
        }
        return null;
    }
    public int guiRenderDepth = 1;
    public GUIStyle labelStyle = new GUIStyle();
    public Rect defaultRect;
    public Color deafultColor = Color.white;
    public float fadeOutBeginPos = 5f;
    public float fadeOutEndPos = 7f;

    public void OnGUI()
    {
        GUI.depth = guiRenderDepth;
        drawMessage();
    }

    public void drawMessage()
    {
        foreach (Transform lSub in transform)
        {
            var lMessageBox = lSub.GetComponent<zzGUIBubbleMessageBox>();
            if (lMessageBox)
            {
                lMessageBox.drawMessage();
            }
        }
    }

    public void addMessage(string pText, Transform pTransform)
    {
        addMessage(pText, pTransform, deafultColor, defaultRect);
    }

    public void addMessage(string pText, Transform pTransform,Color pColor)
    {
        addMessage(pText, pTransform, pColor,defaultRect);
    }

    public void addMessage(string pText,Transform pTransform,
        Color pColor, Rect pRect)
    {
        zzGUIBubbleMessageBox lMessageBox = getMessageBox(pTransform);
        if(!lMessageBox)
        {
            var lNewMessageBoxObject = new GameObject();
            var lNewMessageBoxTransform = lNewMessageBoxObject.transform;
            lNewMessageBoxTransform.parent = transform;
            lNewMessageBoxTransform.localPosition= Vector3.zero;
            lNewMessageBoxTransform.localRotation = Quaternion.identity;
            lMessageBox = lNewMessageBoxObject.AddComponent<zzGUIBubbleMessageBox>();
            lMessageBox.boxPosition = pTransform;
            lMessageBox.style = labelStyle;
        }
        lMessageBox.text = pText;
        lMessageBox.color = pColor;
        lMessageBox.rect = pRect;
        lMessageBox.timestamp = Time.realtimeSinceStartup;
        lMessageBox.fadeOutBeginPos = fadeOutBeginPos;
        lMessageBox.fadeOutEndPos = fadeOutEndPos;
    }
}