
class zzRayDetector extends zzDetectorBase
{
	//var _from:Transform;
	var _to:Transform;

	virtual function detector(pMaxRequired:int,pLayerMask:LayerMask):Collider[]
	{
		var lHits : RaycastHit[];
		lHits = Physics.RaycastAll (getOrigin(), getDirection(), getDistance(),pLayerMask);
		var lOutNum:int;
		lOutNum = Mathf.Min(pMaxRequired,lHits.Length);
		var lOut = new Collider[lOutNum];
		for(var i = 0; i<lOutNum; ++i)
		{
			lOut[i]=lHits[i].collider;
		}
		return lOut;
	}
	
	virtual function getOrigin():Vector3
	{
		//return _from.position;
		return transform.position;
	}
	
	virtual function getDirection():Vector3
	{
		return ( _to.position -transform.position ).normalized;
	}
	
	virtual function getDistance():float
	{
		return ( _to.position -transform.position ).magnitude;
	}
	
	
	//function OnDrawGizmosSelected() 
	function OnDrawGizmos() 
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, _to.position);
	}
}