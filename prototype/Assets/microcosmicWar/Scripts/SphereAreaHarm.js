
//用于爆炸的破坏(去血效果)

var onlyOnce=true;
var harmRadius=5.0;
var harmValueInCentre=10.0;

//[Life]=Distance  现在用于存储美工有生命挝�,离爆炸点的最近距离
protected var injuredLifeInTheFrame=Hashtable();

function Start () 
{
}

function Update()
{
	var lColliderList: Collider[] = Physics.OverlapSphere(transform.position,harmRadius);
	//搜寻范围内的Life,并寻找最短距离
	for(var i:Collider in  lColliderList)
	{
		var lLife:Life = Life.getLifeFromTransform(i.transform);
		if(lLife)
		{
			//lLife.injure(harmValueInCentre);
			var lClosestPoint = i.ClosestPointOnBounds(transform.position);
			var lDistance = Vector3.Distance(lClosestPoint, transform.position);
			
			//因为一个有Life的物体上,可能有多个Collider
			if(injuredLifeInTheFrame.ContainsKey(lLife))
			{
				if(injuredLifeInTheFrame[lLife]>lDistance)
				{
					//print(""+injuredLifeInTheFrame[lLife]+">"+lDistance);
					injuredLifeInTheFrame[lLife] = lDistance;
				}
			}
			else
				injuredLifeInTheFrame[lLife] = lDistance;
		}
		
	}
	
	//执行伤害
	for(var lifeToDistance:System.Collections.DictionaryEntry in injuredLifeInTheFrame)
	{
		// The hit points we apply fall decrease with distance from the explosion point
		var lLifeImp:Life = lifeToDistance.Key;
		var lDistanceImp:float = lifeToDistance.Value;
		var lHarmRange:float = 1.0 - Mathf.Clamp01(lDistanceImp / harmRadius);
		//print(lDistanceImp);
		//print(lHarmRange);
		//print(lHarmRange *harmValueInCentre );
		lLifeImp.injure( lHarmRange *harmValueInCentre );
	}
	
	injuredLifeInTheFrame.Clear();
	if(onlyOnce)
	{
		Destroy(this);
	}

}


function OnDrawGizmosSelected() 
{
	Gizmos.DrawWireSphere(transform.position,harmRadius);
}