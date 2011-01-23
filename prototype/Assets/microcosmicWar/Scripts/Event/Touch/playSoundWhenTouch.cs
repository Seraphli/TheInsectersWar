
using UnityEngine;
using System.Collections;

public class playSoundWhenTouch : MonoBehaviour
{

    public AudioSource[] audioSource;
    public AudioClip[] audioClip;

    public enum PlaySoundMode
    {
        //用第一个audioSource的设置,播放audioClip
        sameAudioSource,

        //播放audioSource,audioClip可为空
        differentAudioSource,
    }

    public PlaySoundMode playSoundMode;

    void OnTriggerEnter (Collider other) 
    {
        playSound(other.transform.position);
    }

    void playSound(Vector3 pPosition)
    {
        switch(playSoundMode)
        {
            case PlaySoundMode.differentAudioSource:
                zzUtilities.playAudioSourceAtPoint(
                    audioSource[Random.Range(0, audioSource.Length)],
                    pPosition);
                break;

            case PlaySoundMode.sameAudioSource:
                zzUtilities.playAudioSourceAtPoint(
                    audioSource[0],
                    audioClip[Random.Range(0, audioClip.Length)],
                    pPosition);
                break;

            default:
                Debug.LogError("switch(playSoundMode)");
                break;
        }
    }
}