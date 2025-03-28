using LightReflectiveMirror;
using Mirror;
using TMPro;
using UnityEngine;

public class TransportManager : MonoBehaviour
{
    public static TransportManager instance {  get; private set; }
    [SerializeField] TextMeshProUGUI joinDialogue;
    public static LightReflectiveMirrorTransport transport { get; private set; }
    private void Start()
    {
        instance = this;
        transport = GetComponent<LightReflectiveMirrorTransport>();
    }
    public void CreateGame()
    {
        NetworkManager.singleton.StartHost();
    }
    public void JoinGame()
    {
        if(joinDialogue.GetParsedText().Length != 5)
        {
            throw new System.Exception("Lobby id not correct! TODO: Implement ui for this");
        }
        transport.serverIP = joinDialogue.GetParsedText();
        NetworkManager.singleton.StartClient();
        //todo: check if ip is valid
    }
}
