
var myGUI:zzInterfaceGUI;

function impGUI()
{
	//GUI.depth = myGUI.depth;
	
	if(myGUI.getVisible())
	{
		var lSkin:GUISkin = myGUI.getSkin();
		if(lSkin)
		{
			//สนำร Skin
			var lPreSkin :GUISkin = GUI.skin ;
			GUI.skin = lSkin;
			myGUI.impGUI();
			GUI.skin = lPreSkin;
		}
		else
			myGUI.impGUI();
	}
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