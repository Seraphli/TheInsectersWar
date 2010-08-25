

virtual function canUse(pGameObject:GameObject):boolean
{
}

virtual function use()
{
}

function Reset()
{
	var lItemObjectImp:ItemObjectImp = zzUtilities.needComponent(gameObject,ItemObjectImp);
	lItemObjectImp.setItemObject(this);
}