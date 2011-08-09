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

    [System.Serializable]
    public class PickerInfo
    {
        public Camera camera;
        public LayerMask pickLayerMask;
    }

    public PickerInfo[] pickerInfos = new PickerInfo[0] { };

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

    public void addButtonDownVoidReceiver(VoidCall pPickEvent)
    {
        buttonDownEvent += toObjectCall(pPickEvent);
    }

    //是否为透明区域
    bool isClearSpace(Material pMaterial, Vector2 pUV)
    {
        var lTexture = pMaterial.mainTexture as Texture2D;
        if (!lTexture)
        {
            return false;
        }
        var lTextureOffset = pMaterial.mainTextureOffset;
        var lTextureScale = pMaterial.mainTextureScale;
        var lU = pUV.x * lTextureScale.x + lTextureOffset.x;
        var lV = pUV.y * lTextureScale.y + lTextureOffset.y;
        if (lTexture.GetPixelBilinear(lU, lV).a == 0f)
            return true;
        return false;

    }

    GameObject check()
    {
        RaycastHit lRaycastHit;

        var lMousePos = Input.mousePosition;

        foreach (var lInfo in pickerInfos)
        {
            var lRay = lInfo.camera.ScreenPointToRay(
                new Vector3(lMousePos.x, lMousePos.y, lInfo.camera.nearClipPlane));
            var lRaycastHits = Physics.RaycastAll(lRay, pickDistance, lInfo.pickLayerMask);
            System.Array.Sort(lRaycastHits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (var lHit in lRaycastHits)
            {
                var lTransform = lHit.transform;
                var lMeshRenderer = lTransform.GetComponent<MeshRenderer>();
                var lMeshCollider = lTransform.GetComponent<MeshCollider>();
                if (lMeshCollider
                    && lMeshRenderer)
                {
                    var lMaterial = lMeshRenderer.material;
                    if (lMaterial && isClearSpace(lMaterial, lHit.textureCoord))
                        continue;
                }
                return lHit.transform.gameObject;
            }
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

        if (pickerInfos.Length == 0)
            pickerInfos = new PickerInfo[] {
                new PickerInfo { camera = Camera.main, pickLayerMask=-1 } 
            };

    }

    void Update()
    {

        if (checkButton && Input.GetKeyDown(button) && ableDownPickJudgeFunc())
            buttonDownEvent(pickWhenDown ? check() : null);

        if (checkButton && Input.GetKeyUp(button))
            buttonUpEvent(pickWhenUp ? check() : null);

    }
}