
using UnityEngine;
using System.Collections;

public enum Identitys
{
    Tower,
    Soldier,
    Barrel,
};

public class ObjectProperty : MonoBehaviour
{
    public Identitys identity = Identitys.Tower;

    void setPosition()
    {

    }
}