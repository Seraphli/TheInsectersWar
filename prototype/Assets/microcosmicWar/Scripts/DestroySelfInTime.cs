using UnityEngine;
using System.Collections;

public class DestroySelfInTime : MonoBehaviour
{
    public float time = 0.1f;
	// Use this for initialization
    void Start()
    {
        Destroy(gameObject, time);

    }
	
	// Update is called once per frame
    //void Update () {
	
    //}
}
