
//@RequireComponent(zzGUI)

enum zzGUIDockPos
{
	min,
	center,
	max,
	custom
};

class zzGUIRelativeLength
{
	var useRelative=false;
	var relativeLength:float;//[0,1]
}

class zzInterfaceGUI extends MonoBehaviour
{
	//var data:int;
	var position : Rect;
	var depth:int;
	var skin : GUISkin;
	
	//位置信息;custom 则使用position的
	var horizontalDockPosition:zzGUIDockPos=zzGUIDockPos.custom;
	var verticalDockPosition:zzGUIDockPos=zzGUIDockPos.custom;
	
	//相对尺寸信息,useRelative=false,则使用position的
	var relativeWidth=zzGUIRelativeLength();
	var relativeHeight=zzGUIRelativeLength();
	
	var visible:boolean = true;
	
	//只是为了付类型
	protected function nullGUICallback(pGUI:zzInterfaceGUI):void
	{
	}
	
	virtual function getSkin():GUISkin
	{
		return skin;
	}
	
	virtual function setVisible(pVisible:boolean):void
	{
		visible = pVisible;
	}
	
	virtual function getVisible():boolean
	{
		return visible;
	}
	
	virtual function impGUI():void
	{
	
	}
	
	virtual function setImage(pImage:Texture):void
	{
	}
	
	virtual function setText(pText:String):void
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
	
	function getParent():zzInterfaceGUI
	{
		var impGUI:zzGUI = transform.parent.GetComponent(zzGUI);
		if(impGUI)
			return impGUI.getGUI();
		return null;
	}
	
	virtual function getWidth():float
	{
		if(relativeWidth.useRelative)
			return relativeWidth.relativeLength*getParent().getWidth();
		return position.width;
	}
	
	virtual function getHeight():float
	{
		if(relativeHeight.useRelative)
			return relativeWidth.relativeLength*getParent().getHeight();
		return position.height;
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
		
		lOut.width = getWidth();
		//print(gameObject.name);
		//print(relativeWidth.useRelative);
		//print(lOut.width);
		lOut.height= getHeight();
		
		return lOut;
	}
	
	function Reset() 
	{
		//添加
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
		
		var lPosition = getPosition();
		Gizmos.DrawLine (Vector3(lPosition.x,-lPosition.y,0), Vector3(lPosition.xMax,-lPosition.y,0));
		Gizmos.DrawLine (Vector3(lPosition.x,-lPosition.y,0), Vector3(lPosition.x,-lPosition.yMax,0));
		
		
		Gizmos.DrawLine (Vector3(lPosition.xMax,-lPosition.y,0), Vector3(lPosition.xMax,-lPosition.yMax,0));
		Gizmos.DrawLine (Vector3(lPosition.x,-lPosition.yMax,0), Vector3(lPosition.xMax,-lPosition.yMax,0));
		/*
			
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.xMax-position.width/2,-position.y,0));
		Gizmos.DrawLine (Vector3(position.x,-position.y,0), Vector3(position.x,-position.yMax+position.height/2,0));
		
		
		Gizmos.DrawLine (Vector3(position.xMax-position.width/2,-position.y,0), Vector3(position.xMax-position.width/2,-position.yMax+position.height/2,0));
		Gizmos.DrawLine (Vector3(position.x,-position.yMax+position.height/2,0), Vector3(position.xMax-position.width/2,-position.yMax+position.height/2,0));
		*/
	}
}