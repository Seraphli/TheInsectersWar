

using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour
{
    public delegate void lifeCallFunc(Life life);

    public int bloodValue = 5;
    public int fullBloodValue = 5;

    protected zzUtilities.voidFunction bloodValueChangeCallback = zzUtilities.nullFunction;
    //FIXME_VAR_TYPE dieCallback=zzUtilities.nullFunction;
    public ArrayList dieCallbackList = new ArrayList();

    protected Hashtable injureInfo;
    //搜索有无bloodBar 以便显示血量
    void Start()
    {
        BloodBar lBloodBar = GetComponentInChildren<BloodBar>();
        if (lBloodBar)
            lBloodBar.setLife(this);
    }
    /*
    void  setDieCallback ( call ){
        dieCallback=call;
    }*/

    public void addDieCallback(lifeCallFunc call)
    {
        dieCallbackList.Add(call);
    }

    public void setBloodValueChangeCallback(zzUtilities.voidFunction call)
    {
        bloodValueChangeCallback = call;
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
            bloodValueChangeCallback();
            if (bloodValue <= 0)
            {
                //dieCallback();
                //foreach(var dieCallback in dieCallbackList)
                //	dieCallback();
                //zzCreatorUtility.Destroy (gameObject);
                zzCreatorUtility.sendMessage(gameObject, "Life_die");
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
        foreach (lifeCallFunc dieCallback in dieCallbackList)
            dieCallback(this);
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