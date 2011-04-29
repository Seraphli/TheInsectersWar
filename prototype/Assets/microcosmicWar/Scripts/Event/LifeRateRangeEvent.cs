using UnityEngine;

public class LifeRateRangeEvent:MonoBehaviour
{
    public float minRange;
    public float maxRange;

    public bool isOn = false;

    System.Action onRangeEvent;
    public void addOnRangeEventReceiver(System.Action pReceiver)
    {
        onRangeEvent += pReceiver;
    }

    System.Action offRangeEvent;
    public void addOffRangeEventReceiver(System.Action pReceiver)
    {
        offRangeEvent += pReceiver;
    }

    public Life life;

    void Start()
    {
        if (onRangeEvent == null)
            onRangeEvent = zzUtilities.nullFunction;
        if (offRangeEvent == null)
            offRangeEvent = zzUtilities.nullFunction;

        life.addBloodValueChangeCallback(lifeChangedFunc);
    }


    void lifeChangedFunc(Life pLife)
    {
        bool lIsOn = false; ;
        float lLifeRate = pLife.getRate();
        if(minRange<=lLifeRate&&lLifeRate<=maxRange)
        {
            lIsOn = true;
        }
        if(isOn)
        {
            if(!lIsOn)
            {
                isOn = lIsOn;
                offRangeEvent();
            }
        }
        else
        {
            if (lIsOn)
            {
                isOn = lIsOn;
                onRangeEvent();
            }

        }
    }
}