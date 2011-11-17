using UnityEngine;

public class LifeWarning:MonoBehaviour
{
    public Life life;
    public string bubbleMessageTagName = "BubbleMessage";

    public GameObject attackedBubblePrefab;
    //public Texture assaultedImage;
    public AudioClip attackedSound;

    public GameObject destroyedBubblePrefab;
    //public Texture destroyedImage;
    public AudioClip destroyedSound;

    public Transform bubblePosition;

    public zzGUIBubbleComputeRect bubble;

    public int lastBloodValue;

    public float warningTimeLength = 4f;

    [SerializeField]
    Race _race;

    public Race race
    {
        get
        {
            if (_race == Race.eNone)
                _race = PlayerInfo.getRace(owner.layer);
            return _race;
        }
    }

    //在race(==nono)不确定时,通过此获取race
    public GameObject owner;

    void Reset()
    {
        bubblePosition = transform;
    }

    void Awake()
    {
        life.addBloodValueChangeCallback(bloodValueChanged);
        life.addDieCallback(destroyed);
        lastBloodValue = life.bloodValue;
    }

    void destroyed(Life pLife)
    {
        if (GameScene.Singleton.playerInfo.race != race)
            return;
        if(bubble)
        {
            GameObject.Destroy(bubble.gameObject);
        }
        if (destroyedSound)
            zzBackgroudAudioPlayer.Singleton.play(destroyedSound);

        bubble = bubbleMessage.addBubble(destroyedBubblePrefab);
        bubble.transform.position = bubblePosition.position;
        bubble.bubblePosition = bubble.transform;
        ((zzGUIBubbleLayout)bubble.bubbleLayout).showTime = warningTimeLength;
    }

    zzGUIBubbleIconMessage bubbleMessage
    {
        get
        {
            return GameObject.FindGameObjectWithTag(bubbleMessageTagName)
                    .GetComponent<zzGUIBubbleIconMessage>();
        }
    }

    zzGUIBubbleComputeRect createBubble(GameObject pPrefab)
    {
        return bubbleMessage.addBubble(bubblePosition, pPrefab);
    }

    void bloodValueChanged(Life pLife)
    {
        if (pLife.bloodValue < lastBloodValue
            && GameScene.Singleton.playerInfo.race == race)
        {
            if (!bubble)
            {
                if (attackedSound)
                    zzBackgroudAudioPlayer.Singleton.play(attackedSound);
                bubble = createBubble(attackedBubblePrefab);
                ((zzGUIBubbleLayout)bubble.bubbleLayout)
                    .showTime = warningTimeLength;
            }
            ((zzGUIBubbleLayout)bubble.bubbleLayout)
                .timePostion = 0f;
        }
        lastBloodValue = pLife.bloodValue;
    }
}