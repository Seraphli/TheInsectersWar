
var interval:float;

protected var frequencyTimer:zzFrequencyTimer;

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
	frequencyTimer.setFrequencyOfImp(1/pInterval);
}

function Update ()
{
	frequencyTimer.Update ();
}