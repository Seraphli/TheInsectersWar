var target:Transform;


function Update () 
{    
	transform.position.x = Mathf.Lerp(transform.position.x, target.position.x, 0.75);
    transform.position.y = Mathf.Lerp(transform.position.y, target.position.y, 0.75); 
}