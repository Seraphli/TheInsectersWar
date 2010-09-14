
var myGUI:zzInterfaceGUI;

function impGUI()
{
	//GUI.depth = myGUI.depth;
	
	if(myGUI.getVisible())
		myGUI.impGUI();
}

function getDepth()
{
	return myGUI.depth;
}

function setGUI(pGUI:zzInterfaceGUI)
{
	myGUI= pGUI;
}

function getGUI()
{
	return myGUI;
}