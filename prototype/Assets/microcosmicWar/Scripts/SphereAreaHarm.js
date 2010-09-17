
//用于爆炸的破坏(去血效果)

var oneTime=true;
var harmRadius=5.0;
var harmValueInCentre=10.0;

protected var injuredLifeInTheFrame=Hashtable();

function Start () 
{
}

function Update()
{
	var lColliderList: Collider[] = Physics.OverlapSphere(transform.position,harmRadius);
	for(var i:Collider in  lColliderList)
	{
		var lLife:Life = Life.getLifeFromTransform(i.transform);
		if(lLife && (!injuredLifeInTheFrame.ContainsKey(lLife)))
		{
			lLife.injure(harmValueInCentre);
			injuredLifeInTheFrame[lLife] = true;
		}
	}
	injuredLifeInTheFrame.Clear();
	if(oneTime)
	{
		Destroy(this);
	}

}


function OnDrawGizmosSelected() 
{
	Gizmos.DrawWireSphere(transform.position,harmRadius);
}