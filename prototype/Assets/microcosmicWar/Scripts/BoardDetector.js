
var boardPlayer:GameObject;
//用于探测板是否在脚下

function Start()
{
	Board.turnOffCollisionWithAllBaord( boardPlayer );
}

function OnTriggerEnter (other : Collider)
{
	print("OnTriggerEnter");
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
