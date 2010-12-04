using UnityEngine;
using System.Collections;

class AutoSupplyAirplane:MonoBehaviour
{
    zzCoroutineTimer timer;
    public SupplyAirplane.FlyInfo supplyInfo = new SupplyAirplane.FlyInfo();
    public float minIntervalTime = 0.0f;
    public float maxIntervalTime =1.0f;
    public float nextTakeoffTime = 0.0f;
    StartSupply startSupply;
    public GameObject plane;

    void Start()
    {
        startSupply = gameObject.AddComponent<StartSupply>();
        startSupply.plane = plane;

        timer = gameObject.AddComponent<zzCoroutineTimer>();
        timer.setInterval(nextTakeoffTime);
        timer.setImpFunction(takeoff);
    }

    void takeoff()
    {
        startSupply.startSupplyPlane(supplyInfo);
        timer.setInterval(getNextTime());
    }

    float getNextTime()
    {
        nextTakeoffTime = Random.Range(minIntervalTime, maxIntervalTime);
        return nextTakeoffTime;
    }
}