using UnityEngine;

public class PlayerSpawn:MonoBehaviour
{
    [FieldUI("优先级")]
    public int index
    {
        set { _index = value; }
        get { return _index; }
    }

    public int _index;

    public GameObject heroSpawnPrefab;

    public HeroSpawn[] heroSpawns = new HeroSpawn[0]{};

    public HeroSpawn addPlayer(NetworkPlayer pPlayer)
    {
        var lViewID = Network.isServer?Network.AllocateViewID():NetworkViewID.unassigned;
        var lOut = createHeroSpawn(pPlayer, lViewID);
        if (Network.isServer)
        {
            networkView.RPC("createHeroSpawn", RPCMode.Others, pPlayer, lViewID);
        }
        return lOut;
    }

    [RPC]
    public HeroSpawn createHeroSpawn(NetworkPlayer pPlayer, NetworkViewID pSpawnID)
    {
        var lHeroSpawnObject = (GameObject)Instantiate(heroSpawnPrefab,
            transform.position,Quaternion.identity);
        var lHeroSpawn = lHeroSpawnObject.GetComponent<HeroSpawn>();
        lHeroSpawnObject.networkView.viewID = pSpawnID;
        var lHeroSpawnTransform = lHeroSpawnObject.transform;
        lHeroSpawnTransform.parent = transform;
        lHeroSpawnTransform.localPosition = Vector3.zero;
        lHeroSpawn.owner = pPlayer;
        System.Array.Resize(ref heroSpawns, heroSpawns.Length + 1);
        heroSpawns[heroSpawns.Length - 1] = lHeroSpawn;
        return lHeroSpawn;
    }
}