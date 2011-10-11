using UnityEngine;

public class WMPurse:MonoBehaviour
{
    [SerializeField]
    int _number;

    //NetworkPlayer _owner;

    //public NetworkPlayer owner
    //{
    //    get { return _owner; }
    //    set 
    //    { 
    //        _owner = value; 
    //        if()
    //    }
    //}
    public HeroSpawn heroSpawn;

    public int number
    {
        get { return _number; }
        set
        {
            if (_number!=value)
            {
                _number = value;
                changedEvent();
            }
        }
    }

    public void cost(int pValue)
    {
        if (Network.isClient)
            networkView.RPC("WMPurseChange", RPCMode.Server, -pValue);
        else
            number -= pValue;
    }

    public void add(int pValue)
    {
        if (Network.isClient)
            networkView.RPC("WMPurseChange", RPCMode.Server, pValue);
        else
            number += pValue;
    }

    System.Action changedEvent;

    public void addChangedReceiver(System.Action pReceiver)
    {
        changedEvent += pReceiver;
    }

    static void nullReceiver() { }

    [RPC]
    void WMPurseChange(int pValue)
    {
        number += pValue;
    }

    [RPC]
    void WMPurseSetValue(int pValue)
    {
        number = pValue;
    }

    void setValue()
    {
        if (heroSpawn.owner != Network.player)
            networkView.RPC("WMPurseSetValue", heroSpawn.owner, _number);
    }

    void Start()
    {
        if (Network.isServer)
            addChangedReceiver(setValue);
        if (changedEvent == null)
            changedEvent = nullReceiver;
    }
}