using UnityEngine;
using System.Collections;

public class ActionEnergyValue : MonoBehaviour
{
    public float fullValue;

    [SerializeField]
    float _nowValue;

    public float nowValue
    {
        get { return _nowValue; }
        set
        {
            _nowValue = Mathf.Clamp(value, 0f, fullValue);
            if (_nowValue == fullValue)
            {
                enabled = false;
            }
            else
                enabled = true;
            valueChangedEvent(_nowValue);
        }
    }

    public float recoverSpeed;

    System.Action<float> valueChangedEvent;

    static void nullValueChangedReceiver(float p){}

    public void addValueChangedReceiver(System.Action<float> pReceiver)
    {
        valueChangedEvent += pReceiver;
    }

    void Start()
    {
        if (valueChangedEvent == null)
            valueChangedEvent = nullValueChangedReceiver;
    }

    //void use(float pCost)
    //{
    //    nowValue -= pCost;
    //}

    public bool tryUse(float pCost)
    {
        if(nowValue >= pCost)
        {
            nowValue -= pCost;
            return true;
        }
        return false;
    }

    void Update()
    {
        nowValue += recoverSpeed * Time.deltaTime;
    }
}