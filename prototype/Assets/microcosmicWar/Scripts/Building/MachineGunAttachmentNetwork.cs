using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineGunAttachmentNetwork:MonoBehaviour
{
    public DefenseTower defenseTower;
    public Life life;
    public NetworkDisappear networkDisappear;

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
                if (transform.parent && transform.parent.GetComponent<StrongholdUpdate>())
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
            networkView.enabled = true;

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

        }
        stream.Serialize(ref lDate);
        if(stream.isReading)
        {
            networkDisappear.appear();
            bitIO.date = lDate;
            var lNewAngleRateValue = bitIO.readToInt(22);
            var lNewLifeRateValue = bitIO.readToInt(9);
            defenseTower.fire = bitIO.readToInt(1) == 1;

            defenseTower.aimAngle = ((float)lNewAngleRateValue / (float)aimAngleRateMaxValue)
                * (defenseTower.maxUpAngle - defenseTower.maxDownAngle)
                + defenseTower.maxDownAngle;

            life.rate = (float)lNewLifeRateValue / (float)lifeRateMaxValue;

        }
    }
}