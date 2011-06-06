using UnityEngine;

public class zzDestroyObject:MonoBehaviour
{
    System.Func<Object> getObjectFunc;

    public void addGetObjectFunc(System.Func<Object> pFunc)
    {
        getObjectFunc += pFunc;
    }

    public void impDestory()
    {
        Destroy(getObjectFunc());
    }
}