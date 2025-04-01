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
        UpdatePlayerStatus();
        return isReady;
    }

    [ClientRpc]
    public void RPCUpdatePlayerStatus(bool host, bool other)
    {
        if (isServer)
        {
            Debug.Log("This is host");
            LobbyManager.instance.SetPlayerReadyStatus(host, other);
        }
        else
        {
            Debug.Log("This is not host");
            LobbyManager.instance.SetPlayerReadyStatus(other, host);

        }
    }

    [Command]
    void UpdatePlayerStatus()
    {
        var hostplayer_readystate = true;
        var otherplayer_readystate = true;

        //Get the status of 
        foreach (var v in NetworkServer.connections.Values)
        {
            if (v.identity.isLocalPlayer)
            {
                hostplayer_readystate = v.identity.GetComponent<InheritedNetworkRoomPlayer>().readyToBegin;
            }
            else
            {
                otherplayer_readystate = v.identity.GetComponent<InheritedNetworkRoomPlayer>().readyToBegin;
            }
        }

        RPCUpdatePlayerStatus(hostplayer_readystate, otherplayer_readystate);
    }

}
