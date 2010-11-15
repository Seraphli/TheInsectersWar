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

    /// <summary>
    /// 在指定位置播放音源
    /// </summary>
    /// <param name="pAudioSource">复制此音源的设置</param>
    /// <param name="position">声源位置</param>
    /// <returns>新建的音源,将会在播放后销毁</returns>
    public static AudioSource playAudioSourceAtPoint(AudioSource pAudioSource, Vector3 position)
    {
        return playAudioSourceAtPoint(pAudioSource, pAudioSource.clip, position);
    }

    /// <summary>
    /// 用音源设置,在指定的位置 播放AudioClip
    /// </summary>
    /// <param name="pAudioSource"></param>
    /// <param name="pAudioClip"></param>
    /// <param name="position"></param>
    /// <returns>新建的音源,将会在播放后销毁</returns>
    public static AudioSource playAudioSourceAtPoint(AudioSource pAudioSource, AudioClip pAudioClip, Vector3 position)
    {
        GameObject lAudioSourceObject = new GameObject("One shot audio");
        lAudioSourceObject.transform.position = position;
        AudioSource source = lAudioSourceObject.AddComponent<AudioSource>();
        source.clip = pAudioClip;
        source.volume = pAudioSource.volume;
        source.pitch = pAudioSource.pitch;
        source.velocityUpdateMode = pAudioSource.velocityUpdateMode;
        source.panLevel = pAudioSource.panLevel;
        source.dopplerLevel = pAudioSource.dopplerLevel;
        source.spread = pAudioSource.spread;
        source.priority = pAudioSource.priority;
        source.minDistance = pAudioSource.minDistance;
        source.maxDistance = pAudioSource.maxDistance;
        source.pan = pAudioSource.pan;
        source.rolloffMode = pAudioSource.rolloffMode;
        source.Play();
        GameObject.Destroy(lAudioSourceObject, pAudioClip.length);
        return source;
    }


    /// <summary>
    /// 画一个从pFrom到pTo的箭头
    /// </summary>
    /// <param name="pFrom"></param>
    /// <param name="pTo"></param>
    public static void GizmosArrow(Vector3 pFrom, Vector3 pTo)
    {
        //箭头心在原点,朝Vector3.right 的情况
        //Vector3 point1 = new Vector3(-1.0f,1.0f,0.0f);
        //Vector3 point2 = new Vector3(-1.0f,-1.0f,0.0f);

        //Quaternion rot = new Quaternion();
        //rot.SetFromToRotation(Vector3.right, pTo - pFrom);

        Gizmos.DrawLine(pFrom, pTo);
        Gizmos.DrawSphere(pTo, 0.3f);
        //Gizmos.DrawLine(rot * point1, pTo);
        //Gizmos.DrawLine(rot * point2, pTo);
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
