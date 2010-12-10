using UnityEngine;
using System.Collections;

public class TriggerTest : MonoBehaviour
{

	// Use this for initialization
    //void Start () {
	
    //}
	
    //// Update is called once per frame
    //void Update () {
	
    //}

    void OnTriggerEnter()
    {
        print("OnTriggerEnter");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
            rigidbody.WakeUp();
    }
}
