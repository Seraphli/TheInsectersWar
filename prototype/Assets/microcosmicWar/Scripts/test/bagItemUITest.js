
var moveLeftkey : KeyCode;
var moveRightkey : KeyCode;
var useItemKey : KeyCode;


var bagItemUI:BagItemUI;

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