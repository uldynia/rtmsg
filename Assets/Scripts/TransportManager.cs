using System.Collections;
using LightReflectiveMirror;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransportManager : MonoBehaviour
{
    public static TransportManager instance {  get; private set; }
    [SerializeField] TMP_InputField joinDialogue;
    [SerializeField] Image titleScreen;
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
        StartCoroutine(Create());
        IEnumerator Create()
        {
            CrossSceneUIManager.instance.LoadingScreen(true);
            yield return new WaitForSeconds(1);
            NetworkManager.singleton.StartHost();
        }
    }
    public void JoinGame()
    {
        if (joinDialogue.text.Length != 5)
        {
            throw new System.Exception($"Lobby id not correct! Length: {joinDialogue.text.Length} TODO: Implement ui for this");
        }

        CrossSceneUIManager.instance.LoadingScreen(true);
        transport.serverIP = joinDialogue.text;
        NetworkManager.singleton.networkAddress = joinDialogue.text;
        StartCoroutine(Create());
        IEnumerator Create()
        {
            StartConnecting(1);
            yield return new WaitForSeconds(1);
            NetworkManager.singleton.StartClient();
        }
    }
    bool connecting;
    public void StartConnecting(int i)
    {
        if(i == 1)
        {
            connecting = true;
            CrossSceneUIManager.instance.LoadingScreen(true);
            StartCoroutine(WaitForRoom());
        }
        if (i == 2) {
            connecting=false;
        }
    }
    public IEnumerator WaitForRoom()
    {
        float loadingTime = 0;
        while ((loadingTime += Time.deltaTime) < 3)
        {
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Title")
            {
                // connection success
                yield break;
            }
            yield return null;
        }
        CrossSceneUIManager.instance.LoadingScreen(false);
        CrossSceneUIManager.instance.OpenPopup("Failed to join the room. Check the room code and make sure you're connected to the internet.");
    }
    public void HideTitleScreen()
    {
        StartCoroutine(Hide());
        IEnumerator Hide()
        {
            yield return CrossSceneUIManager.instance.GradualFillGraphic(titleScreen, 0);
            Destroy(titleScreen.gameObject);
        }
    }
}
