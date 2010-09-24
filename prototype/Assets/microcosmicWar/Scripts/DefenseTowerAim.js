
var target:Transform;
var bullet:Bullet;
var fireDeviation = 4.0;
var defenseTower:DefenseTower;

function Start()
{
	if(!defenseTower)
		defenseTower= GetComponent(DefenseTower);
}

function setTarget(pTarget:Transform)
{
	target = pTarget;
}

function Update ()
{
	if(target)
	{
		defenseTower.takeAim(target.position,fireDeviation);
	}
}