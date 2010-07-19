
var bulletPrefab:GameObject;
var bulletSpeed=3.0;

var bulletLayer=0;

function Update () 
{
}

function EmitBullet()
{
	var clone : GameObject;
	clone = Instantiate(bulletPrefab, transform.position, transform.rotation);
	clone.layer=bulletLayer;
	clone.GetComponentInChildren(Rigidbody).velocity=transform.right*bulletSpeed;
	//clone.velocity=transform.forward;
}

function setBulletLayer(pBulletLayer:int)
{
	bulletLayer= pBulletLayer;
}