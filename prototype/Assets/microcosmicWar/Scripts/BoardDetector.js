
var boardPlayer:GameObject;
protected var originalPosition:Vector3;
//用于探测板是否在脚下

function Start()
{
	Board.turnOffCollisionWithAllBaord( boardPlayer );
	originalPosition=transform.localPosition;
}

function down()
{
	transform.localPosition.y=originalPosition.y-1;
}

function recover()
{
	transform.localPosition.y=originalPosition.y;
}

function OnTriggerEnter (other : Collider)
{
	//print("OnTriggerEnter");
	var lBoard:Board = other.GetComponent(Board);
	if(lBoard)
	{
		lBoard.turnOnCollision(boardPlayer);
	}
}


function OnTriggerExit (other : Collider) 
{
	print("OnTriggerExit");
	var lBoard:Board = other.GetComponent(Board);
	if(lBoard)
	{
		lBoard.turnOffCollision(boardPlayer);
	}
}
