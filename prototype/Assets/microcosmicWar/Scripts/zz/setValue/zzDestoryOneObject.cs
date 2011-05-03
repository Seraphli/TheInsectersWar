using UnityEngine;

public class zzDestoryOneObject:MonoBehaviour
{
    public GameObject objectToDestory;

    public void impDestory()
    {
        Destroy(objectToDestory);
    }
}