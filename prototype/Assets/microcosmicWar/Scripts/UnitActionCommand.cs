using UnityEngine;
using System.Collections;

//public class NewBehaviourScript : MonoBehaviour {

//    // Use this for initialization
//    void Start () {
	
//    }
	
//    // Update is called once per frame
//    void Update () {
	
//    }
//}

public class UnitActionCommand
{
    enum UnitActionCommandValue
    {
        FaceLeft=1,
        FaceRight=1<<1,
        GoForward=1<<2,
        Fire=1<<3,
        Jump=1<<4,
    }

    int mValue=0;

    public bool FaceLeft
    {
        get
        {
            return ( mValue & (int)UnitActionCommandValue.FaceLeft ) !=0 ;
        }

        set
        {
            if (value)
            {
                mValue |= (int)UnitActionCommandValue.FaceLeft;
            }
            else
            {
                mValue &= ~(int)UnitActionCommandValue.FaceLeft;
            }
        }
    }

    bool FaceRight ;
    bool GoForward;
    bool Fire;
    bool Jump;
};
