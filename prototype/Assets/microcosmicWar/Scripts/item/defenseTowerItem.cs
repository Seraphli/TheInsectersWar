
using UnityEngine;
using System.Collections;

class defenseTowerItem : IitemObject
{
    public string defenseTowerTypeName;
    protected Vector3 towerPosition;
    protected int towerFace;
    protected GameObject useObject;

    public override bool canUse(GameObject pGameObject)
    {
        Hero hero = pGameObject.GetComponentInChildren<Hero>();
        int face = (int)hero.getFace();
        Vector3 position = pGameObject.transform.position;

        position.x += hero.getFaceDirection() * 3;
        position.y += 2;
        RaycastHit lHit;
        if (Physics.Raycast(position, new Vector3(0, -1, 0), out lHit, 4, layers.boardValue))
        {
            //FIXME_VAR_TYPE lRange= Vector2(0,2,0);
            //Debug.Log(""+(lHit.point+Vector3(0,4,0))+(lHit.point+Vector3(0,0.1f,0)));
            //if(!Physics.CheckCapsule  (lHit.point+Vector3(0,3,0), lHit.point+Vector3(0,-1,0), 0.25f ) )
            if (!Physics.CheckSphere(lHit.point + new Vector3(0, 2, 0), 1.8f, layers.boardValue))
            {
                towerPosition = lHit.point;
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
        lInfo["rotation"] = new Quaternion();
        lInfo["face"] = towerFace;
        lInfo["layer"] = useObject.layer;
        lInfo["adversaryLayer"] = PlayerInfo.getAdversaryRaceLayer(useObject.layer);
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