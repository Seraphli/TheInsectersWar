using UnityEngine;

public class zzDestroyOneObject:MonoBehaviour
{
    public GameObject objectToDestory;

    public void impDestory()
    {
        Destroy(objectToDestory);
    }
}