
var bagControl:zzItemBagControl;
var UIroot:zzInterfaceGUI;

protected var numOfShowItem = 5;
var itemListUI:zzInterfaceGUI[];
var selectedListUI:zzInterfaceGUI[];
var selectedIndex = 0;
var itemNum=0;

//��ʾ�Ĺ��ߵ�����
var itemIndexList = Array();

function Start()
{
	if(!bagControl)
		bagControl = gameObject.GetComponent(zzItemBagControl);
	
	if(!UIroot)
		UIroot = gameObject.Find("Main Camera")
			.transform.Find("UI/ItemInventory").GetComponent(zzInterfaceGUI);
		
	itemListUI = new zzInterfaceGUI[numOfShowItem];
	selectedListUI = new zzInterfaceGUI[numOfShowItem];
	
	var itemList:zzInterfaceGUI = UIroot.getSubElement("itemList");
	var selectedList:zzInterfaceGUI = UIroot.getSubElement("selectedList");
	
	for(var i = 1 ; i<=numOfShowItem;++i)
	{
		itemListUI[i-1] = itemList.getSubElement(i.ToString());
		selectedListUI[i-1] = selectedList.getSubElement(i.ToString());
	}
	setSelected(1);

	//print(gameObject.name+bagControl+(bagControl==null));
	bagControl.addCallAfterStart(afterBagStartCall);
}

function afterBagStartCall()
{	
	refreshItemShow();
	bagControl.setItemChangedCall(refreshItemShow);
	//Debug.Log(selectedIndex);
	//Debug.Log(gameObject.name);
}

function Reset() 
{
	bagControl = gameObject.GetComponent(zzItemBagControl);
}
	
function refreshItemShow()
{
	var lItemList:Array = bagControl.getItemList();
	//Debug.Log(lItemList);
	itemIndexList = lItemList;
	var lItemUIIndex = 0;
	var lItemTypeTable:zzIndexTable = bagControl
									.getItemSystem().getItemTypeTable();
	for(var i:int in lItemList)
	{
		var lItemType:ItemTypeInfo= lItemTypeTable.getData(i);
		itemListUI[lItemUIIndex].setImage(lItemType.getImage());
		++lItemUIIndex;
	}
	
	itemNum=lItemUIIndex;
	
	//Debug.Log(lItemUIIndex);
	//����ѡ���λ��
	if(lItemUIIndex<selectedIndex)
		setSelected(lItemUIIndex);
	
	//��ʣ���ͼ��ռ����
	for(;lItemUIIndex<numOfShowItem;++lItemUIIndex)
		itemListUI[lItemUIIndex].setVisible(false);
}

//��1��ʼ, 0Ϊû��ѡ, �������Ϊ��
function setSelected(pIndex:int)
{
	//if(pIndex<0)
	//	pIndex = 0;
	//if(pIndex>itemNum)
	//	pIndex = itemNum;
		
	//if(selectedIndex == pIndex)
	//	return;
	for(var i:zzInterfaceGUI in selectedListUI)
		i.setVisible(false);
	if(pIndex>0)
		selectedListUI[pIndex-1].setVisible(true);
		
	selectedIndex = pIndex;
	//Debug.Log(selectedIndex);
}

function haveItem()
{
	//Debug.Log(selectedIndex);
	//Debug.Log(gameObject.name);
	return selectedIndex>0;
}

//����
function selecteDown()
{
	if(haveItem())
	{
		var lNowIndex = selectedIndex-1;
		if(lNowIndex<1)
			lNowIndex = itemNum;
		setSelected(lNowIndex);
	}
}

//����
function selecteUp()
{
	if(haveItem())
	{
		var lNowIndex = selectedIndex+1;
		if(lNowIndex>itemNum)
			lNowIndex = 1;
		setSelected(lNowIndex);
	}
}

function useSelected()
{
	if(selectedIndex>0)
	{
		var lIndex:int = itemIndexList[selectedIndex-1];
		bagControl.useItemOne(lIndex);
	}
}