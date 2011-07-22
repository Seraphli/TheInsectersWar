using UnityEngine;

public class LifeIntervalChange:MonoBehaviour
{
    public float recoverInterval = 0.1f;

    public int changeValueEveryTime = 1;

    public Life life;

    public zzTimer recoverTimer;

    //public event System.Action onRecoverStayEvent;
    //public event System.Action offRecoverStayEvent;

    void Awake()
    {
        if (!recoverTimer)
        {
            recoverTimer = gameObject.AddComponent<zzTimer>();
        }
        recoverTimer.setInterval(recoverInterval);
        recoverTimer.addImpFunction(change);
    }

    void Start()
    {
        if(!life)
            life = transform.parent.GetComponent<Life>();
    }

    void change()
    {
        if (zzCreatorUtility.isHost())
        {
            life.injure(-changeValueEveryTime);
        }
    }
}