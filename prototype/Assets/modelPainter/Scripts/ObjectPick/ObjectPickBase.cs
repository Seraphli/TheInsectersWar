using UnityEngine;
using System.Collections;

public abstract class ObjectPickBase:MonoBehaviour
{
    public virtual void OnLeftOn(GameObject pObject){}

    public virtual void OnLeftOff(GameObject pObject) { }

    public virtual void OnRightOn(GameObject pObject) { }

    public virtual void OnRightOff(GameObject pObject) { }
}