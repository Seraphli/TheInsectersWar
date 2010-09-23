static var zzLanguageRes : Hashtable ={
	"Quit" : "退出"
};

static function getResource(key : String) :String
{
	if (zzLanguageRes.ContainsKey(key))
	{
		return zzLanguageRes[key];
	}
	return "error";
}