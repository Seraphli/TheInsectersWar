using UnityEngine;

public class SoldierFactoryListener:MonoBehaviour
{
    [SerializeField]
    MonoBehaviour _interfaceObject;

    public SoldierFactory.Listener interfaceObject
    {
        get
        {
            return (SoldierFactory.Listener)_interfaceObject;
        }
    }
}