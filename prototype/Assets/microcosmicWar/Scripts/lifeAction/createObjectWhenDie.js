
var objectToCreate:GameObject;
var life:Life;

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