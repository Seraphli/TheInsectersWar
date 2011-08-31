using UnityEngine;
using System.Collections;

public class PlayStateManager:MonoBehaviour
{

    public delegate void PlayStateChangedFunc();

    public void nullPlayStateChangedFunc() { }

    public event PlayStateChangedFunc changedToPlayEvent;

    protected void doChangedToPlayEvent()
    {
        changedToPlayEvent();
    }

    public event PlayStateChangedFunc changedToStopEvent;

    protected void doChangedToStopEvent()
    {
        changedToStopEvent();
    }


    [SerializeField]
    protected bool _inPlaying = false;

    public IEnumerable enumerateObject;

    public Transform ToInitState;

    public void Start()
    {
        if(ToInitState)
        {
            var lInitList = ToInitState.GetComponentsInChildren<zzEditableObjectContainer>();
            foreach (var lEditableObject in lInitList)
            {
                lEditableObject.play = _inPlaying;
            }
        }
        if (changedToPlayEvent == null)
            changedToPlayEvent = nullPlayStateChangedFunc;
        if (changedToStopEvent == null)
            changedToStopEvent = nullPlayStateChangedFunc;
    }

    public bool play
    {
        get { return _inPlaying; }
        set 
        {
            setPlay(value);
        }
    }

    public virtual void setPlay(bool pIsPlay)
    {
        if (_inPlaying == pIsPlay)
            return;
        _inPlaying = pIsPlay;
        if (pIsPlay)//stop=>play
        {
            applyPlayState();
            changedToPlayEvent();
        }
        else//play=>stop
        {
            applyStopState();
            changedToStopEvent();
        }
    }

    public virtual void applyPlayState()
    {
        updateObjects();
    }

    public virtual void applyStopState()
    {
        updateObjects();
    }

    public virtual void updateObject(GameObject pOjbect)
    {
        pOjbect.GetComponent<zzEditableObjectContainer>().play = _inPlaying;
    }

    public void updateObjects()
    {
        foreach (Transform lTransform in enumerateObject)
        {
            lTransform.GetComponent<zzEditableObjectContainer>().play = _inPlaying;
        }
    }
}