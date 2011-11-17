using UnityEngine;

public class EventToChangeColor:MonoBehaviour
{
    public Animation colorAnimation;

    public string injuredAnimationName = "injured";

    public AnimationState injuredAnimationNameState;

    public Life life;

    public int lastBloodValue;

    void Awake()
    {
        life.addBloodValueChangeCallback(bloodValueChanged);
        lastBloodValue = life.bloodValue;
        injuredAnimationNameState = colorAnimation[injuredAnimationName];
    }

    void bloodValueChanged(Life pLife)
    {
        if (pLife.bloodValue < lastBloodValue)
        {
            colorAnimation.Play(injuredAnimationName);
            injuredAnimationNameState.time = 0f;
        }
        lastBloodValue = pLife.bloodValue;
    }
}