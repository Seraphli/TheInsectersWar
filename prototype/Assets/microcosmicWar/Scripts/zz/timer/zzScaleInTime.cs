using UnityEngine;
using System.Collections;

public class zzScaleInTime : MonoBehaviour
{
    public Vector3 scaleVelocity;

    public Transform myTransform;

    void Start()
    {
        myTransform = transform;
    }
	
	// Update is called once per frame
	void Update () 
    {
        myTransform.localScale+=(scaleVelocity * Time.deltaTime);
	
	}
}
