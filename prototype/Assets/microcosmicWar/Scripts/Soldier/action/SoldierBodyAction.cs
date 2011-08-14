using UnityEngine;

public class SoldierBodyAction:SoldierAction
{
    public BodyAction bodyAction;

    public string onActionAnimationName;
    public string offActionAnimationName;

    //动作持续的时间
    public float actionLength;

    //动画持续的时间,在这时间后执行playAction(offActionAnimationName);
    public float actionAnimationLength;

    public zzTimer actionAnimationEndTimer;

    [System.Serializable]
    public class TimeAction
    {
        public float time;

        public zzOnAction action;

        public zzTimer timer
        {
            set 
            { 
                _timer = value;
                _timer.setInterval(time);
                _timer.addImpFunction(imp);
                _timer.enabled = false;
            }
        }

        [SerializeField]
        zzTimer _timer;

        public void OnTimer()
        {
            _timer.enabled = true;
            _timer.timePos = 0;
        }

        public void OffTimer()
        {
            _timer.enabled = false;
        }

        void imp()
        {
            action.impAction();
            _timer.enabled = false;
        }
    }

    public TimeAction[] timeAction = new TimeAction[]{};

    void Awake()
    {
        base.Awake();
        timer.setInterval(actionLength);
        foreach (var lTimeAction in timeAction)
        {
            lTimeAction.timer = gameObject.AddComponent<zzTimer>();
        }
        actionAnimationEndTimer = gameObject.AddComponent<zzTimer>();
        actionAnimationEndTimer.setInterval(actionAnimationLength);
        actionAnimationEndTimer.setImpFunction(actionAnimationEnd);
        actionAnimationEndTimer.enabled = false;
    }

    void actionAnimationEnd()
    {
        bodyAction.playAction(offActionAnimationName);
        actionAnimationEndTimer.enabled = false;
    }

    public override void OnActionStart()
    {
        _inActing = true;

        timer.enabled = true;
        timer.timePos = 0f;

        actionAnimationEndTimer.enabled = true;
        actionAnimationEndTimer.timePos = 0f;

        foreach (var lTimeAction in timeAction)
        {
            lTimeAction.OnTimer();
        }
        bodyAction.playAction(onActionAnimationName);
    }

    public override void OnActionEnd()
    {
        base.OnActionEnd();
        foreach (var lTimeAction in timeAction)
        {
            lTimeAction.OffTimer();
        }
        actionAnimationEnd();
    }

}