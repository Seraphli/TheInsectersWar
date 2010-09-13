
var animationToPlay:Animation;
var animationName:String;
var animationStateLayer = 10;
var life:Life;


function Start()
{
	life = gameObject.GetComponentInChildren(Life);
	life.addDieCallback(deadAction);
	animationToPlay[animationName].layer = animationStateLayer;
}

//�������Ļص���ʹ��
function deadAction()
{
	animationToPlay.Play(animationName);
}