
var target:Transform;
var bullet:Bullet;

function Start()
{
	//target = gameObject.Find("BulletFollowAITest").transform;
}

function setTarget(pTarget:Transform)
{
	target = pTarget;
}

function Update ()
{
	if(target)
	{
		var lToAim = target.position - bullet.transform.position;
		bullet.setForward( Vector3.Lerp(bullet.getForward(),lToAim.normalized,2*Time.deltaTime) );
	}
	//print("getForward"+bullet.getForward());
	//print(lToAim );
	//print(Vector3.Lerp(bullet.getForward(),lToAim,0.6*Time.deltaTime));
}