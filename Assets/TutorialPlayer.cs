using UnityEngine;

public class TutorialPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!TransportManager.instance.tutorialMode)
        {
            Debug.Log("Not tutorial! Destroying now");
            DestroyImmediate(gameObject);
        }
        GameManager.instance.playerTwoHealth = 50;
        PlayerController.localPlayer.RequestHealthUIUpdate();
        InvokeRepeating("SpawnSheep", 3, 3);
    }

    public void SpawnSheep()
    {
        GameManager.instance.SpawnEntity(1, new Vector3(-2.5f, 3), -1, 1);
    }
}
