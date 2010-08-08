
var aliveTime=5.0;
var harmVale=1.0;

function Start()
{
	collisionLayer.addCollider(gameObject);
	//if(!zzCreatorUtility.isHost())
	//	Destroy(this);
}

function Update()
{
	if(zzCreatorUtility.isHost())
	{
		aliveTime-= Time.deltaTime;
		if(aliveTime<0)
			//zzCreatorUtility.Destroy (gameObject);
			lifeEnd();
	}
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
	zzCreatorUtility.sendMessage(gameObject,"lifeEndImp");
}

@RPC
function lifeEndImp()
{
	particleEmitterDetach();
	//zzCreatorUtility.sendMessage(gameObject,"particleEmitterDetach");
	Destroy(gameObject);
}

////@RPC
function particleEmitterDetach()
{
	var lTransform:Transform = transform.Find("particleEmitter");
	if(lTransform)
	{
		//��ֹͬʱִ��Update �� OnCollisionEnter�����
		var lParticleEmitter:ParticleEmitter=lTransform.gameObject.GetComponent(ParticleEmitter);
		lParticleEmitter.emit=false;
		lTransform.parent =null;
	}
	//�������������� ��ʱ
}