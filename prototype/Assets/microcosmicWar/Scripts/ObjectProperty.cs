
using UnityEngine;
using System.Collections;

public enum Identitys
{
    Structure,
    Soldier,
    Barrel,
};

public class ObjectProperty : MonoBehaviour
{
    public Identitys identity = Identitys.Structure;

    void setPosition()
    {

    }
}