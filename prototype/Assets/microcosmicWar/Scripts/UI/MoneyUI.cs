
using UnityEngine;
using System.Collections;

public class MoneyUI : MonoBehaviour
{


    public zzItemBagControl bagControl;
    public zzInterfaceGUI moneyLabel;


    void Start()
    {
        zzSceneObjectMap lSceneObjectMap = GameObject.Find("Main Camera")
            .transform.Find("UI").GetComponent<zzSceneObjectMap>();

        if (!moneyLabel)
            moneyLabel = lSceneObjectMap.getObject("moneyLabel").GetComponent<zzInterfaceGUI>();

        if (!bagControl)
            bagControl = gameObject.GetComponent<zzItemBagControl>();

        bagControl.addCallAfterStart(afterBagStartCall);
    }

    void afterBagStartCall()
    {
        refreshMoneyShow();
        bagControl.setMoneyChangedCall(refreshMoneyShow);
    }

    void refreshMoneyShow()
    {
        int lMoneyNum = bagControl.getMoneyNum();
        string lMoneyNumStr = lMoneyNum.ToString();

        //补成六位
        switch (lMoneyNumStr.Length)
        {
            case 1: lMoneyNumStr = "00000" + lMoneyNumStr; break;
            case 2: lMoneyNumStr = "0000" + lMoneyNumStr; break;
            case 3: lMoneyNumStr = "000" + lMoneyNumStr; break;
            case 4: lMoneyNumStr = "00" + lMoneyNumStr; break;
            case 5: lMoneyNumStr = "0" + lMoneyNumStr; break;
        }
        moneyLabel.setText(lMoneyNumStr);
    }
}