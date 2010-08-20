
var itemObject:IitemObject;

function canUse(pGameObject:GameObject):boolean
{
	return itemObject.canUse(pGameObject);
}

virtual function use()
{
	itemObject.use();
}