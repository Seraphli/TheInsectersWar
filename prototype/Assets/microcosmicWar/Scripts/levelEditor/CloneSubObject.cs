using UnityEngine;
using System.Collections;

public class CloneSubObject:MonoBehaviour
{
    public Transform toCloneSubObject;

    public Transform toBeParent;

    public void doClone()
    {
        foreach (Transform lTransform in toCloneSubObject)
        {
            var lClone = (GameObject)Instantiate(lTransform.gameObject);
            lClone.name = lTransform.name;
            lClone.transform.parent = toBeParent;
        }
    }
}