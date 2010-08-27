
var moveLeftkey = KeyCode.U;
var moveRightkey = KeyCode.I;
var useItemKey = KeyCode.H;


var bagItemUI:BagItemUI;

function Start()
{
	if(!bagItemUI)
		bagItemUI = gameObject.GetComponent(BagItemUI);
}

function Update () 
{
	if(Input.GetKeyDown(moveLeftkey) )
	{
		bagItemUI.selecteDown();
	}
	
	if(Input.GetKeyDown(moveRightkey) )
	{
		bagItemUI.selecteUp();
	}
	
	if(Input.GetKeyDown(useItemKey) )
	{
		bagItemUI.useSelected();
	}
}