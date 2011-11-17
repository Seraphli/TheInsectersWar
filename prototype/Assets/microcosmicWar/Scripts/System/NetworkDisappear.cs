using UnityEngine;

//在一段时间没有接受网络同步(appear)后,执行Disappear
public class NetworkDisappear:MonoBehaviour
{
    [SerializeField]
    float _disappearTime = 3f;

    public float disappearTime
    {
        get { return _disappearTime; }
        set
        {
            _disappearTime = value;
            disappearTimer.setInterval(disappearTime);
        }
    }

    public zzTimer disappearTimer;
    public Behaviour[] disenableWhenDisappear;

    [SerializeField]
    Life _life;

    public Life life
    {
        get { return _life; }
        set
        {
            removeLifeEndEvent();
            _life = value;
            setLifeEndEvent();
        }
    }

    System.Action disappearEvent;

    public void addDisappearEventReceiver(System.Action pReceiver)
    {
        disappearEvent -= zzUtilities.nullFunction;
        disappearEvent += pReceiver;
    }

    void Awake()
    {
        if (Network.isClient)
        {
            disappearTimer = gameObject.AddComponent<zzTimer>();
            disappearTimer.setInterval(disappearTime);
            disappearTimer.addImpFunction(disappear);
            setLifeEndEvent();
            disappearEvent = zzUtilities.nullFunction;
        }
        else
            Destroy(this);
    }

    void setLifeEndEvent()
    {
        if (_life)
            _life.addDieCallback(lifeEnd);
    }

    void removeLifeEndEvent()
    {
        if (_life)
            _life.removeDieCallback(lifeEnd);
    }

    void lifeEnd(Life pLife)
    {
        disappearTimer.enabled = false;
    }

    public void disappear()
    {
        disappearTimer.enabled = false;
        disappearEvent();
        foreach (var lScript in disenableWhenDisappear)
        {
            lScript.enabled = false;
        }
    }

    public void appear()
    {
        disappearTimer.timePos = 0f;
        if (!disappearTimer.enabled)
        {
            disappearTimer.enabled = true;
            foreach (var lScript in disenableWhenDisappear)
            {
                lScript.enabled = true;
            }
        }
    }
}