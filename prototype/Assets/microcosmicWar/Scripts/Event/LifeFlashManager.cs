using UnityEngine;
using System.Collections;

public class LifeFlashManager : MonoBehaviour
{

    #region SingletonInstance
    static LifeFlashManager singletonInstance;

    public static LifeFlashManager Singleton
    {
        get
        {
            return singletonInstance;
        }
    }

    void OnDestroy()
    {
        singletonInstance = null;
    }

    void Awake()
    {
        if (singletonInstance != null)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
    }
    #endregion

    public Color selfAttackedColor;
    public Color attackedBySelfColor;
    public Color attackedByTeammateColor;

    public GameObject awardNumLabelPrafab;

    public void showAwardNumLabel(Vector3 pPosition, int pNum, NetworkPlayer pPlayer)
    {
        networkView.RPC("RPCShowAwardNumLabel", pPlayer, pPosition, pNum);
    }

    public void showAwardNumLabel(Vector3 pPosition, int pNum)
    {
        var lShowNumObject = (GameObject)Instantiate(awardNumLabelPrafab,
            pPosition, Quaternion.identity);
        var lNumber = lShowNumObject.GetComponentInChildren<TextMesh>();
        lNumber.text = pNum.ToString();
    }

    [RPC]
    public void RPCShowAwardNumLabel(Vector3 pPosition, int pNum)
    {
        showAwardNumLabel( pPosition,  pNum);
    }
}
