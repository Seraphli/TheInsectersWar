var zzLanguageResource : Hashtable ={
	"Quit" : "退出"
};

function getResource(key : String)
{
	if (zzLanguageResource.ContainsKey(key))
	{
		return zzLanguageResource[key];
	}
}