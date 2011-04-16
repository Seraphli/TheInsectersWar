using UnityEngine;
using System.Collections;

public class AutoWMGameObjectType : MonoBehaviour
{
    void Start()
    {
        WMPlayStateManager.addManagedObject(gameObject);
    }
}