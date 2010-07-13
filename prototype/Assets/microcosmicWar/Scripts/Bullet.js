
var aliveTime=5.0;
var harmVale=1.0;

function Start()
{
	collisionLayer.addCollider(gameObject);
}

function Update()
{
	aliveTime-= Time.deltaTime;
	if(aliveTime<0)
		Destroy (gameObject);
}

function OnCollisionEnter(collision : Collision)
{
	//print("OnCollisionEnter");
	var lOwner:Transform = collision.transform;
	while(lOwner.parent)
		lOwner=lOwner.parent;
	var lLife=lOwner.gameObject.GetComponentInChildren(Life);
	if(lLife)
		lLife.injure(harmVale);
	Destroy(gameObject);
}