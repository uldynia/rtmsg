using LightReflectiveMirror;
using Mirror;
using TMPro;
using UnityEngine;

public class TransportManager : MonoBehaviour
{
    public static TransportManager instance {  get; private set; }
    [SerializeField] TMP_InputField joinDialogue;
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
        if (joinDialogue.text.Length != 5)
        {
            throw new System.Exception($"Lobby id not correct! Length: {joinDialogue.text.Length} TODO: Implement ui for this");
        }
        transport.serverIP = joinDialogue.text;
        NetworkManager.singleton.networkAddress = joinDialogue.text;
        NetworkManager.singleton.StartClient();
        //todo: check if ip is valid
    }
}
