

///var fireSound:AudioSource;


class EmitterContainer  extends Emitter
{
	var emitList:Emitter[];

	virtual function setInjureInfo(pInjureInfo:Hashtable)
	{
		super.setInjureInfo( pInjureInfo );
		for( var iEmit:Emitter in emitList )
		{
			iEmit.setInjureInfo(pInjureInfo);
		}
	}

	function Start()
	{
	}

	virtual function EmitBullet()
	{
		for( var iEmit:Emitter in emitList )
		{
			iEmit.EmitBullet();
		}
		if(fireSound)
		{
			fireSound.Play();
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