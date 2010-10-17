using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public sealed class zzLanguage : MonoBehaviour {

	private string _language;
	public zzInterfaceGUI pGUI;
	public string Language
	{
		get
		{
			return _language;
		}
		set
		{
			_language = value;
            switch (_language)
            {
                case "English":
                    SetLanguage(LangResForEn.res);
                    break;
                case "Chinese":
                    SetLanguage(LangResForCn.res);                    
                    break;
            }
		}
	}
    private static zzLanguage singletonInstance = null;
    public static zzLanguage GetInstance()
    {
        return singletonInstance;
    }
    private void SetLanguage(Dictionary<string,string> sender)
    {
        foreach (KeyValuePair<string, string> i in sender)
        {
            if (pGUI.getSubElement(i.Key))
                pGUI.getSubElement(i.Key).setText(i.Value);
        }
    }

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("Havae SingletonInstance");
        singletonInstance = this;
        //Debug.Log("Got");
        LangResForEn.AddRes();
        LangResForCn.AddRes();

    }
}


