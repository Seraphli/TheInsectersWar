
var soundToPlay:AudioSource[];


function Start()
{
	life = gameObject.GetComponentInChildren(Life);
	life.addDieCallback(deadAction);
}


//�������Ļص���ʹ��
function deadAction()
{
	if(soundToPlay.Length!=0)
		soundToPlay[Random.Range(0,soundToPlay.Length)].Play();
}