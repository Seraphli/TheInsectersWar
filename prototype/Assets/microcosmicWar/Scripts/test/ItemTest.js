
var key : KeyCode;
var itemName:String;

protected var itemIndex;
var bagControl:zzItemBagControl;

function Start()
{
	itemIndex = zzItemSystem.getSingleton().getItemTypeTable().getIndex(itemName);
}

function Update () 
{
	if(Input.GetKeyDown(key) )
	{
		//print("Input.GetKeyDown(key)");
		bagControl.useItemOne(itemIndex);
	}
}