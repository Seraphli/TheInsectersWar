
var bulletPrefab:GameObject;
var bulletSpeed=3;

function Update () 
{
}

function EmitBullet()
{
	var clone : GameObject;
	clone = Instantiate(bulletPrefab, transform.position, transform.rotation);
	clone.GetComponentInChildren(Rigidbody).velocity=transform.right*bulletSpeed;
	//clone.velocity=transform.forward;
}