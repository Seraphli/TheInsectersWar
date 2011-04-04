using UnityEngine;
using System.Collections;

public class StrongholdOffLineSetting : MonoBehaviour
{
    [System.Serializable]
    public class StrongholdData
    {
        public Stronghold stronghold;
        public Transform soldierFactoryPosition;
        public Race race;
        public string soldierName;
    }

    [System.Serializable]
    public class OffLineData
    {
        public Race playerChoose;
        public StrongholdData[] stronghold;
    }

    public OffLineData[] setting;

    void createSetting()
    {
        Race lPlayerChoose = GameScene.Singleton.playerInfo.race;
        foreach (var lData in setting)
        {
            if (lPlayerChoose == lData.playerChoose)
            {
                var lSoldierFactoryState = SoldierFactoryState.getSingleton();
                foreach (var lStrongholdData in lData.stronghold)
                {
                    lStrongholdData.stronghold.buildRace(lStrongholdData.race);
                    if (lStrongholdData.soldierFactoryPosition
                        && lStrongholdData.soldierName.Length != 0)
                        lSoldierFactoryState.createFactory(
                            lStrongholdData.race,
                            lStrongholdData.soldierFactoryPosition.position,
                            lStrongholdData.soldierName,
                            lStrongholdData.stronghold);
                }
                break;
            }
        }
    }

    void Update()
    {
        //因为要在所需的系统初始化后执行

        if(Network.peerType==NetworkPeerType.Disconnected)
        {
            createSetting();
        }
        
        enabled = false;
    }
}