using UnityEngine;
using System.Collections.Generic;

public class MedicEffectDetector:MonoBehaviour
{
    public zzTriggerDetectorEvent triggerDetectorEvent;

    public GameObject MedicRecoverEffectPrefab;

    Dictionary<Collider, GameObject> soldierToEffect 
        = new Dictionary<Collider,GameObject>();

    void Awake()
    {
        triggerDetectorEvent.addEnterEventReceiver(OnSoldierEnter);
        triggerDetectorEvent.addExitEventReceiver(OnSoldierExit);
    }

    void OnDestroy()
    {
        try
        {
            foreach (var lDictionary in soldierToEffect)
            {
                offEffect(lDictionary.Value);
            }
        }
        catch
        {
        }

    }

    void OnSoldierEnter(Collider other)
    {
        if(soldierToEffect.ContainsKey(other))
        {
            var lEffect = soldierToEffect[other];
            if(lEffect)
            {
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
        //因为一个物体会被多次OnTriggerExit
        if (soldierToEffect.ContainsKey(other))
            offEffect(soldierToEffect[other]);
    }

    void offEffect(GameObject pEffectObject)
    {
        pEffectObject.transform.FindChild("OffEffect")
            .GetComponent<zzOnAction>().impAction();
    }
}