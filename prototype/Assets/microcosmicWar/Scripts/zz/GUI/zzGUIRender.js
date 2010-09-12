
@script ExecuteInEditMode()

class zzGUIRender extends zzGUIContainer
{
	function OnGUI () 
	{
		impSubs();
	}
	
	virtual function getWidth()
	{
		return Screen.width;
	}
	
	virtual function getHeight()
	{
		return Screen.height;
	}
}