using System.Collections;
using UnityEngine;

public class TutorialPlayer : MonoBehaviour
{
    //GameManager.instance.SpawnEntity(1, new Vector3(-2.5f, 3), -1, 1);
    [SerializeField] CanvasGroup canvas;
    [SerializeField] CanvasGroup[] firstTutorialCanvases;
    [SerializeField] RectTransform background;
    public static TutorialPlayer instance { get; private set; }
    
    public enum TUTORIALSTAGE
    {
        START,
        MERGE,
        DEPLOY
    }
    public TUTORIALSTAGE stage;
    IEnumerator SetCanvas(CanvasGroup _canvas, bool enable)
    {
        _canvas.blocksRaycasts = enable;
        _canvas.interactable = enable;
        if(enable)
        {
            while ((_canvas.alpha += Time.deltaTime * 5) < 1) yield return null;
        }
        else
        {
            while ((_canvas.alpha -= Time.deltaTime * 5) > 0) yield return null;
        }
    }
    void Start()
    {
        instance = this;
        if (!TransportManager.instance.tutorialMode)
        {
            Debug.Log("Not tutorial! Destroying now");
            DestroyImmediate(gameObject);
        }
        GameManager.instance.playerTwoHealth = 50;
        PlayerController.localPlayer.RequestHealthUIUpdate();
        InventoryManager.m_instance.spawn_interval = 0;
        StartCoroutine(FirstTutorial());
    }

    IEnumerator FirstTutorial()
    {
        yield return SetCanvas(canvas, true);
        yield return new WaitForSeconds(1);
        yield return SetCanvas(firstTutorialCanvases[0], true);
        yield return new WaitForSeconds(2);
        yield return SetCanvas(firstTutorialCanvases[0], true);
        yield return SetCanvas(firstTutorialCanvases[1], true);
        stage = TUTORIALSTAGE.MERGE;
        background.sizeDelta = background.sizeDelta - new Vector2(0, 150);
        yield return new WaitForSeconds(1);
        InventoryManager.m_instance.SpawnNewAnimal(1);
        yield return new WaitForSeconds(0.2f);
        InventoryManager.m_instance.SpawnNewAnimal(1);
    }
}
