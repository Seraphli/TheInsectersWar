using UnityEngine;

public class ShopFlowerControl:MonoBehaviour
{
    public ShopBuilding shopBuilding;

    void Update()
    {
        if (Input.GetButtonDown("left"))
            shopBuilding.backwardSelect();
        if (Input.GetButtonDown("right"))
            shopBuilding.forwardSelect();

        if (Input.GetButtonDown("up"))
            shopBuilding.buySeleted();
        if (Input.GetButtonDown("down"))
            shopBuilding.sellSeleted();


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
        shopBuilding.sell(pIndex);
    }
}