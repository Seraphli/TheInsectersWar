var target:Transform;

var useRelative=false;
var relativePostion:Vector3;

function Start()
{
	if(useRelative)
		relativePostion=transform.position - target.position ;
}


function Update () 
{    
	if(target && useRelative)
	{
		transform.position=target.position+relativePostion;
		//transform.position.x = target.position.x+relativePostion.x;
		//transform.position.y = target.position.y+relativePostion.y;
	}
	else if(target)
	{
		transform.position.x = Mathf.Lerp(transform.position.x, target.position.x, 0.75);
		transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, 0.75); 
	}
}