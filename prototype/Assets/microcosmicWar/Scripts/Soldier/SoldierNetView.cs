
using UnityEngine;
using System.Collections;

public class SoldierNetView : MonoBehaviour
{


    public Soldier soldier;
    public SoldierModelSmoothMove soldierModelSmoothMove;
    public Life life;
    public Character2D character;
    public ActionCommandControl actionCommandControl;
    //FIXME_VAR_TYPE transform;
    public float disappearTime = 3f;
    public zzTimer disappearTimer;
    public MonoBehaviour[] disenableWhenDisappear;

    //class NetData
    //{
    //    public NetData(int p1,int p2,double pTimestamp)
    //    {
    //        part1 = p1;
    //        part2 = p2;
    //        timestamp = pTimestamp;
    //    }
    //    public int part1;
    //    public int part2;
    //    public double timestamp;
    //}

    //System.Collections.Generic.Queue<NetData> netDataQueue
    //    = new System.Collections.Generic.Queue<NetData>();

    void Awake()
    {
        if (!soldier)
            soldier = gameObject.GetComponentInChildren<Soldier>();
        //if(!soldier)
        //	soldier=gameObject.GetComponentInChildren<Soldier>().getCharacter();
        //character = gameObject.GetComponentInChildren<Soldier>().getCharacter();
        soldierModelSmoothMove = GetComponent<SoldierModelSmoothMove>();
        character = soldier.getCharacter();
        actionCommandControl = gameObject.GetComponentInChildren<ActionCommandControl>();
        if (!life)
            life = gameObject.GetComponentInChildren<Life>();

        if (Network.isClient)
        {
            Destroy(soldier.GetComponentInChildren<SoldierAI>());
            disappearTimer = gameObject.AddComponent<zzTimer>();
            disappearTimer.setInterval(disappearTime);
            disappearTimer.addImpFunction(disappear);
            life.addDieCallback((x) => disappearTimer.enabled = false);
        }
        if(disenableWhenDisappear ==null||disenableWhenDisappear.Length==0)
        {
            disenableWhenDisappear = new MonoBehaviour[] { soldier };
        }
        //if( !zzCreatorUtility.isMine(gameObject.networkView ) )
        //{
        //	Destroy(soldier.GetComponentInChildren<SoldierAI>());
        //}
        //if(!soldier)
        //	Debug.LogError(gameObject.name);
    }

    readonly Vector3 disappearPostion = new Vector3(-100f, -100f, 0f);

    void disappear()
    {
        gameObject.transform.position = disappearPostion;
        disappearTimer.enabled = false;
        foreach (var lScript in disenableWhenDisappear)
        {
            lScript.enabled = false;
        }
    }

    void appear()
    {
        disappearTimer.timePos = 0f;
        if (!disappearTimer.enabled)
        {
            disappearTimer.enabled = true;
            foreach (var lScript in disenableWhenDisappear)
            {
                lScript.enabled = true;
            }
        }
    }

    const int soldierLifeRateMaxValue = (byte.MaxValue + 1) / 2 - 1;
    const int max24BitPosValue = 16777215;
    const int max17BitPosValue = 131071;
    const int max14BitPosValue = 16383;
    const float posRange = 800f;

