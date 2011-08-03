using UnityEngine;

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

    void Awake()
    {
        if (Network.isClient)
        {
            disappearTimer = gameObject.AddComponent<zzTimer>();
            disappearTimer.setInterval(disappearTime);
            disappearTimer.addImpFunction(disappear);
            setLifeEndEvent();
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