
using UnityEngine;
using System.Collections;


public class zzIndexTable
{
    protected ArrayList dataList = new ArrayList();

    protected Hashtable nameToIndex = new Hashtable();

    //return index
    public int addData(string name, object data)
    {
        dataList.Add(data);
        int lIndex = dataList.Count - 1;
        nameToIndex[name] = lIndex;
        return lIndex;
    }

    public int getIndex(string name)
    {
        return (int)nameToIndex[name];
    }

    public object getData(int pIndex)
    {
        return dataList[pIndex];
    }

    public int getNum()
    {
        return dataList.Count;
    }

    override public string ToString()
    {
        return dataList.ToString();
    }
}