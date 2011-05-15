using UnityEngine;

public class BatteryValueShow:MonoBehaviour
{
    public int segmentationNum = 10;

    [SerializeField]
    float _rate;

    public float rate
    {
        get { return _rate; }
        set
        {
            _rate = value;
            //base.rate = Mathf.Floor(value / (1f / segmentationNum)) * 1f / segmentationNum;
            setShowRate(Mathf.Floor(value * segmentationNum) / segmentationNum);
        }
    }

    public zzPlaneMesh planeMesh = new zzPlaneMesh();


    void setShowRate(float pRate)
    {
        planeMesh.UVs[3].y = pRate;
        planeMesh.UVs[2].y = pRate;


        planeMesh.vertices[3].y = pRate;
        planeMesh.vertices[2].y = pRate;

        planeMesh.UpdateMesh();
    }

    void Awake()
    {
        planeMesh.Init(gameObject);
        rate = _rate;
    }

    //在编辑模式下显示
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            if (!GetComponent<MeshFilter>()
                || !GetComponent<MeshFilter>().sharedMesh)
                Awake();
            rate = _rate;
        }
    }
}