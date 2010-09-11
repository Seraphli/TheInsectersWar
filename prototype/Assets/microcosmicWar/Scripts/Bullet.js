
var aliveTime=5.0;
var harmVale:int=1.0;

protected var injureInfo:Hashtable;

function setInjureInfo(pInjureInfo:Hashtable)
{
	injureInfo = pInjureInfo;
}

function Start()
{
	collisionLayer.addCollider(gameObject);
	//if(!zzCreatorUtility.isHost())
	//	Destroy(this);
}

function setAliveTime(pAliveTime:float)
{
	//print(pAliveTime);
	aliveTime = pAliveTime;
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
	//var lLife:Life=lOwner.gameObject.GetComponentInChildren(Life);
	var lLife:Life=lOwner.gameObject.GetComponent(Life);
	
	if(!lLife)
	{
		while(lOwner.parent)
		{
			lOwner=lOwner.parent;
			lLife = lOwner.gameObject.GetComponent(Life);
			if(lLife)
				break;
		}
	}
	
	if(lLife)
		lLife.injure(harmVale,injureInfo);
		
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
		//防止同时执行Update 和 OnCollisionEnter的情况
		var lParticleEmitter:ParticleEmitter=lTransform.gameObject.GetComponent(ParticleEmitter);
		lParticleEmitter.emit=false;
		lTransform.parent =null;
	}
	//脱离所有子物体 临时
}