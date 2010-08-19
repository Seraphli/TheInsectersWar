
//����2D��Ϸ��,��͸������ȥ�������İ�

static var sBoardList=Array();

static function turnOffCollisionWithAllBaord( pCollider :Collider )
{
	//print(sBoardList);
	for(var i:GameObject in sBoardList)
	{
		Physics.IgnoreCollision(i.collider, pCollider);
	}
}

static function turnOffCollisionWithAllBaord( pGameObject :GameObject )
{
	turnOffCollisionWithAllBaord(pGameObject.collider);
}

//�ǵ��ڳ�������ǰ����
static function clearList()
{
	sBoardList.Clear();
}

function Awake()
{
	sBoardList.Add(gameObject);
}

function turnOffCollision( pGameObject :GameObject )
{
	Physics.IgnoreCollision(gameObject.collider, pGameObject.collider);
}

function turnOnCollision( pGameObject :GameObject )
{
	Physics.IgnoreCollision(gameObject.collider, pGameObject.collider,false);
}


function Start()
{
	collisionLayer.addCollider(gameObject);
}