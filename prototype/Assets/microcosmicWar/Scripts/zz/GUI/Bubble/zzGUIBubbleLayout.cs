using UnityEngine;

public class zzGUIBubbleLayout : zzInterfaceGUI
{
    //public Rect bubbleBound;
    public zzGUILayoutBase[] bubbleDrawList = new zzGUILayoutBase[]{};

    //[SerializeField]
    //float _showTime = 5f;
    public float showTime
    {
        get { return timer.interval; }
        set
        {
            timer.setInterval(value);
        }
    }

    public float timePostion
    {
        set
        {
            timer.timePos = value;
        }
    }

    public zzTimer timer;
    //public void drawBubbleLayout(Rect pBubbleRect)
    //{

    //}
    //public zzGUIDockPos horizontalDockPosition = zzGUIDockPos.custom;
    //public zzGUIDockPos verticalDockPosition = zzGUIDockPos.custom;
    void Awake()
    {
        timer.setImpFunction(()=>Destroy(gameObject));
    }

    void drawBubble(Rect pRect)
    {
        foreach (var lbubbleDraw in bubbleDrawList)
        {
            GUILayout.BeginArea(pRect);
            lbubbleDraw.impGUI();
            GUILayout.EndArea();
        }
    }

    public override void impGUI(Rect pRect)
    {
        drawBubble(pRect);
    }
}