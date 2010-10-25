using UnityEngine;
using System.Collections;

//按时间间隔创建物体,未测试
public class zzCreateObjectInTime : MonoBehaviour
{
    public GameObject objectToCreate;
    public float interval;
    public zzTimer timer;
    public Transform placeToCreate;

    void Start()
    {
        timer = gameObject.AddComponent<zzTimer>();
        timer.setInterval(interval);
        timer.setImpFunction(createObject);
    }

    void createObject()
    {
        Instantiate(objectToCreate, placeToCreate.position, placeToCreate.rotation);

    }
}
