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
    }
    public bool ReadyUp()
    {
        isReady = !isReady;
        CmdChangeReadyState(isReady);
        return isReady;
    }
}
