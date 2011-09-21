using UnityEngine;

public class zzGUIModifierEatEvent: zzGUIModifierBase
{
    [SerializeField]
    bool _eatEvent = false;

    public bool eatEvent
    {
        get { return _eatEvent; }
        set { _eatEvent = value; }
    }

    public override void modifierBegin()
    {
        if (_eatEvent)
            Event.current.Use();
    }

    public override void modifierEnd(){}
}