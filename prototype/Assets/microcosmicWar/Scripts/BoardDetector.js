
var boardPlayer:GameObject;
//����̽����Ƿ��ڽ���

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
