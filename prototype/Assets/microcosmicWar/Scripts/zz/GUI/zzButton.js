
class zzButton extends zzInterfaceGUI
{
	//var position  = Rect ();
	var content  : GUIContent;
	var useDefaultStyle=false;
	var  style : GUIStyle;
	var clickCall=nullGUICallback;

	virtual function impGUI():void
	{
		if( _drawButton() )
			clickCall(this);
	}
	
	function _drawButton():boolean
	{
		if(useDefaultStyle)
			return GUI.Button(getPosition() , content);
		return GUI.Button (getPosition() , content , style) ;
	}
	
	virtual function setText(pText:String)
	{
		content.text = pText;
	}
	
	virtual function setClickCall( pCall)
	{
		clickCall = pCall;
	}
}