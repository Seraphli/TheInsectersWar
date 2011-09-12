using UnityEngine;

public class zzDestroyObjectWhenDestroyed:MonoBehaviour
{
    public GameObject objectToDestroy;
    void OnDestroy()
    {
        Destroy(objectToDestroy);
    }
}