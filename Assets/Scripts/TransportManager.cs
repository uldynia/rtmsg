using System.Collections;
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
        transport.connectedToRelay.AddListener( () => { StartConnecting(2); });
        Application.targetFrameRate = 120;
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
        StartConnecting(1);
        NetworkManager.singleton.StartClient();
    }
    bool connecting;
    public void StartConnecting(int i)
    {
        if(i == 1)
        {
            connecting = true;
            Debug.Log("Starting to connect");
            //start showing loading bar
        }
        if(i == 2)
        {
            connecting = false;
            Debug.Log("Connected");
            // stop showing ui
        }
    }
    private void OnGUI()
    {
        GUILayout.Box($"{(float)Screen.width / Screen.height}");
    }
}
