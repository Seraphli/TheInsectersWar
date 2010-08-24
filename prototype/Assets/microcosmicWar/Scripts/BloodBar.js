
protected var life:Life;

protected var fullWidth:float;

protected var localPostion:Vector3;

//血的等级,即所用颜色的索引
protected var lifeLevel=0;

protected var bloodBarMaterials : Material[];

protected var levelRate:float;//1.0/renderer.materials.Length

var bloodColorList:Color[];

function Start()
{
	fullWidth = transform.localScale.x;
	localPostion = transform.localPosition;
	
	bloodBarMaterials=renderer.materials;
	//levelRate=1.0/bloodBarMaterials.Length;
	levelRate=1.0/(bloodColorList.Length-1);
	
	//renderer.material = bloodBarMaterials[lifeLevel];
	renderer.material.color = bloodColorList[lifeLevel];
}

//function Update () {
//}

function UpdateBar()
{
	var lFullBloodValue:float = life.getFullBloodValue();
	var lRate:float = life.getBloodValue()/lFullBloodValue;
	if(lRate<0)
		SetRate(0);
	else
		SetRate(lRate);
}

function updateLevel(pRate:float)
{
	//var lLevel:int =bloodBarMaterials.Length-1- Mathf.FloorToInt( pRate/levelRate );
	var lLevel:int =bloodColorList.Length-1- Mathf.FloorToInt( pRate/levelRate );
	if(lLevel==lifeLevel)
		return lifeLevel;
	lifeLevel=lLevel;
	//renderer.material = bloodBarMaterials[lifeLevel];
	renderer.material.color = bloodColorList[lifeLevel];
}

//pRate>=0
function SetRate(pRate:float)
{
	updateLevel(pRate);
	transform.localScale.x=fullWidth*pRate;
	transform.localPosition.x = localPostion.x+( transform.localScale.x-fullWidth )/2.0;
}

//func

function setLife(pLife:Life)
{
	life=pLife;
	life.setBloodValueChangeCallback(UpdateBar);
}