

class zzWindow extends zzGUIContainer
{

	//var windowRect = Rect (20, 20, 120, 50);
	var title : GUIContent;
	var useDefaultStyle=false;
	var  style : GUIStyle;
	var ID=0;
	
	var enableDrag=false;

	//var subElements:zzGUI[];

	
	virtual function impGUI():void
	{
		if(useDefaultStyle)
		{
			//print("useDefaultStyle");
			position = GUI.Window (ID, getPosition(), DoMyWindow,title);
			return;
		}
			//print("not useDefaultStyle");
		position = GUI.Window (ID, getPosition(), DoMyWindow,title,style);
	}
	
	virtual function DoMyWindow (windowID : int)
	{
		impSubs();
			//print("enableDrag:"+enableDrag);
		if(enableDrag)
			GUI.DragWindow ();
	}
	

	// Make the contents of the window
	//function DoMyWindow (windowID : int) 
//	{
	//if (GUI.Button (Rect (10,20,100,20), "Hello World"))
	//print ("Got a click");
		//for(var i:zzGUI in subElements)
		//	i.impGUI();
			//print("DoMyWindow begin");
			
			/*
		for(var i:Transform in transform)
		{
			//print(i.name);
			var impGUI:zzGUI = i.GetComponent(zzGUI);
			if(impGUI)
				impGUI.impGUI();
		}
		*/
			//print("DoMyWindow end");
//	}

}