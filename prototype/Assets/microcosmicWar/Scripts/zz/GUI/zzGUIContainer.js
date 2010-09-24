
class zzGUIContainer extends zzInterfaceGUI
{
	virtual function impGUI():void
	{
		impSubs();
	}
	
	
	function impSubs():void
	{
		var lGUIlist = ArrayList();
		for(var lTransform:Transform in transform)
		{
			var impGUI:zzGUI = lTransform.GetComponent(zzGUI);
			if(impGUI)
			{
				//���������,С������ǰ��,�ȱ���Ⱦ,�ᱻ��ȴ����ס
				for(var i=0;i<lGUIlist.Count;++i)
				{
					if(impGUI.getDepth()<lGUIlist[i].getDepth())
					{
						lGUIlist.Insert(i,impGUI);
						impGUI=null;//��˵�������
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
