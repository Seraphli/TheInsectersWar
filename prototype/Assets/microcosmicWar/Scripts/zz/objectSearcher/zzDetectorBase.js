
//越高的优先级 越被先处理
var priority=0;

virtual function detector(pMaxRequired:int,pLayerMask:LayerMask):Collider[]
{
}

virtual function getPriority()
{
	return priority;
}


function Reset() 
{
	var lzzObjectSearcher:zzObjectSearcher = zzUtilities.needComponent(gameObject,zzObjectSearcher);
	lzzObjectSearcher.setDetector(this);
}