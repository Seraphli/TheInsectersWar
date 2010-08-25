
var bagControl:zzItemBagControl;
var moneyLabel:zzInterfaceGUI;


function Start()
{
	if(!moneyLabel)
		moneyLabel = gameObject.Find("Main Camera")
			.transform.Find("UI/portrait/moneyLabel").GetComponent(zzInterfaceGUI);

	bagControl.addCallAfterStart(afterBagStartCall);
}

function afterBagStartCall()
{	
	refreshMoneyShow();
	bagControl.setMoneyChangedCall(refreshMoneyShow);
}

function refreshMoneyShow()
{
	var lMoneyNum:int = bagControl.getMoneyNum();
	var lMoneyNumStr:String = lMoneyNum.ToString();
	
	//≤π≥…¡˘Œª
	switch(lMoneyNumStr.Length)
	{
		case 1: lMoneyNumStr="00000"+lMoneyNumStr; break;
		case 2: lMoneyNumStr="0000"+lMoneyNumStr; break;
		case 3: lMoneyNumStr="000"+lMoneyNumStr; break;
		case 4: lMoneyNumStr="00"+lMoneyNumStr; break;
		case 5: lMoneyNumStr="0"+lMoneyNumStr; break;
	}
	moneyLabel.setText(lMoneyNumStr);
}