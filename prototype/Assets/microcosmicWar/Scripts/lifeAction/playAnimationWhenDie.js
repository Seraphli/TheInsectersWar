
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

//在死亡的回调中使用
function deadAction()
{
	animationToPlay.Play(animationName);
}