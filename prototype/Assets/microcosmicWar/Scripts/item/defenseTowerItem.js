
class defenseTowerItem extends IitemObject
{
	var towerPosition:Vector3;
	var towerFace:int;
	var useObject:GameObject;
	
	virtual function canUse(pGameObject:GameObject):boolean
	{
		var hero:Hero = pGameObject.GetComponentInChildren(Hero);
		var face = hero.getFaceDirection();
		var position = pGameObject.transform.position;
			
		position.x+=face*3;
		position.y+=2;
		var lHit : RaycastHit;
		if (Physics.Raycast (position, Vector3(0,-1,0) , lHit, 4,layers.boardValue)) 
		{
			//var lRange = Vector2(0,2,0);
			Debug.Log(""+(lHit.point+Vector3(0,4,0))+(lHit.point+Vector3(0,0.1,0)));
			//if(!Physics.CheckCapsule  (lHit.point+Vector3(0,3,0), lHit.point+Vector3(0,-1,0), 0.25 ) )
			if(!Physics.CheckSphere(lHit.point+Vector3(0,2,0), 1.8,layers.boardValue) )
			{
				towerPosition=lHit.point;
				towerFace=face;
				useObject=pGameObject;
				Debug.Log("can use");
				return true;
			}
		}
		Debug.Log(""+position+","+lHit.point);
		return false;
	}

	virtual function use()
	{
		zzGameObjectCreator.getSingleton().create({
			"creatorName":"AiMachineGun",
			"position":towerPosition,
			"rotation":Quaternion(),
			"face":towerFace,
			"layer":useObject.layer,
			"adversaryLayer":PlayerInfo.getAdversaryRaceLayer(useObject.layer)
		});
	}
};