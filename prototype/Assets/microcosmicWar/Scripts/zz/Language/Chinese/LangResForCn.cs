using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LangResForCn : MonoBehaviour {
	public static Dictionary<string, string> res = new Dictionary<string, string>();

    public static void AddRes ()
    {
        res.Add("Quit", "�˳�");
        res.Add("NetworkPlayer", "�����ս(������)");
        res.Add("sewer1", "��ˮ����ͼ(������)");
        res.Add("SinglePlayer", "����ģʽ");
        res.Add("Language", "En");
    }
}
