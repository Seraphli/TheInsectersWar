
class zzDetectorContainer extends zzDetectorBase
{
	var subDetectorList:zzDetectorBase[];

	function Start()
	{
		//创建子探测器列表, subDetectorList; 按优先级排序
		var lDetectorList = ArrayList();
		var i = 0;
		for(var lTransform:Transform in transform)
		{
			var lDetector:zzDetectorBase = lTransform.GetComponent(zzDetectorBase);
			if(lDetector)
			{
				//按优先级排序,大的排在前面.先检测
				for(i=0;i<lDetectorList.Count;++i)
				{
					if(lDetector.getPriority()>lDetectorList[i].getPriority())
					{
						lDetectorList.Insert(i,lDetector);
						lDetector=null;//来说明已添加
						break;
					}
				}
				if(lDetector)
					lDetectorList.Add(lDetector);
			}
		}
		
		subDetectorList = new zzDetectorBase[lDetectorList.Count];
		for(i=0;i<lDetectorList.Count;++i)
		{
			subDetectorList[i]=lDetectorList[i];
		}
	}


	virtual function detector(pMaxRequired:int,pLayerMask:LayerMask):Collider[]
	{
		var lOut = new Collider[0];
		for(var subDetector in subDetectorList)
		{
			var lSubResult:Collider[] = subDetector.detector(pMaxRequired,pLayerMask);
			pMaxRequired -= lSubResult.Length;
			lOut+=lSubResult;
			if(pMaxRequired<=0)
				break;
		}
		return lOut;
	}
	
}