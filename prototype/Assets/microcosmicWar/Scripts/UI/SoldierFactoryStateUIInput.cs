
using UnityEngine;
using System.Collections;

public class SoldierFactoryStateUIInput : MonoBehaviour
{


    public KeyCode moveLeftkey = KeyCode.N;
    public KeyCode moveRightkey = KeyCode.M;
    public KeyCode useItemKey = KeyCode.B;


    public SoldierFactoryStateUI bagItemUI;

    void Start()
    {
        if (!bagItemUI)
            bagItemUI = gameObject.GetComponent<SoldierFactoryStateUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(moveLeftkey))
        {
            //print("moveLeftkey");
            bagItemUI.selecteDown();
        }

        if (Input.GetKeyDown(moveRightkey))
        {
            //print("moveRightkey");
            bagItemUI.selecteUp();
        }

        if (Input.GetKeyDown(useItemKey))
        {
            //print("useItemKey");
            bagItemUI.useSelected();
        }
    }
}