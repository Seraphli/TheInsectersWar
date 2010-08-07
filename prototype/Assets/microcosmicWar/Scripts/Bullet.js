
var aliveTime=5.0;
var harmVale=1.0;

function Start()
{
	collisionLayer.addCollider(gameObject);
	if(!zzCreatorUtility.isHost())
		Destroy(this);
}

function Update()
{
	//if(zzCreatorUtility.isHost())
	//{
		aliveTime-= Time.deltaTime;
		if(aliveTime<0)
			//zzCreatorUtility.Destroy (gameObject);
			lifeEnd();
	//}
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
		
	//if(zzCreatorUtility.isHost())
	lifeEnd();
}

function lifeEnd()
{
	particleEmitterDetach();
	zzCreatorUtility.Destroy (gameObject);
}

function particleEmitterDetach()
{
	print("particleEmitterDetach");
	var lTransform:Transform = transform.Find("particleEmitter");
	var particleEmitter:ParticleEmitter=lTransform.gameObject.GetComponent(ParticleEmitter);
	particleEmitter.emit=false;
	lTransform.parent =null;
	//脱离所有子物体 临时
}