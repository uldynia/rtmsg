using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager instance;
    [SerializeField] TextMeshProUGUI serverCode;
    [SerializeField] TextMeshProUGUI readyButtonText;
    [SerializeField] GameObject opponentDisplay;

    [SerializeField] TextMeshProUGUI localPlayerReadyStatus;
    [SerializeField] TextMeshProUGUI otherPlayerReadyStatus;

    [SerializeField] AudioClip readyup_audioclip;

    NetworkRoomManager roomManager;
    GameObject oldInstance;
    private void Awake()
    {
        instance = this;
        CrossSceneUIManager.instance.LoadingScreen(false);
        roomManager = TransportManager.instance.GetComponent<NetworkRoomManager>();
        roomManager.OnReady = () =>
        {
            StartGameVisual();
            Invoke("ChangeScene", 2);
        }; 
    }
    private void Start()
    {
        if (TransportManager.instance.tutorialMode) ChangeScene();
    }
    public void ChangeScene()
    {
        roomManager.ServerChangeScene(roomManager.GameplayScene);
    }
    [ClientRpc]
    public void StartGameVisual()
    {
        CrossSceneUIManager.instance.LoadingScreenDuration();
    }
    public void SetOpponentProfile(bool show)
    {
        Debug.LogWarning("TODO: Implement showing of opponent's profile picture and name.");
        opponentDisplay?.SetActive(show);
    }
    bool backToLobby;
    private void Update()
    {
        serverCode.text = TransportManager.transport.serverId;
        if (!TransportManager.transport.Available() && !backToLobby)
        {
            CrossSceneUIManager.instance.OpenPopup("Failed to connect to server! Sending you back to the lobby.");
            backToLobby = true;
            StartCoroutine(ReturnToLobby());
        }
    }
    IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(2);
        Destroy(TransportManager.instance.gameObject);
        SceneManager.LoadScene("Title");
    }
    public void StartRound()
    {
        // todo: check number of players
        readyButtonText.text = InheritedNetworkRoomPlayer.instance.ReadyUp() ? "CANCEL" : "FIGHT!";
        AudioSfxManager.m_instance.OnPlayNewAudioClip(readyup_audioclip);
    }

    public void SetPlayerReadyStatus(bool current_player, bool other_player)
    {
        localPlayerReadyStatus.text = current_player ? "READY" : "NOT READY";
        otherPlayerReadyStatus.text = other_player ? "READY" : "NOT READY";

        localPlayerReadyStatus.color = current_player ? Color.green: Color.red;
        otherPlayerReadyStatus.color = other_player ? Color.green: Color.red;
    }
}
