
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;


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

public class zzGenericIndexTable<K,V>
{
    //protected ArrayList dataList = new ArrayList();
    protected List<V> dataList = new List<V>();

    protected List<K> keyList = new List<K>();

    protected Dictionary<K, int> keyToIndex = new Dictionary<K, int>();

    //return index
    public int addData(K pKey, V pValue)
    {
        keyList.Add(pKey);
        dataList.Add(pValue);
        int lIndex = dataList.Count - 1;
        keyToIndex[pKey] = lIndex;
        return lIndex;
    }

    public int getIndex(K pKey)
    {
        return keyToIndex[pKey];
    }

    public V getData(int pIndex)
    {
        return dataList[pIndex];
    }

    public V getDataByKey(K pKey)
    {
        return getData(getIndex(pKey));
    }

    public int getNum()
    {
        return dataList.Count;
    }

    //得到按索引顺序的 键 列表
    public ReadOnlyCollection<K> getKeyList()
    {
        return keyList.AsReadOnly();
    }

    public int Count
    {
        get
        {
            return dataList.Count;
        }
    }

    override public string ToString()
    {
        return dataList.ToString();
    }

}