    //    血量      动作           x坐标               y坐标        无Y方向的速度 
    //|<---7--->|<---8--->|<---------24-------->|<---------24--------->|<-1->|
    //
    //    血量      动作        x坐标         y坐标       Y方向的速度 有Y方向的速度  
    //|<---7--->|<---8--->|<-----17----->|<-----17----->|<-----14----->|<-1->|
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {

        Vector3 lPostion = Vector3.zero;
        int lPart1 = 0;
        int lPart2 = 0;
        if (stream.isWriting)
        {
            var lBitIn = new zz.LongBitIO(0);
            lPostion = transform.position;
            bool lInAir = character.yVelocity != zzCharacter.yNullVelocity;
            if (lInAir)
            {
                var lLifeRateValue = (int)(life.rate * soldierLifeRateMaxValue);
                lBitIn.write(lLifeRateValue, 7);
                lBitIn.write(actionCommandControl.commandValue, 8);
                int lXValue = (int)((double)max17BitPosValue * ((double)lPostion.x / posRange));
                lBitIn.write(lXValue, 17);
                int lYValue = (int)((double)max17BitPosValue * ((double)lPostion.y / posRange));
                lBitIn.write(lYValue, 17);
                int lYVelocity = (int)((double)max14BitPosValue *
                    (((double)character.yVelocity - character.minYVelocity) /
                    (character.jumpSpeed - character.minYVelocity)));
                lBitIn.write(lYVelocity, 14);
                lBitIn.write(1, 1);
            }
            else
            {
                var lLifeRateValue = (int)(life.rate * soldierLifeRateMaxValue);
                lBitIn.write(lLifeRateValue, 7);
                lBitIn.write(actionCommandControl.commandValue, 8);
                int lXValue = (int)((double)max24BitPosValue * ((double)lPostion.x / posRange));
                lBitIn.write(lXValue, 24);
                int lYValue = (int)((double)max24BitPosValue * ((double)lPostion.y / posRange));
                lBitIn.write(lYValue, 24);
                lBitIn.write(0, 1);
            }
            lPart1 = lBitIn.readToInt(32);
            lPart2 = lBitIn.readToInt(32);
        }

        stream.Serialize(ref lPart1);
        stream.Serialize(ref lPart2);

        if (stream.isReading)
        {
            double lTimestamp = info.timestamp;
            //{
            //    double lDelay = 0.4;
            //    float lPacketLossRatio = 0.4f;
            //    NetData lData = null;
            //    if (Random.value > lPacketLossRatio)
            //        netDataQueue.Enqueue(new NetData(lPart1, lPart2, info.timestamp));
            //    while (netDataQueue.Count>0
            //        && Network.time - netDataQueue.Peek().timestamp > lDelay)
            //    {
            //        lData = netDataQueue.Dequeue();
            //    }
            //    if (lData != null)
            //    {
            //        lPart1 = lData.part1;
            //        lPart2 = lData.part2;
            //        lTimestamp = lData.timestamp;
            //    }
            //    else
            //        return;
            //}
            appear();
            var lBitOut = new zz.LongBitIO(0);
            lBitOut.write(lPart2, 32);
            lBitOut.write(lPart1, 32);
            if (lBitOut.readToInt(1) == 1)
            {
                var lYVelocity = lBitOut.readToInt(14);
                var lYValue = lBitOut.readToInt(17);
                var lXValue = lBitOut.readToInt(17);
                //var lActionCommandControl = lBitOut.readToInt(8);
                actionCommandControl.commandValue = lBitOut.readToInt(8);
                var lLifeRateValue = lBitOut.readToInt(7);

                life.rate = (float)lLifeRateValue / (float)soldierLifeRateMaxValue;
                lPostion.x = (float)((double)lXValue / (double)max17BitPosValue * posRange);
                lPostion.y = (float)((double)lYValue / (double)max17BitPosValue * posRange);
                character.yVelocity = (float)((double)lYVelocity / (double)max14BitPosValue
                    * (character.jumpSpeed - character.minYVelocity)
                    + character.minYVelocity);

            }
            else
            {
                var lYValue = lBitOut.readToInt(24);
                var lXValue = lBitOut.readToInt(24);
                actionCommandControl.commandValue = lBitOut.readToInt(8);
                var lLifeRateValue = lBitOut.readToInt(7);

                life.rate = (float)lLifeRateValue / (float)soldierLifeRateMaxValue;
                lPostion.x = (float)((double)lXValue / (double)max24BitPosValue * posRange);
                lPostion.y = (float)((double)lYValue / (double)max24BitPosValue * posRange);
                character.yVelocity = zzCharacter.yNullVelocity;
            }
            soldierModelSmoothMove.beginMove();
            transform.position = lPostion;
            var lDeltaTime = (float)(Network.time - lTimestamp);
            //if (lDeltaTime > 0.02f)
            //{
                character.update2D(actionCommandControl.getCommand(),
                    UnitFace.getValue(actionCommandControl.face),
                    life.isAlive(), lDeltaTime * Time.timeScale);
                character.lastUpdateTime = Time.time;
                //}
            soldierModelSmoothMove.endMove();
        }

    }
}