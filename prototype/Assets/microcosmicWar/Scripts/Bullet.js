
var aliveTime=5.0;

function Update () 
{
	aliveTime-= Time.deltaTime;
	if(aliveTime<0)
		Destroy (gameObject);
}

function OnCollisionEnter (collision : Collision)
{
	Destroy (gameObject);
}