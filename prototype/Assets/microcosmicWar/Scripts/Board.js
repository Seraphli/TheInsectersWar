
//用于2D游戏中,穿透可跳上去跳下来的板

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

//记得在场景结束前调用
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