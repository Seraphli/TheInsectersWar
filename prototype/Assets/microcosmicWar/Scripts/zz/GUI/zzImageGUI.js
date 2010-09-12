
class zzImageGUI extends zzInterfaceGUI
{
	//var position : Rect;
	var image : Texture;
	var scaleMode : ScaleMode = ScaleMode.StretchToFill;
	var alphaBlend : boolean = true;
	var imageAspect : float = 0;

	virtual function impGUI()
	{
		//print("zzImageGUI impGUI");
		if(image)
			GUI.DrawTexture (getPosition() , image , scaleMode , alphaBlend , imageAspect);
	}
	
	virtual function setImage(pImage:Texture)
	{
		image=pImage;
	}
}