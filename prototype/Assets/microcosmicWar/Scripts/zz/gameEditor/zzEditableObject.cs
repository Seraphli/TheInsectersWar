using UnityEngine;
using System.Collections;

public class zzEditableObject : MonoBehaviour
{
    public zzEditableObjectContainer objectContainer;

    public bool inPlaying
    {
        get { return GetComponent<zzEditableObjectContainer>().inPlaying; }
    }

    public virtual void applyPlayState()
    {
    }

    public virtual void applyPauseState()
    {
    }

    public virtual void OnDragOn()
    {
    }

    public virtual void OnDragOff()
    {
    }

    //public string objectTypeName;

    //protected virtual void applyState()
    //{
    //    if(_inPlaying)
    //    {
    //        rigidbody.isKinematic = _fixed;
    //        rigidbody.useGravity = _useGravity;
    //        rigidbody.constraints = rigidbodyConstraints;
    //    }
    //    else
    //    {
    //        rigidbody.isKinematic = true;
    //        rigidbody.useGravity = false;
    //        rigidbody.freezeRotation = false;
    //    }
    //}

    //public bool play
    //{
    //    get { return _inPlaying; }
    //    set
    //    {
    //        if (_inPlaying == value)
    //            return;
    //        _inPlaying = value;
    //        applyState();
    //    }
    //}

    //public bool draged
    //{
    //    set
    //    {
    //        if (value)
    //        {
    //            rigidbody.isKinematic = false;
    //        }
    //        else
    //            applyState();
    //    }
    //}

    //[zzSerialize]
    //[FieldUI("冻结旋转")]
    //public bool freezeRotation
    //{
    //    get { return _freezeRotation; }
    //    set 
    //    {
    //        _freezeRotation = value;
    //        if (_inPlaying)
    //        {
    //            rigidbody.freezeRotation = _freezeRotation;
    //            rigidbody.WakeUp();
    //        }
    //    }
    //}

    //[SerializeField]
    //GameObjectType _gameObjectType;

    //public GameObject[] objectList = new GameObject[0];


    protected virtual void Awake()
    {
        //_inPlaying = false;
        //applyState();
        applyPauseState();
        //setObjectType(_gameObjectType);
    }

    //protected void setObjectType(GameObjectType pType)
    //{
    //    int lLayer = GameConfig.Singleton.getLayer(pType);
    //    foreach (var lObject in objectList)
    //    {
    //        lObject.layer = lLayer;
    //    }
    //}

    public virtual InPoint[] inPoints
    {
        get { return new InPoint[0]; }
    }

    public virtual OutPoint[] outPoints
    {
        get { return new OutPoint[0]; }
    }


    protected void    setInPointsId()
    {
        int lID = -1;
        foreach (var lPoint in inPoints)
        {
            lPoint.pointId = ++lID;
        }
    }
}