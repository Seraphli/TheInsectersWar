
using UnityEngine;
using System.Collections;

class defenseTowerItem : IitemObject
{
    public string defenseTowerTypeName = "unnamed defenseTowerItem";
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
        return Physics.CheckSphere(position + new Vector3(0, 2, 0), 1.8f, layers.standPlaceValue);
    }

    public static bool canBuild(GameObject pGameObject,out Vector3 position)
    {
        return getBuildPosition(pGameObject, out position) && (!haveBoardOverhead(position));
    }

    public override bool canUse(GameObject pGameObject)
    {
        Hero hero = pGameObject.GetComponentInChildren<Hero>();
        int face = (int)hero.getFace();

        //Hero hero = pGameObject.GetComponentInChildren<Hero>();
        //Vector3 position = pGameObject.transform.position;

        //position.x += hero.getFaceDirection() * 3;
        //position.y += 2;
        //RaycastHit lHit;
        //if (Physics.Raycast(position, new Vector3(0, -1, 0), out lHit, 4, layers.boardValue))
        if (canBuild(pGameObject, out towerPosition))
        {
            //FIXME_VAR_TYPE lRange= Vector2(0,2,0);
            //Debug.Log(""+(lHit.point+Vector3(0,4,0))+(lHit.point+Vector3(0,0.1f,0)));
            //if(!Physics.CheckCapsule  (lHit.point+Vector3(0,3,0), lHit.point+Vector3(0,-1,0), 0.25f ) )
            //if (!haveBoardOverhead(towerPosition))
            //{
                //towerPosition = lHit.point;
                towerFace = face;
                //Debug.Log(face);
                useObject = pGameObject;
                //Debug.Log("can use");
                return true;
            //}
        }
        //Debug.Log(""+position+","+lHit.point);
        return false;
    }

    public override void use()
    {
        Hashtable lInfo = new Hashtable();
        lInfo["creatorName"] = defenseTowerTypeName;
        lInfo["position"] = towerPosition;
        lInfo["rotation"] = new Quaternion();
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