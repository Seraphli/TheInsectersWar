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
	for(var i:System.Collections.DictionaryEntry in zzLanguageResource.zzLanguageRes)
		{
			//判断最后两个字符是否是Cn,是则去掉Cn作为物体的名字
			if (i.Key.Substring(i.Key.Length-2,2) == "Cn")
				pGUI.getSubElement(i.Key.Remove(i.Key.Length-2,2)).setText(i.Value);
		}
}
//设置成英文
static function setEnglish(pGUI : zzInterfaceGUI) : String
{
	for(var i:System.Collections.DictionaryEntry in zzLanguageResource.zzLanguageRes)
		{
			//判断最后两个字符是否是En,是则去掉En作为物体的名字
			if (i.Key.Substring(i.Key.Length-2,2) == "En")
				pGUI.getSubElement(i.Key.Remove(i.Key.Length-2,2)).setText(i.Value);
		}
}