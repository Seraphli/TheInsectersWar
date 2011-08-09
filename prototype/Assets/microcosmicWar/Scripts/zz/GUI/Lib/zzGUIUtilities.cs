using UnityEngine;

public class zzGUIUtilities
{
    public static string toString(float pValue)
    {
        string lText = pValue.ToString();
        if (lText.IndexOf('.') < 0)
            lText += ".0";
        return lText;
    }

    public static bool stringToFloat(string pValue, out float pOut)
    {
        if (pValue.Length == 0)
        {
            pOut = 0f;
            return true;
        }
        else
        {
            var lNums = pValue.Split('.');
            if (lNums.Length > 2)
            {
                pValue = lNums[0] + "." + lNums[1];
                for (int i = 2; i < lNums.Length; ++i)
                {
                    pValue += lNums[i];
                }
            }
            return float.TryParse(pValue, out pOut);
        }
    }
}