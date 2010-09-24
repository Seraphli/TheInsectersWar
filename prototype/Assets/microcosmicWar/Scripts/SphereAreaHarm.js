
//”√”⁄±¨’®µƒ∆∆ªµ(»•—™–ßπ˚)

var onlyOnce=true;
var harmRadius=5.0;
var harmValueInCentre=10.0;

// ∑˚∫œ ¥À mask µƒŒÔÃÂ,÷¥––…À∫¶
var harmLayerMask:int;

//[Life]=Distance  œ÷‘⁄”√”⁄¥Ê¥¢√¿π§”–…˙√¸ŒŒÔ,¿Î±¨’®µ„µƒ◊ÓΩ¸æ‡¿Î
protected var injuredLifeInTheFrame=Hashtable();


function setHarmLayerMask(pMark:int)
{
	harmLayerMask = pMark;
	//print(harmLayerMask);
	//print(transform.position);
}

function Update()
{
/*
	print("harmLayerMask");
	print(transform.position);
	print("harmRadius:"+harmRadius);
	print("harmValueInCentre:"+harmValueInCentre);
*/
	var lColliderList: Collider[] = Physics.OverlapSphere(transform.position,harmRadius,harmLayerMask);
	//À——∞∑∂Œßƒ⁄µƒLife,≤¢—∞’“◊Ó∂Ãæ‡¿Î
	for(var i:Collider in  lColliderList)
	{
	
		//Trigger “≤ª·±ªÃΩ≤‚µΩ
		if(i.isTrigger)
			continue;
		var lLife:Life = Life.getLifeFromTransform(i.transform);
		if(lLife)
		{
			//lLife.injure(harmValueInCentre);
			var lClosestPoint = i.ClosestPointOnBounds(transform.position);
			var lDistance = Vector3.Distance(lClosestPoint, transform.position);
			
			//“ÚŒ™“ª∏ˆ”–LifeµƒŒÔÃÂ…œ,ø…ƒ‹”–∂‡∏ˆCollider
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
	
	//÷¥––…À∫¶
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