using UnityEngine;

public class doByRace : MonoBehaviour
{
    public System.Action _pismireEvent;
    public System.Action _beeEvent;


    public bool implementWhenAwake = true;

    public void pismireEvent(System.Action pReceiver)
    {
        if (implementWhenAwake
            && GameScene.Singleton.playerInfo.race == Race.ePismire)
            pReceiver();
        _pismireEvent += pReceiver;
    }

    public void beeEvent(System.Action pReceiver)
    {
        if (implementWhenAwake
            && GameScene.Singleton.playerInfo.race == Race.eBee)
            pReceiver();
        _beeEvent += pReceiver;
    }

    public void imp()
    {
        switch(GameScene.Singleton.playerInfo.race)
        {
            case Race.eBee:
                _beeEvent();
                break;
            case Race.ePismire:
                _pismireEvent();
                break;
            case Race.eNone: break;
        }
    }
}