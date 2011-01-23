
using UnityEngine;
using System.Collections;

/// <summary>
/// 在指定的时间内,恢复指定的血量
/// </summary>
public class LifeRecoverConstValue : MonoBehaviour
{

    //总共可以恢复的生命值
    public int recoverValue = 100;

    //效果持续时间
    public float duration = 10.0f;
    public Life life;

    //FIXME_VAR_TYPE timePos= 0.0f;
    public float recoverValueResidue = 0.0f;

    public void setLife(Life plife)
    {
        life = plife;
    }

    void Update()
    {
        float lDeltaTime = Time.deltaTime;
        if (lDeltaTime > duration)
            lDeltaTime = duration;
        recoverValueResidue += recoverValue * lDeltaTime / duration;
        int lNowRecoverValue = (int)recoverValueResidue;
        recoverValueResidue -= lNowRecoverValue;
        duration -= lDeltaTime;
        //if(lNowRecoverValue>recoverValue)
        //	lNowRecoverValue = recoverValue;
        recoverValue -= lNowRecoverValue;
        life.setBloodValue(life.getBloodValue() + lNowRecoverValue);

        if (duration <= 0)
            zzCreatorUtility.Destroy(this);
    }
}