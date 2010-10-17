using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LangResForEn : MonoBehaviour
{
    public static Dictionary<string, string> res = new Dictionary<string, string>();

    public static void AddRes()
    {
        res.Add("Quit", "Quit");
        res.Add("NetworkPlayer", "Multiplayer :sewer (new)");
        res.Add("sewer1", "sewer1");
        res.Add("SinglePlayer", "Single Player:sewer (new)");
        res.Add("Language", "ÖÐÎÄ");
    }
}