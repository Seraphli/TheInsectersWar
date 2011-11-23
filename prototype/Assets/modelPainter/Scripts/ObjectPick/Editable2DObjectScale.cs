using UnityEngine;

public class Editable2DObjectScale:ObjectPickBase
{
    enum ScaleMode
    {
        none,
        nonUniform,
        uniform,
    }

    public float scaleValue = 0.01f;

    zzEditableObjectContainer editableObject;

    public Vector2 lastMousePosition;

    [SerializeField]
    ScaleMode scaleMode = ScaleMode.none;

    void Start()
    {
        if (scaleMode == ScaleMode.none)
            this.enabled = false;
    }

    public void endDrag()
    {
        if (scaleMode == ScaleMode.none)
            return;
        scaleMode = ScaleMode.none;
        editableObject.draged = false;
        this.enabled = false;
        if (editableObject.rigidbody)
            editableObject.rigidbody.detectCollisions = true;
    }

    void OnDragObject(GameObject pObject, ScaleMode pMode)
    {
        if (scaleMode != ScaleMode.none)
            return;
        var lEditableObject = zzEditableObjectContainer.findRoot(pObject);
        if (!lEditableObject)
            return;
        zzUndo.registerUndo(lEditableObject.transform);
        editableObject = lEditableObject;
        lEditableObject.draged = true;
        scaleMode = pMode;
        //放缩的尺寸基于原始的放缩值
        var lObjectScale = lEditableObject.transform.localScale;
        scaleValue = Mathf.Sqrt(lObjectScale.x * lObjectScale.x + lObjectScale.y * lObjectScale.y) * 0.01f;
        lastMousePosition = getMousePosition();
        this.enabled = true;
        if (editableObject.rigidbody)
            editableObject.rigidbody.detectCollisions = false;
    }

    Vector2 getMousePosition()
    {
        var lMousePosition = Input.mousePosition;
        return new Vector2(lMousePosition.x, lMousePosition.y);
    }

    float getScale(float pPosChange)
    {
        if (pPosChange > 0)
            return 1f + pPosChange;
        else if (pPosChange < 0)
            return 1f / (1 - pPosChange);
        return 1f;
    }

    Vector3 getScale(Vector2 pPosChange, ScaleMode pDragMode)
    {
        pPosChange *= scaleValue;

        switch (scaleMode)
        {
            case ScaleMode.nonUniform:
                return new Vector3((pPosChange.x), (pPosChange.y), 0f);
            case ScaleMode.uniform:
                {
                    var lScaleValue = pPosChange.x+pPosChange.y;
                    return new Vector3(lScaleValue, lScaleValue, 0f);
                }
        }
        Debug.LogError("dragMode error");
        return Vector3.one;

    }

    void FixedUpdate()
    {
        var lNewPos = getMousePosition();
        var lPosChange = lNewPos - lastMousePosition;
        switch (scaleMode)
        {
            case ScaleMode.nonUniform:
                lPosChange *= scaleValue;
                editableObject.transformScale(new Vector3((lPosChange.x), (lPosChange.y), 0f));
                break;
            case ScaleMode.uniform:
                editableObject.transformUniformScale((lPosChange.x + lPosChange.y) * 0.01f);
                break;
            default: 
                Debug.LogError("scaleMode error");
                break;
        }
        lastMousePosition = lNewPos;
    }

    public override void OnLeftOn(GameObject pObject)
    {
        OnDragObject(pObject, ScaleMode.nonUniform);
    }

    public override void OnLeftOff(GameObject pObject)
    {
        endDrag();
    }

    public override void OnRightOn(GameObject pObject)
    {
        OnDragObject(pObject, ScaleMode.uniform);
    }

    public override void OnRightOff(GameObject pObject)
    {
        endDrag();
    }
}