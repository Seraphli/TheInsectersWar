
var delayTime=0.0;

function Start()
{
	var life:Life = gameObject.GetComponentInChildren(Life);
	life.addDieCallback(deadAction);
}


//�������Ļص���ʹ��
function deadAction()
{
	Destroy(gameObject,delayTime);
}