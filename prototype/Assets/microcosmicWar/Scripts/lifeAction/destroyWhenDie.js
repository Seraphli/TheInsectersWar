

function Start()
{
	life = gameObject.GetComponentInChildren(Life);
	life.addDieCallback(deadAction);
}


//�������Ļص���ʹ��
function deadAction()
{
	Destroy(gameObject);
}