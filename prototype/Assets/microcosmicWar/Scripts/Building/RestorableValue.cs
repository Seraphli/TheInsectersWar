using UnityEngine;

public class RestorableValue:MonoBehaviour
{
    System.Action<float> rateChangedEvent;

    public void addRateChangedEventReceiver(System.Action<float> pReceiver)
    {
        rateChangedEvent += pReceiver;
    }

    [SerializeField]
    int _nowValue = 5;

    public int nowValue
    {
        get { return _nowValue; }
        set 
        {
            _nowValue = Mathf.Clamp(value, 0, _fullValue);
            float lRate = (float)_nowValue / (float)_fullValue;
            rateChangedEvent(lRate);
            if(lRate==1f)
            {
                enabled = false;
                recoverValueResidue = 0f;
            }
            else
                enabled = true;
        }
    }

    [SerializeField]
    int _fullValue = 5;

    public int fullValue
    {
        get { return _fullValue; }
        set 
        {
            _nowValue = value;
            _fullValue = value; 
        }
    }

    public float restoreSpeed = 1f;

    [SerializeField]
    float recoverValueResidue = 0.0f;

    public void reduce(int pValue)
    {
        nowValue -= pValue;
        if(Network.isServer)
        {
            networkView.RPC("RPCRestorableValueChange", RPCMode.Others, nowValue);
        }
    }

    void RPCRestorableValueChange(int pValue)
    {
        nowValue = pValue;
    }

    void Update()
    {
        recoverValueResidue += Time.deltaTime * restoreSpeed;
        if(recoverValueResidue>1f)
        {
            int lRecoverValue = Mathf.FloorToInt(recoverValueResidue);
            recoverValueResidue -= lRecoverValue;
            nowValue += lRecoverValue;
        }
    }
}