

using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour
{
    public const string harmTypeName = "harmType";
    public enum HarmType
    {
        none,
        explode,
    }

    [SerializeField]
    HarmType _harmType = HarmType.none;

    public HarmType harmType
    {
        get { return _harmType; }
    }

    public delegate void lifeCallFunc(Life life);

    static void nullLifeCallFunc(Life life){}

    public int bloodValue = 5;
    public int fullBloodValue = 5;

    protected event lifeCallFunc bloodValueChangeCallback = nullLifeCallFunc;

    lifeCallFunc injuredEvent = nullLifeCallFunc;

    public void addInjuredEventReceiver(lifeCallFunc pReceiver)
    {
        injuredEvent -= nullLifeCallFunc;
        injuredEvent += pReceiver;
    }

    public lifeCallFunc dieCallbackList;

    //在死亡或者去血的回调中,读取
    //protected Hashtable injureInfo;
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

    CharacterInfo _characterInfo;
    public CharacterInfo characterInfo
    {
        get { return _characterInfo; }
    }

    public void injure(int value, CharacterInfo pOwner)
    {
        _characterInfo = pOwner;
        injure(value);
        _characterInfo = null;
    }

    public void injure(int value, CharacterInfo pOwner, HarmType pHarmType)
    {
        _characterInfo = pOwner;
        _harmType = pHarmType;
        injure(value);
        _harmType = HarmType.none;
        _characterInfo = null;
    }

    //public void injure(int value, WMPurse pAttackerPurse)
    //{
    //    _attackerPurse = pAttackerPurse;
    //    injure(value);
    //    _attackerPurse = null;
    //}

    //public void injure(int value, Hashtable pInjureInfo, WMPurse pAttackerPurse)
    //{
    //    attackerPurse = pAttackerPurse;
    //    injure(value, pInjureInfo);
    //    attackerPurse = null;
    //}

    public void injure(int value, HarmType pHarmType)
    {
        _harmType = pHarmType;
        injure(value);
        _harmType = HarmType.none;
    }

    //public void injure(int value, Hashtable pInjureInfo)
    //{
    //    injureInfo = pInjureInfo;
    //    injure(value);
    //    injureInfo = null;
    //}

    public void injure(int value)
    {
        if (value > 0)
            injuredEvent(this);
        if (!Network.isClient && bloodValue > 0)
        {
            setBloodValue(bloodValue - value);
        }
    }

    //在回调中调用
    //public Hashtable getInjureInfo()
    //{
    //    return injureInfo;
    //}

    public bool netSendPositionWhenDie = true;

    public void setBloodValue(int pValue)
    {
        if (isDead())
            return;
        if (pValue > fullBloodValue)
            pValue = fullBloodValue;
        if (bloodValue != pValue)
        {
            bloodValue = pValue;
            bloodValueChangeCallback(this);
            if (bloodValue <= 0)
            {
                bloodValue = 0;
                //zzCreatorUtility.sendMessage(gameObject, "Life_die");
                dieCallbackList(this);
                if (Network.isServer && networkView)
                {
                    if (netSendPositionWhenDie)
                        networkView.RPC("RPCMakeDeadPos", RPCMode.Others, transform.position);
                    else
                        networkView.RPC("RPCMakeDead", RPCMode.Others);
                }
            }
        }
    }

    public void makeDead()
    {
        setBloodValue(0);
    }

    [RPC]
    public void RPCMakeDeadPos(Vector3 pPos)
    {
        transform.position = pPos;
        makeDead();
    }

    [RPC]
    public void RPCMakeDead()
    {
        makeDead();
    }

    //public void Life_die()
    //{
    //    dieCallbackList(this);
    //}

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

    public byte rateInByte
    {
        get 
        {
            var lRate = rate;
            float lValue;
            //if (lRate > 0.5)
            //    return (byte)(Mathf.FloorToInt((float)byte.MaxValue) * lRate);
            lValue = (float)Mathf.CeilToInt((float)byte.MaxValue * lRate);
            //if (lRate<1f)
            //{
            //    print(lRate);
            //    print(lValue);
            //    print((byte)lValue);
            //}
            return (byte)lValue; 
        }
        set
        {
            //if (value < byte.MaxValue)
            //{
            //    print((float)value / (float)byte.MaxValue);
            //}
            rate = (float)value/(float)byte.MaxValue;
        }
    }

    public float rate
    {
        get
        {
            float lFullBloodValue = getFullBloodValue();
            float lRate = getBloodValue() / lFullBloodValue;
            if (lRate < 0)
                return 0.0f;
            else
                return lRate;
        }
        set
        {
            //if (value != 1f)
            //    print((int)(getFullBloodValue() * value));
            setBloodValue((int)(getFullBloodValue()*value));
        }
    }

    public bool isFull()
    {
        return bloodValue >= fullBloodValue;
    }


    //public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    //{
    //    int lBloodValue = getBloodValue();

    //    //---------------------------------------------------
    //    //if (stream.isWriting)
    //    //{
    //    //	lBloodValue=getBloodValue();
    //    //}

    //    stream.Serialize(ref lBloodValue);

    //    if (stream.isReading)
    //    {
    //        setBloodValue(lBloodValue);
    //    }
    //}


    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        char lRateInByte = (char)rateInByte;

        //---------------------------------------------------
        //if (stream.isWriting)
        //{
        //	lBloodValue=getBloodValue();
        //}

        stream.Serialize(ref lRateInByte);

        if (stream.isReading)
        {
            rateInByte = (byte)lRateInByte;
        }
    }

    public static Life getLifeFromTransform(Transform pOwn)
    {
        Life lLife = pOwn.GetComponent<Life>();

        if (!lLife)
        {
            while (pOwn.parent)
            {
                pOwn = pOwn.parent;
                lLife = pOwn.GetComponent<Life>();
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