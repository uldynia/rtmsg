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

    bool waitingForMerge, waitingForDeploy, waitingForAttack;
    public void MergeTutorialComplete()
    {
        if(!waitingForMerge)
        {
            waitingForMerge = true;
        }
    }
    public void DeployTutorialComplete()
    {
        if (!waitingForDeploy)
        {
            waitingForDeploy = true;
        }
    }

    public void TakeDamageTutorialComplete()
    {
        if(!waitingForAttack) { waitingForAttack = true; }
    }
    IEnumerator FirstTutorial()
    {
        stage = TUTORIALSTAGE.START;
        yield return SetCanvas(canvas, true);
        yield return new WaitForSeconds(1);
        yield return SetCanvas(firstTutorialCanvases[0], true);
        yield return new WaitForSeconds(2);
        yield return SetCanvas(firstTutorialCanvases[0], false);
        yield return SetCanvas(firstTutorialCanvases[1], true);

        stage = TUTORIALSTAGE.MERGE;
        background.sizeDelta = background.sizeDelta - new Vector2(0, 150);
        yield return new WaitForSeconds(1);
        InventoryManager.m_instance.SpawnNewAnimal(1);
        yield return new WaitForSeconds(0.2f);
        InventoryManager.m_instance.SpawnNewAnimal(1);
        while (!waitingForMerge) yield return null;
        yield return SetCanvas(firstTutorialCanvases[1], false);

        stage = TUTORIALSTAGE.START;
        background.sizeDelta = background.sizeDelta + new Vector2(0, 150);
        yield return SetCanvas(firstTutorialCanvases[2], true);
        yield return new WaitForSeconds(2);
        yield return SetCanvas(firstTutorialCanvases[2], false);
        yield return SetCanvas(firstTutorialCanvases[3], true);
        var target = firstTutorialCanvases[3].transform.position.y + 150;
        while (firstTutorialCanvases[3].transform.position.y < target)
        {
            firstTutorialCanvases[3].transform.position += Vector3.up * Time.deltaTime * Mathf.Max(50, target - firstTutorialCanvases[3].transform.position.y) * 10;
            yield return null;
        }
        stage = TUTORIALSTAGE.DEPLOY;
        yield return new WaitForSeconds(1);
        background.sizeDelta = background.sizeDelta - new Vector2(0, 300);

        while (!waitingForDeploy) yield return null;
        yield return SetCanvas(firstTutorialCanvases[3], false);

        stage = TUTORIALSTAGE.START;
        yield return SetCanvas(canvas, false);
        background.sizeDelta = background.sizeDelta + new Vector2(0, 300);
        while (!waitingForAttack) yield return null;

        yield return new WaitForSeconds(1);
        yield return SetCanvas(canvas, true);
        yield return SetCanvas(firstTutorialCanvases[4], true);
        yield return new WaitForSeconds(2);
        yield return SetCanvas(firstTutorialCanvases[4], false);
        yield return SetCanvas(firstTutorialCanvases[5], true);

        StartCoroutine(SetCanvas(background.GetComponent<CanvasGroup>(), false));
        GameManager.instance.SpawnEntity(15, new Vector3(-2, 5), -1, 1);
        GameManager.instance.SpawnEntity(15, new Vector3(1, 5), -1, 1);
        target = firstTutorialCanvases[5].transform.position.y - 300;
        InventoryManager.m_instance.SpawnNewAnimal(0);
        InventoryManager.m_instance.SpawnNewAnimal(0);
        InventoryManager.m_instance.SpawnNewAnimal(0);
        while (firstTutorialCanvases[5].transform.position.y > target)
        {
            firstTutorialCanvases[5].transform.position -= Vector3.up * Time.deltaTime * Mathf.Max(50, firstTutorialCanvases[5].transform.position.y - target) * 10;
            yield return null;
        }

    }
    
}
