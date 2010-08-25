
class healthPackItem extends IitemObject
{
	var useObject:GameObject;
	
	virtual function canUse(pGameObject:GameObject):boolean
	{
		useObject = pGameObject;
		return true;
	}

	virtual function use()
	{
		var lLife:Life=useObject.GetComponent(Life);
		var lLifeRecover:LifeRecover = useObject.AddComponent(LifeRecover);
		lLifeRecover.setLife(lLife);
	}
};