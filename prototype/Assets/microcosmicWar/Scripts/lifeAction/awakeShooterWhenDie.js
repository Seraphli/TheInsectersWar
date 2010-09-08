

//打死此小兵后奖励的钱
var shootAward=1;
var life:Life;

function Start()
{
	life = gameObject.GetComponentInChildren(Life);
	life.addDieCallback(deadAction);
}

//在死亡的回调中使用
function deadAction()
{
	var lInjureInfo:Hashtable = life.getInjureInfo();
	if(lInjureInfo && lInjureInfo.ContainsKey("bagControl"))
	{
		var lBagControl:zzItemBagControl = lInjureInfo["bagControl"];
		lBagControl.addMoney(shootAward);
	}
}

function Update () {
}