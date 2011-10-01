
using UnityEngine;
using System.Collections;

public class bagItemUIInput : MonoBehaviour
{


    public KeyCode moveLeftkey = KeyCode.U;
    public KeyCode moveRightkey = KeyCode.I;
    public KeyCode useItemKey = KeyCode.H;


    public WMItemBag itemBag;

    //void Start()
    //{
    //    if (!bagItemUI)
    //        bagItemUI = gameObject.GetComponent<BagItemUI>();
    //}

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
            use(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            use(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            use(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            use(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            use(4);
    }

    void use(int pIndex)
    {
        itemBag.items[pIndex].use();

    }
}