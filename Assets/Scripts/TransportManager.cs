using System.Collections;
using LightReflectiveMirror;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransportManager : MonoBehaviour
{
    public static TransportManager instance {  get; private set; }
    [SerializeField] TMP_InputField joinDialogue;
    [SerializeField] Image titleScreen;
    [SerializeField] Transport alternativeTransport;
    public bool tutorialMode = false;
    public static LightReflectiveMirrorTransport transport { get; private set; }
    private void Awake()
    {
        foreach (var go in FindObjectsByType<TransportManager>(FindObjectsSortMode.None))
        {
            if (go != this)
            {
                Debug.Log("Destroying old object");
                Destroy(go.gameObject);
                GetComponentInChildren<CrossSceneUIManager>().LoadingScreen(false);
            }
        }
        instance = this;
        transport = GetComponent<LightReflectiveMirrorTransport>();
        Application.targetFrameRate = 120;
        DontDestroyOnLoad(gameObject);
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
    public void StartTutorial()
    {
        StartCoroutine(Tutorial());
        IEnumerator Tutorial()
        {
            CrossSceneUIManager.instance.LoadingScreenDuration();
            tutorialMode = true;
            yield return new WaitForSeconds(1);
            NetworkManager.singleton.transport = alternativeTransport;
            NetworkManager.singleton.StartHost();
        }
    }
    public void EndTutorial()
    {
        StartCoroutine(EndTutorial());
        IEnumerator EndTutorial()
        {
            CrossSceneUIManager.instance.LoadingScreenDuration();
            yield return new WaitForSeconds(1);
            NetworkManager.singleton.StopHost();
            SceneManager.LoadScene("Title");
            Destroy(gameObject);
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

            CrossSceneUIManager.instance.LoadingScreen(true);
            StartCoroutine(WaitForRoom());
            yield return new WaitForSeconds(1);
            NetworkManager.singleton.StartClient();
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
        if(!transport.Available())
        {
            CrossSceneUIManager.instance.OpenPopup("Failed to connect to server! Sending you back to the lobby.");
            transport.ConnectToRelay();
            return;
        }
        StartCoroutine(Hide());
        IEnumerator Hide()
        {
            yield return CrossSceneUIManager.instance.GradualFillGraphic(titleScreen, 0);
            Destroy(titleScreen.gameObject);
        }
    }
}
