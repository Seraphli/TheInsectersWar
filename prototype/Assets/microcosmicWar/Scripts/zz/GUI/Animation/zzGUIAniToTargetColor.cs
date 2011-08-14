using UnityEngine;
using System.Collections;

public class zzGUIAniToTargetColor : MonoBehaviour
{
    public zzGUIModifierColor colorGUI;

    System.Action clearEvent;

    static void nullClearEventReceiver(){}

    public void addClearEventReceiver(System.Action pReceiver)
    {
        clearEvent += pReceiver;
    }

    Color nowColor
    {
        set { colorGUI.color = value; }
        get { return colorGUI.color; }
    }

    public Color targetColor;
    public float duration;

    [SerializeField]
    Color originalColor;
    [SerializeField]
    float beginTime;

    void Start()
    {
        if (clearEvent == null)
            clearEvent = nullClearEventReceiver;
        transitToColor(targetColor);
    }

    public void transitToColor(Color pColor)
    {
        beginTime = Time.realtimeSinceStartup;
        originalColor = nowColor;
        targetColor = pColor;
        enabled = true;
    }

    void Update()
    {
        float lDeltaTime = Time.realtimeSinceStartup - beginTime;
        if (lDeltaTime >= duration)
        {
            nowColor = targetColor;
            enabled = false;
            if (targetColor.a == 0f)
                clearEvent();
        }
        else
        {
            nowColor = Color.Lerp(originalColor, targetColor, lDeltaTime / duration);
        }
    }
}