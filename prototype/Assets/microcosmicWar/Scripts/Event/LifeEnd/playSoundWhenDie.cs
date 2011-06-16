
using UnityEngine;
using System.Collections;

public class playSoundWhenDie : MonoBehaviour
{

    public AudioSource[] soundToPlay;
    public Life life;


    void Awake()
    {
        life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }


    //在死亡的回调中使用
    void deadAction(Life p)
    {
        if (soundToPlay.Length != 0)
            soundToPlay[Random.Range(0, soundToPlay.Length)].Play();
    }
}