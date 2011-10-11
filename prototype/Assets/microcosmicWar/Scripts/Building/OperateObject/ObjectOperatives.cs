using UnityEngine;

public class ObjectOperatives : MonoBehaviour
{
    public zzDetectorBase operateObjectDetector;

    [SerializeField]
    OperateObject _operatingObject;

    public OperateObject operatingObject
    {
        get { return _operatingObject; }
        set
        {
            if (value)
            {
                onOperateEvent();
            }
            else
            {
                offOperateEvent();
            }
            _operatingObject = value;
        }
    }

    public CommandControlBase commandControl;

    System.Action onOperateEvent;

    public void addOnOperateReceiver(System.Action pReceiver)
    {
        onOperateEvent += pReceiver;
    }

    System.Action offOperateEvent;

    public void addOffOperateReceiver(System.Action pReceiver)
    {
        offOperateEvent += pReceiver;
    }

    static void nullReceiver() { }


    void Start()
    {
        if (onOperateEvent == null)
            onOperateEvent = nullReceiver;
        if (offOperateEvent == null)
            offOperateEvent = nullReceiver;
    }

    public void tryOperate()
    {
        OperateObject lToOperateObject = null;
        var lColliders = operateObjectDetector.detect();
        foreach (var lCollider in lColliders)
        {
            var lOperateObject = lCollider.GetComponent<OperateObject>();
            if(lOperateObject)
            {
                if (lOperateObject.canOperate(this)
                    &&(lToOperateObject == null
                    || lToOperateObject.priority < lOperateObject.priority)
                    )
                    lToOperateObject = lOperateObject;
            }
        }
        if (lToOperateObject)
            lToOperateObject.operateThis(this);
    }

    public void releaseOperate()
    {
        if (_operatingObject)
            _operatingObject.endOperate(this);
        _operatingObject = null;
    }

    public void doOperate()
    {
        if (_operatingObject)
            releaseOperate();
        else
            tryOperate();
    }

    void OnDisable()
    {
        if (_operatingObject)
            releaseOperate();
    }
}