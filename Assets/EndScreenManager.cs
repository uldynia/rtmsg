using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] CanvasGroup HUD, loseScreen, winScreen;
    public static EndScreenManager instance { get; private set; }
    private void Start()
    {
        instance = this;
    }
    public void ShowEndScreen(bool won, Vector3 cameraPosition)
    {
        StartCoroutine(Screen());
        IEnumerator Screen()
        {
            while((HUD.alpha -= Time.deltaTime * 3) > 0)
            {
                Camera.main.transform.position += (cameraPosition - Camera.main.transform.position) * Time.deltaTime * 3;
                Camera.main.orthographicSize -= Time.deltaTime * 6;
                yield return null;
            }
            yield return new WaitForSeconds(2);
            var screen = won ? winScreen : loseScreen;
            screen.gameObject.SetActive(true);
            while( (screen.alpha += Time.deltaTime * 3) < 1)
            {
                yield return null;
            }
            yield return new WaitForSeconds(2);
            if(TransportManager.instance.tutorialMode)
            {
                TransportManager.instance.EndTutorial();
                yield break;
            }
            var text = screen.transform.Find("returntobase").GetComponent<TextMeshProUGUI>();
            for (uint i = 5; i > 0; i--)
            {
                text.text = $"Returning to screen in {i} seconds.";
                yield return new WaitForSeconds(0.8f);
            }
            CrossSceneUIManager.instance.LoadingScreenDuration();
            yield return new WaitForSeconds(1);
            NetworkManager.singleton.ServerChangeScene("Lobby");
        }
    }
}
