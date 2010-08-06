
var bulletPrefab:GameObject;
var bulletSpeed=3.0;

var bulletLayer=0;

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
}

virtual function setBulletLayer(pBulletLayer:int)
{
	//print("Emit.setBulletLayer");
	bulletLayer= pBulletLayer;
}