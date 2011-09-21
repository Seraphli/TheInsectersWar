using UnityEngine;

public class zzPlayRandomSound:MonoBehaviour
{
    public AudioSource[] soundToPlay;
    void Start()
    {
        if (soundToPlay.Length != 0)
            soundToPlay[Random.Range(0, soundToPlay.Length)].Play();
    }
}