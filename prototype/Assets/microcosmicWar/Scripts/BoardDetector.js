
var boardPlayer:GameObject;
protected var originalPosition:Vector3;
//用于探测板是否在脚下
protected var inited=false;

function Start()
{
	Board.turnOffCollisionWithAllBaord( boardPlayer );
	originalPosition=transform.localPosition;
	inited=true;
	//print(originalPosition);
}

function down()
{
	if(inited)
		transform.localPosition.y=originalPosition.y-1;
	//print("down:"+transform.localPosition.y);
}

function recover()
{
	
	if(inited)
		transform.localPosition.y=originalPosition.y;
	//print("recover:"+transform.localPosition.y);
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
	//print("OnTriggerExit");
	var lBoard:Board = other.GetComponent(Board);
	if(lBoard)
	{
		lBoard.turnOffCollision(boardPlayer);
	}
}
