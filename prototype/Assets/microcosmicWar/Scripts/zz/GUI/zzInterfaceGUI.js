
//@RequireComponent(zzGUI)

enum zzGUIDockPos
{
	min,
	center,
	max,
	custom
};

class zzInterfaceGUI extends MonoBehaviour
{
	//var data:int;
	var position : Rect;
	var depth:int;
	var horizontalDockPosition:zzGUIDockPos=zzGUIDockPos.custom;
	var verticalDockPosition:zzGUIDockPos=zzGUIDockPos.custom;
	var visible:boolean = true;
	
	function setVisible(pVisible:boolean)
	{
		visible = pVisible;
	}
	
	function getVisible()
	{
		return visible;
	}
	
	virtual function impGUI()
	{
	
	}
	
	virtual function setImage(pImage:Texture)
	{
	}
	
	virtual function setText(pText:String)
	{
	}
	
	function getSubElement(pName:String):zzInterfaceGUI
	{
		var lTransform:Transform = transform.Find(pName);
		if(lTransform)
		{
			var impGUI:zzGUI = lTransform.GetComponent(zzGUI);
			if(impGUI)
				return impGUI.getGUI();
		}
		return null;
	}
	
	virtual function getPosition()
	{
		var lOut = position;
		
		//horizontal
		switch(horizontalDockPosition)
		{
			case zzGUIDockPos.min:
				lOut.x=0;
				break;
			case zzGUIDockPos.center:
				lOut.x=Screen.width/2-lOut.width/2;
				break;
			case zzGUIDockPos.max:
				lOut.x=Screen.width-lOut.width;
				break;
			//case zzGUIDockPos.custom:
			//	break;
		}
		
		//vertical
		switch(verticalDockPosition)
		{
			case zzGUIDockPos.min:
				lOut.y=0;
				break;
			case zzGUIDockPos.center:
				lOut.y=Screen.height/2-lOut.height/2;
				break;
			case zzGUIDockPos.max:
				lOut.y=Screen.height-lOut.height;
				break;
			//case zzGUIDockPos.custom:
			//	break;
		}
		
		return lOut;
	}
	
	function Reset() 
	{
		//Ìí¼Ó
		/*
		if(! gameObject.GetComponent(zzGUI) )
		{
			var lzzGUI:zzGUI = gameObject.AddComponent(zzGUI);
			lzzGUI.setGUI(this);
		}
		*/
		var lzzGUI:zzGUI = zzUtilities.needComponent(gameObject,zzGUI);
		lzzGUI.setGUI(this);
	}
	
	function OnDrawGizmosSelected () 
	{
		DrawGizmos(Color.white);
	}
	
	function OnDrawGizmos () 
	{
		//Gizmos.color = Color.blue;
		DrawGizmos(Color.blue);
	}
	
	function DrawGizmos(pColor:Color)
	{
		Gizmos.color = pColor;
		
		Gizmos.matrix = transform.localToWorldMatrix;
		
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.xMax,-position.y,0));
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.x,-position.yMax,0));
		
		
		Gizmos.DrawLine (Vector3(position.xMax,-position.y,0), Vector3(position.xMax,-position.yMax,0));
		Gizmos.DrawLine (Vector3(position.x,-position.yMax,0), Vector3(position.xMax,-position.yMax,0));
		/*
			
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.xMax-position.width/2,-position.y,0));
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.x,-position.yMax+position.height/2,0));
		
		
		Gizmos.DrawLine (Vector3(position.xMax-position.width/2,-position.y,0), Vector3(position.xMax-position.width/2,-position.yMax+position.height/2,0));
		Gizmos.DrawLine (Vector3(position.x,-position.yMax+position.height/2,0), Vector3(position.xMax-position.width/2,-position.yMax+position.height/2,0));
		*/
	}
}