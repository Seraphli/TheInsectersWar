

//������С��������Ǯ
var shootAward=1;
var life:Life;

function Start()
{
	life = gameObject.GetComponentInChildren(Life);
	life.addDieCallback(deadAction);
}

//�������Ļص���ʹ��
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