
class zzFrequencyTimer
{

	//ִ��Ƶ��/ÿ��ִ�еĴ���
	var frequencyOfImplement=3.0;

	//protected var timeToWait:float;//=1.0/frequencyOfImplement

	var timePos=0.0;

	protected var impFunction=zzUtilities.nullFunction;


	function setImpFunction(pFunc)
	{
		impFunction=pFunc;
	}

	function Update () 
	{
		timePos+=Time.deltaTime;
		if(timePos>1.0/frequencyOfImplement)
		{
			impFunction();
			timePos=0.0;
		}
	}

}