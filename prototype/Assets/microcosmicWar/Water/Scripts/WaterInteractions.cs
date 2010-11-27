using UnityEngine;
using System.Collections;

public class WaterInteractions : MonoBehaviour {
	
	public GameObject impactParticle;	
	public AudioClip waterImpactSound;
	public float destroyTimer = 1;
	
	private GameObject go;

	void OnTriggerEnter(Collider other)
    {
		GameObject impactClone = GameObject.Instantiate(impactParticle, other.transform.position, Quaternion.identity) as GameObject;
		EmitJumpParticles(impactClone);
		SetAutoDestroy(other.gameObject,destroyTimer);
	}

	public void EmitJumpParticles(GameObject go)
	{
		go.audio.PlayOneShot(waterImpactSound, 1f);

		ParticleEmitter emitter;
		for(int i = 0; i < go.transform.childCount; i++)
		{
			emitter = go.transform.GetChild(i).GetComponent("ParticleEmitter") as ParticleEmitter;
							
			if(emitter == null) continue;
							
			emitter.emit = false;
			emitter.Emit();
		}
		SetAutoDestroy(go,2);
	}
	
	public void SetAutoDestroy(GameObject go,float time)
	{
		AutoDestroy aux = go.AddComponent("AutoDestroy") as AutoDestroy;
		aux.time = time;
	}
}
