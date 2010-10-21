using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LangResForCn : MonoBehaviour {
	public static Dictionary<string, string> res = new Dictionary<string, string>();

    public static void AddRes ()
    {
        res.Add("Quit", "退出");
        res.Add("NetworkPlayer", "网络对战(不可用)");
        res.Add("sewer1", "下水道地图(不可用)");
        res.Add("SinglePlayer", "单人模式");
        res.Add("Language", "En");
    }
}
