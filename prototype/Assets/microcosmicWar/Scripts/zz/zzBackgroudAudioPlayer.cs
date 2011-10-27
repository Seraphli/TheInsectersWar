using UnityEngine;

//需要加进有 AudioListener 的物体中
public class zzBackgroudAudioPlayer : MonoBehaviour
{
    #region Singleton
    static protected zzBackgroudAudioPlayer singletonInstance = null;

    public static zzBackgroudAudioPlayer Singleton
    {
        get
        {
            return singletonInstance;
        }
    }

    void OnDestroy()
    {
        singletonInstance = null;
    }

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
    }
    #endregion

    public void play(AudioClip pAudioClip)
    {
        play(pAudioClip,1f);
    }

    public void play(AudioClip pAudioClip, float pvolume)
    {
        var lAudioSource = gameObject.AddComponent<AudioSource>();
        lAudioSource.clip = pAudioClip;
        lAudioSource.loop = false;
        lAudioSource.volume = pvolume;
        lAudioSource.Play();
        //在播放完后销毁
        Destroy(lAudioSource, pAudioClip.length);
    }
}