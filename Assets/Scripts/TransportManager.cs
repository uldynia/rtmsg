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
            yield return leaveAnimation();
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
            yield return leaveAnimation();
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
            PlayerPrefs.SetInt("Tutorial", 1);
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
            CrossSceneUIManager.instance.OpenPopup("Invalid room code!");
        }
        transport.serverIP = joinDialogue.text;
        NetworkManager.singleton.networkAddress = joinDialogue.text;
        StartCoroutine(Create());
        IEnumerator Create()
        {
            yield return leaveAnimation();
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
    [SerializeField] Image archive;
    public void ToggleArchive(bool show)
    {
        var group = archive.GetComponent<CanvasGroup>();
        group.interactable = show;
        group.blocksRaycasts = show;
        StartCoroutine(CrossSceneUIManager.instance.GradualFillGraphic(archive, show ? 1 : 0));
    }
    public void HideTitleScreen()
    {
        if(!transport.Available())
        {
            CrossSceneUIManager.instance.OpenPopup("Failed to connect to server! Try again in a bit.");
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
    [SerializeField] GameObject background;
    [SerializeField] SpriteRenderer leftAnimal, rightAnimal;
    public IEnumerator leaveAnimation()
    {
        background.transform.localScale = Vector3.one * 0.7f;
        Debug.Log(background.transform.localScale.magnitude);
        while(background.transform.localScale.magnitude < 1.3f)
        {
            background.transform.localScale += Vector3.one * Time.deltaTime * 0.07f;
            leftAnimal.color -= new Color(0, 0, 0, Time.deltaTime * 2);
            leftAnimal.transform.position += Vector3.left * Time.deltaTime * 5;
            rightAnimal.color -= new Color(0, 0, 0, Time.deltaTime * 2);
            rightAnimal.transform.position += Vector3.right * Time.deltaTime * 5;

            yield return null;
        }
    }
}
