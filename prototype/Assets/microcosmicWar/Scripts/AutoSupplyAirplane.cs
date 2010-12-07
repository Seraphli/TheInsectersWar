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

    public float throwRangeMin;
    public float throwRangeMax;

    [System.Serializable]
    class AwardItemInfo
    {
        public string nameOfAwardItem;
        //public int IdOfAwardItem;

        public int weigth;

    }

    zzRandomObjectByWeight<int> randomItemID = new zzRandomObjectByWeight<int>();

    [SerializeField]
    AwardItemInfo[] awardItemInfoes;

    void initSupplyObject(GameObject pObject)
    {
        AwardItemWhenTouch lAwardItemWhenTouch = pObject.GetComponent<AwardItemWhenTouch>();
        if (!lAwardItemWhenTouch)
            Debug.LogError("!lAwardItemWhenTouch");
        int lItemID =  randomItemID.randomObject();
        lAwardItemWhenTouch.itemID = lItemID;
    }

    void Start()
    {
        zzIndexTable    lItemTypeTable = zzItemSystem.getSingleton().getItemTypeTable();
        foreach (var lAwardItemInfo in awardItemInfoes)
        {
            randomItemID.addRandomObject(
                lItemTypeTable.getIndex(lAwardItemInfo.nameOfAwardItem),
                lAwardItemInfo.weigth);
        }

        //IDOfAwardItem = .getIndex(nameOfAwardItem);

        startSupply = gameObject.AddComponent<StartSupply>();
        startSupply.planeToCreate = plane;
        startSupply.initSupplyObjectFunc = initSupplyObject;

        timer = gameObject.AddComponent<zzCoroutineTimer>();
        timer.setInterval(nextTakeoffTime);
        timer.setImpFunction(takeoff);
    }

    void takeoff()
    {
        supplyInfo.putX = Random.Range(throwRangeMin, throwRangeMax);
        startSupply.startSupplyPlane(supplyInfo);
        timer.setInterval(getNextTime());
    }

    float getNextTime()
    {
        nextTakeoffTime = Random.Range(minIntervalTime, maxIntervalTime);
        return nextTakeoffTime;
    }
}