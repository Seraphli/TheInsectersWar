using UnityEngine;
using System.Collections;

public class zzObjectPicker:MonoBehaviour
{
    public delegate void ObjectCall(GameObject pObject);
    public delegate void VoidCall();

    static void nullObjectCall(GameObject pObject)
    { 
    }

    public LayerMask pickLayerMask;
    public float pickDistance = 1000.0f;

    public KeyCode button = KeyCode.Mouse0;

    public bool checkButton = true;

    public bool pickWhenDown;
    public bool pickWhenUp;

    public static Ray getMainCameraRay()
    {
        var lMousePos = Input.mousePosition;
        var lRay = Camera.main.ScreenPointToRay(
            new Vector3(lMousePos.x, lMousePos.y, Camera.main.nearClipPlane));
        return lRay;

    }

    static ObjectCall toObjectCall(VoidCall pVoidCall)
    {
        return (GameObject pObject) => pVoidCall();
    }


    public void addButtonDownObjectReceiver(ObjectCall pPickEvent)
    {
        buttonDownEvent += pPickEvent;
    }

    public void addButtonUpObjectReceiver(ObjectCall pPickEvent)
    {
        buttonUpEvent += pPickEvent;
    }

    public void addButtonUpVoidReceiver(VoidCall pPickEvent)
    {
        buttonUpEvent += toObjectCall(pPickEvent);
    }

    GameObject check()
    {
        RaycastHit lRaycastHit;
        if (Physics.Raycast(getMainCameraRay(), out lRaycastHit, pickDistance, pickLayerMask))
        {
            return lRaycastHit.collider.gameObject;
        }
        return null;

    }

    ObjectCall buttonDownEvent;
    ObjectCall buttonUpEvent;

    public delegate bool AblePickJudgeFunc();

    static bool nullAblePickJudgeFunc()
    {
        return true;
    }

    public AblePickJudgeFunc ableDownPickJudgeFunc = nullAblePickJudgeFunc;

    void Start()
    {
        if (buttonDownEvent==null)
            buttonDownEvent += nullObjectCall;


        if (buttonUpEvent == null)
            buttonUpEvent += nullObjectCall;

    }

    void Update()
    {
        if (ableDownPickJudgeFunc())
        {
            if (checkButton && Input.GetKeyDown(button))
                buttonDownEvent(pickWhenDown ? check() : null);

        }

        if (checkButton && Input.GetKeyUp(button))
            buttonUpEvent(pickWhenUp ? check() : null);

    }
}