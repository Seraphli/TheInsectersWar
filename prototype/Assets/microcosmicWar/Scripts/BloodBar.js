
protected var life:Life;

protected var fullWidth:float;

protected var localPostion:Vector3;

function Start()
{
	fullWidth = transform.localScale.x;
	localPostion = transform.localPosition;
}

//function Update () {
//}

function UpdateBar()
{
	SetRate(life.getBloodValue()/life.getFullBloodValue());
}

function SetRate(pRate:float)
{
	transform.localScale.x=fullWidth*pRate;
	transform.localPosition.x = localPostion.x+( transform.localScale.x-fullWidth )/2.0;
}

//func

function setLife(pLife:Life)
{
	life=pLife;
	life.setBloodValueChangeCallback(UpdateBar);
}