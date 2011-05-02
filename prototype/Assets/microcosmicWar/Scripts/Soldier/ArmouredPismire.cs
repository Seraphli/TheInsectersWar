using UnityEngine;
using System.Collections;

public class ArmouredPismire:MonoBehaviour
{
    public DefenseTower defenseTower;
    public LifeTriggerDetector[] lifeTriggerDetectors;

    public ParticleEmitter jumpJet;

    //敌人还在这个角度时,枪不动
    public float fireDeviation = 4.0f;

    public Soldier soldier;
    public zzCharacter character;

    public float speedOnGround;
    public float speedOnAir;

    void Start()
    {
        character = soldier.character;
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
            jumpJet.emit = false;
        }
        else
        {
            character.runSpeed = speedOnAir;
            jumpJet.emit = true;
        }

    }
}