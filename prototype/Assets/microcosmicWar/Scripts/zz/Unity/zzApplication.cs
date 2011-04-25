using UnityEngine;

public class zzApplication:MonoBehaviour
{
    public void LoadLevel(string name)
    {
        Application.LoadLevel(name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}