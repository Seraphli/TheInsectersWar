
class zzLabel extends zzInterfaceGUI
{
	//var position  = Rect ();
	var content  : GUIContent;
	var  style : GUIStyle;

	virtual function impGUI()
	{
		GUI.Button(getPosition() , content , style);
	}
	
	virtual function setText(pText:String)
	{
		content.text = pText;
	}
}