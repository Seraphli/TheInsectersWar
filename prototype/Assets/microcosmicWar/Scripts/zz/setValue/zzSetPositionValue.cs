using UnityEngine;

public class zzSetPositionValue:MonoBehaviour
{
    [SerializeField]
    Transform source;

    public void addReceiver(System.Action<Vector3> pSetFunc)
    {
        setFunc += pSetFunc;
    }

    System.Action<Vector3> setFunc;

    public void setValue()
    {
        print(source.position);
        setFunc(source.position);
    }
}