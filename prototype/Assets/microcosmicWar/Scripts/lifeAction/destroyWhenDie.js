

function Start()
{
	life = gameObject.GetComponentInChildren(Life);
	life.addDieCallback(deadAction);
}


//在死亡的回调中使用
function deadAction()
{
	Destroy(gameObject);
}