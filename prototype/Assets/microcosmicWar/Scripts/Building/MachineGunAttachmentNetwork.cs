using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineGunAttachmentNetwork:MonoBehaviour
{
    public DefenseTower defenseTower;
    public Life life;
    //public NetworkStateSynchronization preSysnState;
    //public HashSet<NetworkPlayer> insidePlayerScope;
    //public NetworkView attachmentNetView;

    IEnumerator Start()
    {
        if(Network.peerType== NetworkPeerType.Disconnected)
        {
            Destroy(this);
        }
        else
        {
            while(true)
            {
                if (transform.parent.GetComponent<StrongholdUpdate>())
                    break;
                yield return null;
            }
            var lParent = transform.parent;
            life = lParent.GetComponent<Life>();
            if(life.isAlive())
            {
                if (Network.isServer)
                {
                    var lParentNetview = lParent.networkView;
                    //preSysnState = lParentNetview.stateSynchronization;
                    //lParentNetview.stateSynchronization = NetworkStateSynchronization.Off;
                    lParentNetview.enabled = false;
                }
            }

        }
    }

    zz.IntBitIO bitIO = new zz.IntBitIO(0);
    const int lifeRateMaxValue = (byte.MaxValue + 1) * 2 - 1;
    const int aimAngleRateMaxValue = 4194303;
    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int lDate = 0;
        if(stream.isWriting)
        {
            //if (defenseTower.fire)
            //    lDate |= 1;
            ////lDate <<= 1;
            //var lLifeRateValue = (ushort)(life.rate * lifeRateMaxValue);
            //lDate <<= 8;
            //lDate |= (lLifeRateValue & 0xff);
            //lDate <<= 1;
            //if (lLifeRateValue > byte.MaxValue)
            //    lDate |= 1 << 9;

            //lDate <<= 22;
            //var lAimAngleRate = (defenseTower.aimAngle - defenseTower.maxDownAngle)
            //    / (defenseTower.maxUpAngle - defenseTower.maxDownAngle);
            //var lAngleRateValue = (int)(lAimAngleRate * aimAngleRateMaxValue);
            //lDate |= (lAngleRateValue & 0x3fffff);

            bitIO.date = 0;
            if (defenseTower.fire)
                bitIO.write(1, 1);
            else
                bitIO.write(0, 1);

            var lLifeRateValue = (int)(life.rate * lifeRateMaxValue);
            bitIO.write(lLifeRateValue, 9);

            var lAimAngleRate = (defenseTower.aimAngle - defenseTower.maxDownAngle)
                / (defenseTower.maxUpAngle - defenseTower.maxDownAngle);
            var lAngleRateValue = (int)(lAimAngleRate * aimAngleRateMaxValue);
            bitIO.write(lAngleRateValue, 22);

            lDate = bitIO.date;
            //print("fire:" + defenseTower.fire
            //    + " life.rate:" + life.rate
            //    + " aimAngle:" + defenseTower.aimAngle);
        }
        stream.Serialize(ref lDate);
        if(stream.isReading)
        {
            //defenseTower.fire = (lDate & 0x1) != 0;
            //lDate >>= 1;
            //int lLifeRateValue = lDate & 0xff;
            //lDate >>= 8;
            //if ((lDate & 1) != 0)
            //    lLifeRateValue += byte.MaxValue + 1;
            //lDate >>= 1;
            //var lAngleRateValue = lDate;
            //defenseTower.aimAngle = (lAngleRateValue / aimAngleRateMaxValue)
            //    * (defenseTower.maxUpAngle - defenseTower.maxDownAngle)
            //    + defenseTower.maxDownAngle;
            bitIO.date = lDate;
            var lNewAngleRateValue = bitIO.readToInt(22);
            var lNewLifeRateValue = bitIO.readToInt(9);
            defenseTower.fire = bitIO.readToInt(1) == 1;

            defenseTower.aimAngle = ((float)lNewAngleRateValue / (float)aimAngleRateMaxValue)
                * (defenseTower.maxUpAngle - defenseTower.maxDownAngle)
                + defenseTower.maxDownAngle;

            life.rate = (float)lNewLifeRateValue / (float)lifeRateMaxValue;
            //print("fire:" + defenseTower.fire
            //    + " life.rate:" + life.rate
            //    + " aimAngle:" + defenseTower.aimAngle);
        }
    }
}