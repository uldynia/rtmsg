using Mirror;
using UnityEngine;

public class InheritedNetworkRoomPlayer : NetworkRoomPlayer
{
    public static InheritedNetworkRoomPlayer instance { get; private set; }
    bool isReady = false;
    public override void Start()
    {
        if (isLocalPlayer) {
            instance = this;
        }
        else
        {
            LobbyManager.instance.SetOpponentProfile(true);
        }
    }
    private void OnDestroy()
    {
        if(!isLocalPlayer) 
        LobbyManager.instance.SetOpponentProfile(false);
    }
    public bool ReadyUp()
    {
        isReady = !isReady;
        CmdChangeReadyState(isReady);
        return isReady;
    }
}
