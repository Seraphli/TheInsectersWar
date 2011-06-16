

using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour
{
    public const string harmTypeName = "harmType";
    public enum harmType
    {
        explode,
    }

    public delegate void lifeCallFunc(Life life);

    static void nullLifeCallFunc(Life life){}

    public int bloodValue = 5;
    public int fullBloodValue = 5;

    protected event lifeCallFunc bloodValueChangeCallback = nullLifeCallFunc;

    public lifeCallFunc dieCallbackList;

    //在死亡或者去血的回调中,读取
    protected Hashtable injureInfo;
    //搜索有无bloodBar 以便显示血量
    //void Start()
    //{
    //}
    /*
    void  setDieCallback ( call ){
        dieCallback=call;
    }*/

    public void addDieCallback(lifeCallFunc call)
    {
        dieCallbackList+=call;
    }

    public void removeDieCallback(lifeCallFunc call)
    {
        dieCallbackList-=call;
    }

    public void addBloodValueChangeCallback(lifeCallFunc call)
    {
        bloodValueChangeCallback -= nullLifeCallFunc;
        bloodValueChangeCallback += call;
    }

    public void injure(int value, Hashtable pInjureInfo)
    {
        injureInfo = pInjureInfo;
        if (bloodValue > 0)
        {
            setBloodValue(bloodValue - value);
        }
        injureInfo = null;
    }

    public void injure(int value)
    {
        injure(value, null);
    }

    //在回调中调用
    public Hashtable getInjureInfo()
    {
        return injureInfo;
    }

    public void setBloodValue(int pValue)
    {
        if (pValue > fullBloodValue)
            pValue = fullBloodValue;
        if (bloodValue != pValue)
        {
            bloodValue = pValue;
            bloodValueChangeCallback(this);
            if (bloodValue <= 0)
            {
                //zzCreatorUtility.sendMessage(gameObject, "Life_die");
                Life_die();
                if (Network.peerType != NetworkPeerType.Disconnected)
                    networkView.RPC("Life_die", RPCMode.Others);
                    //NetworkHelper.makeDead(networkView);
            }
        }
    }

    public void makeDead()
    {
        setBloodValue(0);
    }

    [RPC]
    public void Life_die()
    {
        bloodValue = 0;
        dieCallbackList(this);
    }

    public void setFullBloodValue(int lValue)
    {
        fullBloodValue = lValue;
    }

    public int getBloodValue()
    {
        return bloodValue;
    }


    public int getFullBloodValue()
    {
        return fullBloodValue;
    }

    public bool isAlive()
    {
        return bloodValue > 0;
    }

    public bool isDead()
    {
        return bloodValue <= 0;
    }

    public float getRate()
    {
        float lFullBloodValue = getFullBloodValue();
        float lRate = getBloodValue() / lFullBloodValue;
        if (lRate < 0)
            return 0.0f;
        else
            return lRate;
    }

    public bool isFull()
    {
        return bloodValue >= fullBloodValue;
    }


    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int lBloodValue = getBloodValue();

        //---------------------------------------------------
        //if (stream.isWriting)
        //{
        //	lBloodValue=getBloodValue();
        //}

        stream.Serialize(ref lBloodValue);

        if (stream.isReading)
        {
            setBloodValue(lBloodValue);
        }
    }

    public static Life getLifeFromTransform(Transform pOwn)
    {
        Life lLife = pOwn.gameObject.GetComponent<Life>();

        if (!lLife)
        {
            while (pOwn.parent)
            {
                pOwn = pOwn.parent;
                lLife = pOwn.gameObject.GetComponent<Life>();
                if (lLife)
                    break;
            }
        }
        return lLife;
    }

    //返回活着的物体的生命组件,可用来判断是否存活
    public static Life getLifeIfAlive(Transform pOwn)
    {
        Life lLife = getLifeFromTransform(pOwn);
        if(lLife && lLife.isDead())
        {
            lLife = null;
        }
        return lLife;
    }


    //function Update ()
    //{
    //}0
}