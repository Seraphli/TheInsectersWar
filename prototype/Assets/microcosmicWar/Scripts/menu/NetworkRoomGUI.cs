using UnityEngine;

public class NetworkRoomGUI : MonoBehaviour
{
    public NetworkRoom networkRoom;

    public zzInterfaceGUI[] playerListLabel;

    public zzInterfaceGUI[] pismirePlayerLabel;
    public zzInterfaceGUI[] beePlayerLabel;

    public zzInterfaceGUI[] pismirePlayerSelected;
    public zzInterfaceGUI[] beePlayerSelected;

    public zzButton[] pismirePlayerButton;
    public zzButton[] beePlayerButton;

    public zzButton[] pismirePlayerRemoveButton;
    public zzButton[] beePlayerRemoveButton;

    void setVisible(zzInterfaceGUI[] pGUIs,bool pVisible)
    {
        foreach(var lUI in pGUIs)
        {
            lUI.visible = pVisible;
        }
    }

    void Awake()
    {
        networkRoom.addRoomDataChangedReceiver(OnNetworkRoomChanged);
        for (int i = 0; i < pismirePlayerButton.Length; ++i)
        {
            var lIndex = i;
            pismirePlayerButton[i].addClickEventReceiver(() => networkRoom.selectPismire(lIndex));
        }
        for (int i = 0; i < beePlayerButton.Length; ++i)
        {
            var lIndex = i;
            beePlayerButton[i].addClickEventReceiver(() => networkRoom.selectBee(lIndex));
        }
    }

    void OnNetworkRoomChanged()
    {
        setVisible(playerListLabel, false);

        setVisible(pismirePlayerSelected, false);
        setVisible(beePlayerSelected, false);

        setVisible(pismirePlayerButton, true);
        setVisible(beePlayerButton, true);

        for (int i = 0; i < networkRoom.playersInfo.Length;++i )
        {
            var lPlayerInfo = networkRoom.playersInfo[i];
            if (lPlayerInfo == null)
                continue;
            string lShowName = (i + 1) + "." + lPlayerInfo.playerName;
            playerListLabel[i].setText(lShowName);
            playerListLabel[i].visible = true;
            if (lPlayerInfo.race == Race.ePismire)
            {
                pismirePlayerButton[lPlayerInfo.spawnIndex].visible = false;
                pismirePlayerSelected[lPlayerInfo.spawnIndex].visible = true;
                pismirePlayerLabel[lPlayerInfo.spawnIndex].setText(lShowName);
            }
            else if (lPlayerInfo.race == Race.eBee)
            {
                beePlayerButton[lPlayerInfo.spawnIndex].visible = false;
                beePlayerSelected[lPlayerInfo.spawnIndex].visible = true;
                beePlayerLabel[lPlayerInfo.spawnIndex].setText(lShowName);
            }
        }
    }
}