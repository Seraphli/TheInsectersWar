
@script ExecuteInEditMode()

class zzWindow extends zzGUIContainer
{

	//var windowRect = Rect (20, 20, 120, 50);
	var title : GUIContent;
	var  style : GUIStyle;
	var ID=0;

	//var subElements:zzGUI[];

	function OnGUI () 
	{
		impGUI();
	}
	
	virtual function impGUI()
	{
		GUI.Window (ID, getPosition(), impSubs,title,style);
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