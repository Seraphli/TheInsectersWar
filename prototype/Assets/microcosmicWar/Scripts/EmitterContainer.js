
class EmitterContainer  extends Emitter
{
	var emitList:Emitter[];

	function Start()
	{
	}

	virtual function EmitBullet()
	{
		for( var iEmit:Emitter in emitList )
		{
			iEmit.EmitBullet();
		}
	}

	virtual function setBulletLayer(pBulletLayer:int)
	{
		//print("EmitterContainer.setBulletLayer");
		for( var iEmit:Emitter in emitList )
		{
			iEmit.setBulletLayer(pBulletLayer);
		}
	}
}