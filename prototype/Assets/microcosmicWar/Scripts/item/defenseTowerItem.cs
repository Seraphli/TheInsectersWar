
using UnityEngine;
using System.Collections;

class defenseTowerItem : IitemObject
{
    public string defenseTowerTypeName = "unnamed defenseTowerItem";
    public int needEnergy = 10;
    protected Vector3 towerPosition;
    protected int towerFace;
    protected GameObject useObject;

    public class BuildingInfo
    {
        Vector3 position;
        int face;
        GameObject creator;
    }

    public static bool getBuildPosition(GameObject pGameObject,out Vector3 position)
    {
        
        Hero hero = pGameObject.GetComponentInChildren<Hero>();
        Vector3 lPosition = pGameObject.transform.position;

        lPosition.x += hero.getFaceDirection() * 3;
        lPosition.y += 2;
        RaycastHit lHit;

        if (
            Physics.Raycast(lPosition,
                new Vector3(0, -1, 0),
                out lHit,
                4, layers.standPlaceValue)
            )
        {
            position = lHit.point;
            return true;
        }
        position = Vector3.zero;
        return false;
    }

    public static bool haveBoardOverhead(Vector3 position)
    {
        //2m 高度内 不能有 站立处
        return Physics.CheckSphere(position + new Vector3(0, 2, 0), 1.8f, layers.standPlaceValue);
    }

    public static bool canBuild(GameObject pGameObject,out Vector3 position)
    {
        return getBuildPosition(pGameObject, out position) && (!haveBoardOverhead(position));
    }

    //附加检测是否在自己领地内
    public static Stronghold canBuildCheckManor(GameObject pGameObject, out Vector3 position)
    {
        if (canBuild(pGameObject, out position) )
        {

            Stronghold lStronghold = null;
            Collider[] lIsInSelfZone = Physics.OverlapSphere(position,0.1f, layers.manorValue);
            if (lIsInSelfZone.Length != 0)
            {
                lStronghold = lIsInSelfZone[0].transform.parent.GetComponent<Stronghold>();
            }

            if (lStronghold
                && lStronghold.occupied == true
                && lStronghold.owner == PlayerInfo.getRace(pGameObject.layer))
                return lStronghold;
        }
        return null;
    }

    public override bool canUse(GameObject pGameObject)
    {
        Hero hero = pGameObject.GetComponentInChildren<Hero>();
        int face = (int)hero.getFace();

        var lStronghold = canBuildCheckManor(pGameObject, out towerPosition);
        if (lStronghold )
        {
            var lEnergyValue = lStronghold.GetComponent<zzSceneObjectMap>()
                    .getObject("energyValue").GetComponent<RestorableValue>();
            if (lEnergyValue.nowValue >= needEnergy)
            {
                lEnergyValue.reduce(needEnergy);
                //towerPosition = lHit.point;
                towerFace = face;
                //Debug.Log(face);
                useObject = pGameObject;
                //Debug.Log("can use");
                return true;
            }
        }
        //Debug.Log(""+position+","+lHit.point);
        return false;
    }

    public override void use()
    {
        Hashtable lInfo = new Hashtable();
        lInfo["creatorName"] = defenseTowerTypeName;
        lInfo["position"] = towerPosition;
        //lInfo["rotation"] = new Quaternion();
        lInfo["face"] = towerFace;
        lInfo["layer"] = useObject.layer;
        lInfo["race"] = (int)PlayerInfo.getRace(useObject.layer);
        //lInfo["adversaryLayerValue"] = PlayerInfo.getAdversaryRaceLayer(useObject.layer);
        zzGameObjectCreator.getSingleton().create(lInfo);
        //zzGameObjectCreator.getSingleton().create({
        //    //"creatorName":"AiMachineGun",
        //    "creatorName":defenseTowerTypeName,
        //    "position":towerPosition,
        //    "rotation":Quaternion(),
        //    "face":towerFace,
        //    "layer":useObject.layer,
        //    "adversaryLayer":PlayerInfo.getAdversaryRaceLayer(useObject.layer)
        //});
    }
};