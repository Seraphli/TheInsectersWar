
var bulletPrefab:GameObject;
var bulletSpeed=20.0;

//…‰≥Ã
var shootRange:float = 10.0;

var bulletLayer=0;

var fireSound:AudioSource;

protected var injureInfo:Hashtable;
protected var bulletAliveTime:float;

protected function initBulletNullFunc(pBullet:Bullet)
{
}

var initBulletFunc = initBulletNullFunc;

function setInitBulletFunc(pFunc)
{
	initBulletFunc = pFunc;
}

virtual function setInjureInfo(pInjureInfo:Hashtable)
{
	injureInfo = pInjureInfo;
}

function Start()
{
	bulletAliveTime = shootRange/bulletSpeed;
	//print(""+bulletAliveTime+"="+shootRange"//"+bulletSpeed);
}

function Update () 
{
}

virtual function EmitBullet()
{
	if(zzCreatorUtility.isHost())
	{
		var clone : GameObject;
		clone = zzCreatorUtility.Instantiate(bulletPrefab, transform.position, transform.rotation,0);
		clone.layer=bulletLayer;
		
		//print(transform.localToWorldMatrix.MultiplyVector(Vector3(1,0,0)) );
		//var lRigidbody:Rigidbody = clone.GetComponentInChildren(Rigidbody);
		//lRigidbody.velocity=transform.localToWorldMatrix.MultiplyVector(Vector3(1,0,0))*bulletSpeed;
		//clone.velocity=transform.forward;
		var pBullet:Bullet = clone.GetComponentInChildren(Bullet);
		pBullet.setAliveTime(bulletAliveTime);
		//pBullet.setForward(getForward());
		//pBullet.setSpeed(bulletSpeed);
		pBullet.setForwardVelocity(getForward()*bulletSpeed);
		if(injureInfo)
		{
			pBullet.setInjureInfo(injureInfo);
		}
	}
	if(fireSound)
	{
		fireSound.Play();
	}
}

virtual function setBulletLayer(pBulletLayer:int)
{
	//print("Emit.setBulletLayer");
	bulletLayer= pBulletLayer;
}

virtual function getForward():Vector3
{
	//return transform.right;
	return transform.localToWorldMatrix.MultiplyVector(Vector3(1,0,0));
}

virtual function getFireRay():Ray
{
	return Ray(transform.position,getForward());
}

function OnDrawGizmosSelected() 
{
	Gizmos.DrawLine(transform.position,transform.position+getForward()*shootRange);
}