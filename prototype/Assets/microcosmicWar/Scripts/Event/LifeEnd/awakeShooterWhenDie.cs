using UnityEngine;
using System.Collections;

public class awakeShooterWhenDie : MonoBehaviour
{


    //打死 小兵 或者 塔后,奖励钱用的

    //打死此小兵后奖励的钱数量
    public int shootAward = 1;
    public Life life;

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
        //Hashtable lInjureInfo = life.getInjureInfo();
        ////得到 保存 在子弹中的 发射者的 背包信息,以加钱
        //if (lInjureInfo!=null && lInjureInfo.ContainsKey("bagControl"))
        //{
        //    zzItemBagControl lBagControl = (zzItemBagControl)lInjureInfo["bagControl"];
        //    lBagControl.addMoney(shootAward);
        //}
        if (life.attackerPurse)
            life.attackerPurse.add(shootAward);
    }

}