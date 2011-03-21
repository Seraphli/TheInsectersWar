using UnityEngine;
using System.Collections;

public class zzEditableObjectContainer:MonoBehaviour
{
    [SerializeField]
    bool _inPlaying = false;

    public bool inPlaying
    {
        get { return _inPlaying; }
    }

    //public string objectTypeName;

    public zzEditableObject[] editableObjectList;

    protected void applyState()
    {
        if (_inPlaying)
        {
            foreach (var lObject in editableObjectList)
            {
                lObject.applyPlayState();
            }
        }
        else
        {
            foreach (var lObject in editableObjectList)
            {
                lObject.applyPauseState();
            }
        }
    }

    public bool play
    {
        get { return _inPlaying; }
        set
        {
            if (_inPlaying == value)
                return;
            _inPlaying = value;
            applyState();
        }
    }

    public bool draged
    {
        set
        {
            if (value)
            {
                foreach (var lObject in editableObjectList)
                {
                    lObject.OnDragOn();
                }
            }
            else
                applyState();
        }
    }

    void Awake()
    {
        editableObjectList = GetComponents<zzEditableObject>();
        //foreach (var lObject in editableObjectList)
        //{
        //    lObject.objectContainer = this;
        //}
    }

    [SerializeField]
    bool canScale = true;
    [SerializeField]
    bool _uniformScale;

    public bool uniformScale
    {
        get { return _uniformScale; }
    }

    public void transformScale(Vector3 pScaleChange)
    {
        if(!canScale)
        {
            return;
        }

        var lLocalScale = transform.localScale;
        Vector3 lScale;
        if (_uniformScale)
        {
            float lLength = (pScaleChange.x > 0 ? 1f : -1f) * pScaleChange.magnitude;
            float lUniformValue = lLocalScale.x + lLength;
            lScale = new Vector3(lUniformValue, lUniformValue, lUniformValue);
        }
        else
            lScale = lLocalScale + pScaleChange;
        transform.localScale = lScale;
    }


    public static zzEditableObjectContainer findRoot(GameObject pObject)
    {
        if (!pObject)
            return null;
        var lEditable = pObject.GetComponent<zzEditableObjectContainer>();
        Transform lTransform = pObject.transform.parent;
        while (lTransform)
        {
            var lParentEditable = lTransform.GetComponent<zzEditableObjectContainer>();
            if (lParentEditable)
                lEditable = lParentEditable;
            lTransform = lTransform.parent;
        }
        if (lEditable)
            return lEditable;
        return null;
    }
}