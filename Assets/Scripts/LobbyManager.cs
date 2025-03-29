using Mirror;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lobby;
    private void Start()
    {

        CrossSceneUIManager.instance.LoadingScreen(false);
    }
    private void Update()
    {
        lobby.text = TransportManager.transport.serverId;
    }
    public void StartRound()
    {
        // todo: check number of players
        lobby.text = InheritedNetworkRoomPlayer.instance.ReadyUp() ? "READY" : "NOT READY";
    }
}
