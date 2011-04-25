using UnityEngine;

public class zzDontDestroyOnLoad:MonoBehaviour
{
    public void imp()
    {
        DontDestroyOnLoad(gameObject);
    }
}