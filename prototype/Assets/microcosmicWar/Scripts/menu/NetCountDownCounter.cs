using UnityEngine;

public class NetCountDownCounter:MonoBehaviour
{
    public int timeLength = 5;
    public int timePos;
    //public zzTimer timer;
    public zzTimer secTimer;
    System.Action<string> messageSender;
    public string messageWhenOnCount = "游戏倒计时 {0}";
    public string messageWhenOffCount = "游戏倒计时中断";
    System.Action timeUpEvent;
    public void addTimeUpReceiver(System.Action pReceiver)
    {
        timeUpEvent += pReceiver;
    }

    public void addMessageReceiver(System.Action<string> pReceiver)
    {
        messageSender += pReceiver;
    }

    void Start()
    {
        secTimer = gameObject.AddComponent<zzTimer>();
        secTimer.setInterval(1f);
        secTimer.setImpFunction(second);
        secTimer.enabled = false;
    }

    void second()
    {
        ++timePos;
        messageSender((timeLength - timePos).ToString());
        if(timeLength == timePos)
        {
            secTimer.enabled = false;
            timeUpEvent();
        }
    }

    public void onCount()
    {
        if (!secTimer.enabled)
        {
            messageSender(zzUtilities.FormatString(messageWhenOnCount, timeLength));
            timePos = 0;
            secTimer.enabled = true;
            if(Network.isServer)
                networkView.RPC("NetCountDownCounterOnCount",RPCMode.Others);
        }
    }

    [RPC]
    public void NetCountDownCounterOnCount(NetworkMessageInfo pInfo)
    {
        onCount();
        secTimer.timePos = (float)(Network.time - pInfo.timestamp);
    }

    public void offCount()
    {
        if (secTimer.enabled)
        {
            secTimer.enabled = false;
            messageSender(messageWhenOffCount);
            if (Network.isServer)
                networkView.RPC("NetCountDownCounterOffCount", RPCMode.Others);
        }
    }

    [RPC]
    public void NetCountDownCounterOffCount()
    {
        offCount();
    }
}