
var interval:float;

protected var frequencyTimer=zzFrequencyTimer();

function Start()
{
	setInterval(interval);
}

function setImpFunction(pFunc)
{
	frequencyTimer.setImpFunction(pFunc);
}

function setInterval( pInterval: float )
{
	//print(1.0/pInterval);
	//防止在 Start 前执行,使interval未赋值执行setInterval
	interval = pInterval;
	frequencyTimer.setFrequencyOfImp(1.0/pInterval);
}

function Update ()
{
	frequencyTimer.Update ();
}