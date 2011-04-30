using UnityEngine;

public class SoldierEvent:MonoBehaviour
{
    System.Action onFireEvent;
    System.Action offFireEvent;

    public ActionCommandControl actionCommandControl;

    [SerializeField]
    bool _inFiring = false;

    void Start()
    {
        if (onFireEvent == null)
            onFireEvent = zzUtilities.nullFunction;

        if (offFireEvent == null)
            offFireEvent = zzUtilities.nullFunction;

        if (!actionCommandControl)
            actionCommandControl = GetComponent<ActionCommandControl>();
    }

    void Update()
    {
        inFiring = actionCommandControl.getCommand().Fire;
    }

    public bool inFiring
    {
        get
        {
            return _inFiring;
        }
        set
        {
            if(_inFiring!=value)
            {
                _inFiring = value;
                if (_inFiring)
                    onFireEvent();
                else
                    offFireEvent();
            }
        }
    }

    public void addOnFireEventReceiver(System.Action pReceiver)
    {
        onFireEvent += pReceiver;
    }

    public void addOffFireEventReceiver(System.Action pReceiver)
    {
        offFireEvent += pReceiver;
    }

}