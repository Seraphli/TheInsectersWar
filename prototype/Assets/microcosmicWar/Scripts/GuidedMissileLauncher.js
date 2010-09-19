

class GuidedMissileLauncher extends DefenseTower
{
	var fireInterval:float[];
	
	var fireTimer:zzTimer;

	//现在间隔的索引
	protected var fireIntervalIndex=-1;
	
	protected function moveToNextIntervalIndex():int
	{
		if(fireIntervalIndex>=fireInterval.Length)
			fireIntervalIndex=0;
		return fireIntervalIndex++;
		
	}
	
	protected function getIntervalAndMove():float
	{
		return fireInterval[moveToNextIntervalIndex()];
	}
	
	protected function fireAndSetNextTime()
	{
		emitter.EmitBullet();
		fireTimer.setInterval(getIntervalAndMove());
	}
	
	function Start()
	{
		super.Start();
		if(!fireTimer)
			fireTimer = gameObject.AddComponent(zzTimer);
		fireTimer.setImpFunction(fireAndSetNextTime);
		fireTimer.setInterval(getIntervalAndMove());
	}

	//virtual function Update () 
	//{
	//	super.Update();
	//}


}