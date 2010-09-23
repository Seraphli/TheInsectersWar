
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

function setChinese()
{
	var getValue : String;
	
	getValue = zzLanguageResource.getResource("Quit");
	
	Debug.Log(getValue);
}