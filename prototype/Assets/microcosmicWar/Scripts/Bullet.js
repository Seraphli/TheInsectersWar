
@script RequireComponent(Rigidbody)
@script RequireComponent(Life)
@script RequireComponent(destroyWhenDie)

var aliveTime=5.0;
var harmVale:int=1.0;
var bulletLife:Life;
var shape:GameObject;

protected var injureInfo:Hashtable;


var bulletRigidbody:Rigidbody ;

function Awake()
{
	bulletRigidbody= gameObject.GetComponent(Rigidbody);
	bulletLife = gameObject.GetComponent(Life);
	bulletLife.addDieCallback(lifeEndImp);
}

function setInjureInfo(pInjureInfo:Hashtable)
{
	injureInfo = pInjureInfo;
}

function Start()
{
	if(shape)
	{
		//print("collisionLayer.addCollider(shape)");
		shape.layer = gameObject.layer;
		collisionLayer.addCollider(shape);
	}
	else
	{
		//print("collisionLayer.addCollider(gameObject)");
		collisionLayer.addCollider(gameObject);
	}
	//print(gameObject.layer);
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
	/*
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
	*/
	var lLife:Life=Life.getLifeFromTransform(collision.transform);
	
	if(lLife)
		lLife.injure(harmVale,injureInfo);
		
	//if(zzCreatorUtility.isHost())
	lifeEnd();
}

function lifeEnd()
{
//	zzCreatorUtility.sendMessage(gameObject,"lifeEndImp");
	bulletLife.setBloodValue(0);
}

@RPC
function lifeEndImp()
{
	particleEmitterDetach();
	//zzCreatorUtility.sendMessage(gameObject,"particleEmitterDetach");
	//Destroy(gameObject);
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

function getForward():Vector3
{
	return transform.rotation*Vector3.right;
}

//设置子弹飞的方向
function setForward(pForward:Vector3)
{
	pForward.z=0;
	var lRotation = Quaternion ();
	lRotation.SetFromToRotation(Vector3.right,pForward);
	//transform.rotation.SetFromToRotation(Vector3.right,pForward);
	transform.rotation=lRotation;
	var lSpeed:float = bulletRigidbody.velocity.magnitude;
	bulletRigidbody.velocity = pForward.normalized*lSpeed;
}


//设置子弹飞的方向和速度
function setForwardVelocity(pVelocity:Vector3)
{
	pVelocity.z=0;
	transform.rotation.SetFromToRotation(Vector3.right,pVelocity);
	bulletRigidbody.velocity = pVelocity;
}

function OnDrawGizmosSelected() 
{
	Gizmos.DrawLine(transform.position,transform.position+getForward()*3 );
}