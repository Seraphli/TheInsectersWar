using UnityEngine;

public class LifeFlash:MonoBehaviour
{
    public Animation colorAnimation;

    public string injuredAnimationName = "injured";

    public AnimationState injuredAnimationNameState;

    public Life life;

    public zzUseSameMaterial materialObject;

    public bool isPlayer = false;

    void Awake()
    {
        life.addInjuredEventReceiver(bloodValueChanged);
        injuredAnimationNameState = colorAnimation[injuredAnimationName];
    }

    void bloodValueChanged(Life pLife)
    {
        var lCharacterInfo = pLife.characterInfo;
        var lLifeFlashManager = LifeFlashManager.Singleton;
        if (isPlayer)
        {
            playColorAnimation(lLifeFlashManager.selfAttackedColor);
        }
        else if(lCharacterInfo!=null
            && lCharacterInfo.race == PlayerInfo.playerRace)
        {
            if (lCharacterInfo.belongToThePlayer)
                playColorAnimation( lLifeFlashManager.attackedBySelfColor );
            else
                playColorAnimation( lLifeFlashManager.attackedByTeammateColor );
        }
    }

    void playColorAnimation(Color lColor)
    {
        var lMaterial = materialObject.objectMaterial;
        var lLastColor = materialObject.objectMaterial.color;
        lColor.a = lLastColor.a;
        lMaterial.color = lColor;
        colorAnimation.Play(injuredAnimationName);
        injuredAnimationNameState.time = 0f;
    }
}