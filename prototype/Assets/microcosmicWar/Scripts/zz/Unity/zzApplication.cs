using UnityEngine;

public class zzApplication:MonoBehaviour
{
    public bool changeRunInBackground = false;
    public bool newRunInBackground = false;

    void Start()
    {
        if (changeRunInBackground)
            Application.runInBackground = newRunInBackground;
    }

    public void LoadLevel(string name)
    {
        Application.LoadLevel(name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}