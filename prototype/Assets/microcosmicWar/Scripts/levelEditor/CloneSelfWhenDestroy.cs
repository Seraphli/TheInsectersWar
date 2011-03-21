using UnityEngine;
using System.Collections;

public class CloneSelfWhenDestroy : MonoBehaviour
{
    void OnDestroy()
    {
        GameObject objectToClone = gameObject;
        var lClone = (GameObject)Instantiate(objectToClone);
        lClone.name = objectToClone.name;
        lClone.transform.parent = objectToClone.transform.parent;
    }
}