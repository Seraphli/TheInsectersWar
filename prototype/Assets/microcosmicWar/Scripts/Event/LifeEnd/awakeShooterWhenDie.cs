using UnityEngine;
using System.Collections;

public class awakeShooterWhenDie : MonoBehaviour
{


    //打死 小兵 或者 塔后,奖励钱用的

    //打死此小兵后奖励的钱数量
    public int shootAward = 1;
    public Life life;
    public Transform showNumPosition;

    void Start()
    {
        if (Network.isClient)
            return;
        life = gameObject.GetComponentInChildren<Life>();
        life.addDieCallback(deadAction);
    }

    //在死亡的回调中使用
    void deadAction(Life p)
    {
        var lCharacterInfo = life.characterInfo;
        if (lCharacterInfo != null)
        {
            lCharacterInfo.purse.add(shootAward);
            var lLifeFlashManager = LifeFlashManager.Singleton;
            var lPos = (showNumPosition ? showNumPosition : transform).position;
            if (lCharacterInfo.belongToThePlayer)
                lLifeFlashManager.showAwardNumLabel(lPos, shootAward);
            else
                lLifeFlashManager.showAwardNumLabel(
                    lPos, shootAward, lCharacterInfo.player);
        }
    }

    //[RPC]
    //void AwakeShooterShowNum()
    //{
    //    var lShowNumPosition = showNumPosition?showNumPosition:transform;
    //    var lShowNumObject = (GameObject)Instantiate(LifeFlashManager.Singleton.awakeNumLabelPrafab,
    //        lShowNumPosition.position,Quaternion.identity);
    //    var lNumber = lShowNumObject.GetComponent<zzSceneTextureNumber>();
    //    lNumber.number = shootAward;
    //}

}