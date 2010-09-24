
var myGUI:zzInterfaceGUI;

static function impGUI(pMyGUI:zzInterfaceGUI)
{
	//GUI.depth = myGUI.depth;
	
	if(pMyGUI.getVisible())
	{
		var lSkin:GUISkin = pMyGUI.getSkin();
		if(lSkin)
		{
			//สนำร Skin
			var lPreSkin :GUISkin = GUI.skin ;
			GUI.skin = lSkin;
			pMyGUI.impGUI();
			GUI.skin = lPreSkin;
		}
		else
			pMyGUI.impGUI();
	}
}

function impGUI()
{
	impGUI(myGUI);
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