using UnityEngine;
using System.Collections.Generic;

public class MedicEffectDetector:MonoBehaviour
{
    public zzTriggerDetectorEvent triggerDetectorEvent;

    public GameObject MedicRecoverEffectPrefab;

    Dictionary<Collider, GameObject> soldierToEffect 
        = new Dictionary<Collider,GameObject>();

    //public float lifeVauleCheckInterval = 1f;

    void Awake()
    {
        triggerDetectorEvent.addEnterEventReceiver(OnSoldierEnter);
        triggerDetectorEvent.addExitEventReceiver(OnSoldierExit);

        //var lTimer = gameObject.AddComponent<zzCoroutineTimer>();
        //lTimer.setInterval(lifeVauleCheckInterval);
    }

    void OnDestroy()
    {
        foreach (var lDictionary in soldierToEffect)
        {
            try
            {
                offEffect(lDictionary.Value);
            }
            catch
            {
            }
        }
    }

    void OnSoldierEnter(Collider other)
    {
        var lLife = other.GetComponent<Life>();
        if (lLife.isFull())
            return;
        GameObject lEffect;
        if (soldierToEffect.TryGetValue(other, out lEffect))
        {
            //var lEffect = soldierToEffect[other];
            if(lEffect)
            {
                //以防效果在关闭中
                lEffect.transform.FindChild("OnEffect")
                    .GetComponent<zzOnAction>().impAction();
                return;
            }
        }
        var lEffectObject = (GameObject)Object.Instantiate(MedicRecoverEffectPrefab);
        lEffectObject.transform.parent = other.gameObject.transform;
        lEffectObject.transform.localPosition = Vector3.zero;
        soldierToEffect[other] = lEffectObject;
    }

    void OnSoldierExit(Collider other)
    {
        GameObject lObject;
        if (soldierToEffect.TryGetValue(other, out lObject))
        {
            offEffect(lObject);
        }
    }

    void offEffect(GameObject pEffectObject)
    {
        pEffectObject.transform.FindChild("OffEffect")
            .GetComponent<zzOnAction>().impAction();
    }
}