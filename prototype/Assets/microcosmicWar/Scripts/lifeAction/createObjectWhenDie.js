
var objectToCreate:GameObject;

function Start()
{
	life = gameObject.GetComponentInChildren(Life);
	life.addDieCallback(deadAction);
}


//�������Ļص���ʹ��
function deadAction()
{
	Instantiate(objectToCreate,transform.position,transform.rotation);
}