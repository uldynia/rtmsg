using Mirror;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lobby;
    private void Update()
    {
        lobby.text = TransportManager.transport.serverId;
    }
    public void StartRound()
    {
        // todo: check number of players
        NetworkManager.singleton.ServerChangeScene("Game");
    }
}
