
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

static function setChinese() : String
{
	var resValue : String;
	
	resValue = zzLanguageResource.getResource("Quit");
	
	return resValue;
}