
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class zzRandomObjectByWeight<T>
{
    List<T> objectList = new List<T>();
    public void addRandomObject(T pObject,int weight)
    {
        int lPreNum = objectList.Count;
        objectList.Capacity = objectList.Count + weight;
        for (int numToAdd = weight; numToAdd > 0; --numToAdd)
            objectList.Add(pObject);
    }

    public int totalWeigth
    {
        get { return objectList.Count; }
    }

    public T randomObject()
    {
        return objectList[Random.Range(0, objectList.Count)];
    }

    public T[]  getList()
    {
        return objectList.ToArray();
    }
}