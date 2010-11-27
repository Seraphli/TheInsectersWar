using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
	public float time;
	
	void Update()
	{
		time -= Time.deltaTime;
		
		if(time < 0.0)
		{
			Destroy(gameObject);
		}
	}
}

