
//Խ�ߵ����ȼ� Խ���ȴ���
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