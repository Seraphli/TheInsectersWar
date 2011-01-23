
using UnityEngine;
using System.Collections;

public class bagItemUIInput : MonoBehaviour
{


    public KeyCode moveLeftkey = KeyCode.U;
    public KeyCode moveRightkey = KeyCode.I;
    public KeyCode useItemKey = KeyCode.H;


    public BagItemUI bagItemUI;

    void Start()
    {
        if (!bagItemUI)
            bagItemUI = gameObject.GetComponent<BagItemUI>();
    }

    void Update()
    {
        //if (Input.GetKeyDown(moveLeftkey))
        //{
        //    //print("moveLeftkey");
        //    bagItemUI.selecteDown();
        //}

        //if (Input.GetKeyDown(moveRightkey))
        //{
        //    //print("moveRightkey");
        //    bagItemUI.selecteUp();
        //}

        //if (Input.GetKeyDown(useItemKey))
        //{
        //    //print("useItemKey");
        //    bagItemUI.useSelected();
        //}
        if (Input.GetKeyDown(KeyCode.Alpha1))
            bagItemUI.useByIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            bagItemUI.useByIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            bagItemUI.useByIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            bagItemUI.useByIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            bagItemUI.useByIndex(4);
    }
}