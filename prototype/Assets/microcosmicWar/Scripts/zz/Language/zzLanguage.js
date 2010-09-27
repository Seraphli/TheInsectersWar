
//单实例类指针赋初值
static protected var singletonInstance:zzLanguage=null;

static function getSingleton()
{
	return singletonInstance;
}

function Awake()
{
	if(singletonInstance)
		Debug.LogError("have singletonInstance");
	singletonInstance = this;
}
//设置成中文
static function setChinese(pGUI : zzInterfaceGUI) : String
{
	for(var i:System.Collections.DictionaryEntry in LangResForCn.Res)
		{
			if(pGUI.getSubElement(i.Key))
				pGUI.getSubElement(i.Key).setText(i.Value);
		}
}
//设置成英文
static function setEnglish(pGUI : zzInterfaceGUI) : String
{
	for(var i:System.Collections.DictionaryEntry in LangResForEn.Res)
		{
			if(pGUI.getSubElement(i.Key))
				pGUI.getSubElement(i.Key).setText(i.Value);
		}
}
//加入到哈希表中,0代表英文,1代表中文
/*static function addRes(key : String,value : String,Language : int)
{
	//var KEY : Object = key;var VALUE : Object = value;
	Debug.Log("Got");
	switch (Language)
	{
		case Language.En :if (!LangResForEn.zzLanguageRes.Contains(key)){
											Debug.Log("Got");
										LangResForEn.zzLanguageRes[key] = value;
					}
					break;
		case Language.Cn :if (!LangResForCn.zzLanguageRes.Contains(key)){
						Debug.Log("Got");
						LangResForCn.zzLanguageRes[key] = value;
					}
					break;
	}
}*/
//得到相应语言的关键字,0代表英文,1代表中文
static function SwitchLanguage (Res : String,Lang : int)
{
	switch (Lang)
	{
		case Language.En :if (LangResForEn.Res.Contains(Res))
											return LangResForEn.Res[Res];
					break;
		case Language.Cn :if (LangResForCn.Res.Contains(Res))
											return LangResForCn.Res[Res];
					break;
	}
}