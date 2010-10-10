using UnityEngine;
using System.Collections;

public class zzUtilities
{
    public delegate void voidFunction();

    static public void nullFunction()
    {
    }


    public static Component needComponent(GameObject gameObject, System.Type componentType)
    {
        Component lOut = gameObject.GetComponent(componentType);
        if (!lOut)
        {
            lOut = gameObject.AddComponent(componentType);
        }
        return lOut;
    }
    /*
    static Component reAddComponent ( GameObject gameObject ,  System.Type componentType  ){
        if(gameObject.GetComponent<componentType>())
            gameObject.Destroy(gameObject.GetComponent<componentType>());
		
        return gameObject.AddComponent<componentType>();
    }
    */
    //移除Array中索引位置的元素,用末尾取代,节省移动数组的开支
    /*
    public static void  quickRemoveArrayElement ( Array array ,  int index  ){
        FIXME_VAR_TYPE t=array.Pop();
        //当索引不为最后一位时,现在的array.length为原最后索引值,因为pop使其减一
        if(index!=array.length)
            array[index]=t;
    }
    */
    public static void quickRemoveArrayElement(ArrayList array, int index)
    {
        int lMaxIndex = array.Count - 1;
        object t = array[lMaxIndex];
        //array.RemoveAt(index);
        //当索引不为最后一位时,现在的array.length为原最后索引值
        if (index != lMaxIndex)
            array[index] = t;
        array.RemoveAt(lMaxIndex);
    }

    //删除第一个在Array出现的pValue
    /*
    public static void  removeValueInArray ( Array array ,  pValue){
            for( FIXME_VAR_TYPE i=0;i<array.length;++i)
            {
                if(array[i]== pValue)
                {
                    array.RemoveAt(i);
                    break;
                }
            }
    }
    */

    //删除第一个在Array出现的pValue
    public static void removeValueInArray(ArrayList array, object pValue)
    {
        for (int i = 0; i < array.Count; ++i)
        {
            if (Object.Equals(array[i] , pValue) )
            {
                array.RemoveAt(i);
                break;
            }
        }
    }

    public static float normalize(float pValue)
    {
        if (pValue > 0)
            return 1;
        else if (pValue < 0)
            return -1;
        return 0;
    }


}
public class DataWrap
{
    public object data;

    override public string ToString()
    {
        return data.ToString();
    }
}

public class zzPair
{
    override public string ToString()
    {
        return "left:" + left + " right:" + right;
    }

    public zzPair() { }
    public zzPair(object Lift, object Right)
    {
        left = Lift;
        right = Right;
    }

    public object left;
    public object right;
}
