using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrossSceneUIManager : MonoBehaviour
{
    public static CrossSceneUIManager instance { get; private set; }
    [SerializeField] Image loadingScreen;
    [SerializeField] CanvasGroup popup;
    [SerializeField] TextMeshProUGUI popupText;
    void Start()
    {
        instance = this;
    }
    public void LoadingScreenDuration(int time = 3)
    {
        StartCoroutine(Screen());
        IEnumerator Screen()
        {
            loadingScreen.fillAmount = 0;
            LoadingScreen(true);
            yield return new WaitForSeconds(time);
            LoadingScreen(false);
        }
    }
    public void LoadingScreen(bool open)
    {
        StartCoroutine(Loading());
        IEnumerator Loading()
        {
            if(open)
                loadingScreen.gameObject.SetActive(true);
            loadingScreen.fillAmount = open?0:1;
            yield return GradualFillGraphic(loadingScreen, open ? 1 : 0, 2);
            if (!open)
                loadingScreen.gameObject.SetActive(false);
        }
    }
    public void OpenPopup(string text)
    {
        StartCoroutine(open());
        IEnumerator open()
        {
            popup.gameObject.SetActive(true);
            popupText.text = text;
            popup.transform.localScale = Vector3.one * 0.8f;
            while(popup.transform.localScale.sqrMagnitude < 1)
            {
                popup.transform.localScale += new Vector3(Time.deltaTime, Time.deltaTime);
                yield return null;
            }
        }
    }
    public void ClosePopup ()
    {
        StartCoroutine(close());
        IEnumerator close()
        {
            while (popup.transform.localScale.sqrMagnitude < 0.8f)
            {
                popup.transform.localScale -= new Vector3(Time.deltaTime, Time.deltaTime);
                yield return null;
            }
            popup.gameObject.SetActive(false);
        }
    }
    public IEnumerator GradualFillGraphic(Image image, float targetValue, float multiplier = 1)
    {
        if (image.fillAmount < targetValue)
        {
            image.fillAmount = 0;
            while (image != null && (image.fillAmount += Time.deltaTime * multiplier * 5 * Mathf.Max(image.fillAmount, 0.1f)) < targetValue)
            {
                yield return null;
            }
        }
        else
        {
            image.fillAmount = 1;
            while (image != null && (image.fillAmount -= Time.deltaTime * 5 * multiplier * Mathf.Max(image.fillAmount, 0.1f)) > targetValue) yield return null;
        }
    }
}
