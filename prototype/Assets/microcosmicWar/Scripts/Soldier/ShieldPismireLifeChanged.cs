using UnityEngine;
using System.Collections;

class ShieldPismireLifeChanged:MonoBehaviour
{
    public Life life;
    public Animation myAnimation;
    public int lifeValue;
    public SoldierAI Ai;

    AnimationState fire1AnimationState;
    AnimationState fire2AnimationState;

    void Start()
    {
        lifeValue = life.getBloodValue();
        life.addBloodValueChangeCallback(lifeChangedCall);

        fire1AnimationState = myAnimation["fire1"];
        fire2AnimationState = myAnimation["fire2"];
    }

    void lifeChangedCall(Life pLife)
    {
        int lLastLifeValue = lifeValue;
        lifeValue = pLife.getBloodValue();
        if (lifeValue > lLastLifeValue || lifeValue<=0 )
            return;
        if (life.harmType == Life.HarmType.explode)
        {
            myAnimation.CrossFade("fire2", 0.2f);
            if (Ai)
                Ai.lockMove(fire2AnimationState.length);
        }
        else if (!fire2AnimationState.enabled )
        {
            myAnimation.CrossFade("fire1", 0.1f);
            if (Ai)
                Ai.lockMove(fire1AnimationState.length);
        }
    }

    //public bool lockError = false;

    //void    Update()
    //{
    //    if (!lockError
    //        && !fire1AnimationState.enabled
    //        && !fire2AnimationState.enabled
    //        && Ai.moveLock
    //        && life.isAlive())
    //    {
    //        Debug.LogError("Ai.moveLock error");
    //        lockError = true;
    //    }
    //}

    //public void unlockMove()
    //{
    //    Ai.moveLock = false;
    //}
}