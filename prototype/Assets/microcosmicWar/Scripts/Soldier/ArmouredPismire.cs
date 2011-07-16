using UnityEngine;
using System.Collections;

public class ArmouredPismire:MonoBehaviour
{
    public ActionCommandControl actionCommandControl;
    public DefenseTower defenseTower;
    public LifeTriggerDetector[] lifeTriggerDetectors;

    public ParticleEmitter jumpJet;
    public float jumpJetTime;
    public Animation myAnimation;
    public string jumpAnimationName;

    //敌人还在这个角度时,枪不动
    public float fireDeviation = 4.0f;

    public Soldier soldier;
    public Character2D character;

    public float speedOnGround;
    public float speedOnAir;

    zzTimer jumpJetEffectzzTimer;

    void Start()
    {
        character = soldier.character2D;
        jumpJetEffectzzTimer = gameObject.AddComponent<zzTimer>();
        jumpJetEffectzzTimer.setInterval(jumpJetTime);
        jumpJetEffectzzTimer.addImpFunction(OffJumpJetEffect);
        jumpJetEffectzzTimer.enabled = false;
    }

    void Update()
    {
        Transform lAim = null;
        foreach (var lDetector in lifeTriggerDetectors)
        {
            if(lDetector.lockedTarget)
            {
                lAim = lDetector.lockedTarget;
                break;
            }
        }

        if (lAim)
            defenseTower.takeAim(lAim.position, fireDeviation);

        if (character.isGrounded())
        {
            character.runSpeed = speedOnGround;
        }
        else
        {
            character.runSpeed = speedOnAir;
        }
        if (actionCommandControl.getCommand().Jump)
            OnJump();

    }

    void OnJump()
    {
        myAnimation.CrossFade(jumpAnimationName, 0.2f);
        jumpJet.emit = true;
        jumpJetEffectzzTimer.enabled = true;
    }

    void OffJumpJetEffect()
    {
        jumpJet.emit = false;
        jumpJetEffectzzTimer.enabled = false;
    }


}