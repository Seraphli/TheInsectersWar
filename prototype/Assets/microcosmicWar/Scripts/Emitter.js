
var bulletPrefab:GameObject;
var bulletSpeed=3.0;

var bulletLayer=0;

var fireSound:AudioSource;

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
		clone.GetComponentInChildren(Rigidbody).velocity=transform.right*bulletSpeed;
		//clone.velocity=transform.forward;
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

virtual function getForward()
{
	return transform.right;
}

virtual function getFireRay()
{
	return Ray(transform.position,transform.right.normalized);
}