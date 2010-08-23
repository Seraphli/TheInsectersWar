
class zzGUIContainer extends zzInterfaceGUI
{
	virtual function impGUI()
	{
		impSubs();
	}
	
	function impSubs()
	{
		var lGUIlist = ArrayList();
		for(var lTransform:Transform in transform)
		{
			var impGUI:zzGUI = lTransform.GetComponent(zzGUI);
			if(impGUI)
			{
				//按深度排序
				for(var i=0;i<lGUIlist.Count;++i)
				{
					if(impGUI.getDepth()<lGUIlist[i].getDepth())
					{
						lGUIlist.Insert(i,impGUI);
						impGUI=null;//来说明已添加
						break;
					}
				}
				if(impGUI)
					lGUIlist.Add(impGUI);
			}
		}
		
		//print("********************");
		for(var imp:zzGUI in lGUIlist)
		{
			//print(imp.gameObject.name+" "+imp.getDepth());
			imp.impGUI();
		}
	}
}
