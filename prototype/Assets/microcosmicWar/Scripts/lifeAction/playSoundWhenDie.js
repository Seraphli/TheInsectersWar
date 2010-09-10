
var soundToPlay:AudioSource[];


function Start()
{
	life = gameObject.GetComponentInChildren(Life);
	life.addDieCallback(deadAction);
}


//在死亡的回调中使用
function deadAction()
{
	if(soundToPlay.Length!=0)
		soundToPlay[Random.Range(0,soundToPlay.Length)].Play();
}